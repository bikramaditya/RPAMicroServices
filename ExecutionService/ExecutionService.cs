using Microsoft.ServiceFabric.Services.Communication.Runtime;
using Microsoft.ServiceFabric.Services.Runtime;
using System.Collections.Generic;
using System.Fabric;
using System.Threading;
using Microsoft.Azure;
using Microsoft.ServiceBus.Messaging;
using Microsoft.ServiceFabric.Data;
using ServiceFabric.ServiceBus.Services;
using ServiceFabric.ServiceBus.Services.CommunicationListeners;
using System;
using System.Threading.Tasks;
using System.IO;
using RPA;
using System.Text.RegularExpressions;
using MySql.Data.MySqlClient;

namespace ExecutionService
{
    /// <summary>
    /// An instance of this class is created for each service replica by the Service Fabric runtime.
    /// </summary>
    internal sealed class ExecutionService : StatefulService
    {
        public ExecutionService(StatefulServiceContext serviceContext) : base(serviceContext)
        {
        }

        protected override IEnumerable<ServiceReplicaListener> CreateServiceReplicaListeners()
        {

            string listenQueueName = CloudConfigurationManager.GetSetting("executionQueueName");

            yield return new ServiceReplicaListener(context => new ServiceBusQueueCommunicationListener(
                new Handler(this)
                , context
                , listenQueueName
                , requireSessions: false), "StatefulService-ServiceBusSubscriptionListener");
        }
    }
    internal sealed class Handler : AutoCompleteServiceBusMessageReceiver
    {
        private readonly StatefulService _service;
        QueueClient _errorQueueClient;

        MySql.Data.MySqlClient.MySqlConnection _conn;
        string _myConnectionString;

        public Handler(StatefulService service)
        {
            string sendConnString = CloudConfigurationManager.GetSetting("Microsoft.ServiceBus.ConnectionString.Send");

            string errorQueueName = CloudConfigurationManager.GetSetting("errorQueueName");
            string executionQueueName = CloudConfigurationManager.GetSetting("executionQueueName");
            string moreInfoQueueName = CloudConfigurationManager.GetSetting("moreInfoQueueName");

            _errorQueueClient = QueueClient.CreateFromConnectionString(sendConnString, errorQueueName);

            _service = service;
            
            String DBHost = CloudConfigurationManager.GetSetting("DBHost");
            String DBName = CloudConfigurationManager.GetSetting("DBName");
            String DBUser = CloudConfigurationManager.GetSetting("DBUser");
            String DBPW = CloudConfigurationManager.GetSetting("DBPW");

            _myConnectionString = "Server=" + DBHost + ";uid=" + DBUser + ";" + "pwd=" + DBPW + ";database=" + DBName + ";";

            _conn = new MySql.Data.MySqlClient.MySqlConnection();
            _conn.ConnectionString = _myConnectionString;
            _conn.Open();
        }

        protected override Task ReceiveMessageImplAsync(BrokeredMessage message, MessageSession session, CancellationToken cancellationToken)
        {
            ServiceEventSource.Current.ServiceMessage(_service, $"Handling queue message {message.MessageId} in session {session?.SessionId ?? "none"}");

            RPATicket ticket = null;
            try
            {
                ticket = message.GetBody<RPATicket>();
                bool isValid = validate(ticket);
                
                if (isValid)
                {
                    executionScript(ticket);
                }
                else
                {
                    sendToErrorQueue(ticket);
                }
            }
            catch (Exception e)
            {
                ticket.Error = e.Message;
                sendToErrorQueue(ticket);
                return Task.FromResult(false);
            }
            storeResultToDB(ticket);

            return Task.FromResult(true);
            
        }

        private void storeResultToDB(RPATicket ticket)
        {
            checkAndOpenConn();
            //store results success and error
        }
        private void checkAndOpenConn()
        {
            _conn = new MySql.Data.MySqlClient.MySqlConnection();
            _conn.ConnectionString = _myConnectionString;
            _conn.Open();
        }

        private void executionScript(RPATicket ticket)
        {
            try
            {
                //connect to remote machine and execute
            }
            catch (Exception e)
            {
                Console.Write(e.Message);
            }
        }

        private bool validate(RPATicket ticket)
        {
            return true;
        }

        private void sendToErrorQueue(RPATicket ticket)
        {
            try
            {
                BrokeredMessage ticketMsg = new BrokeredMessage(ticket);

                _errorQueueClient.Send(ticketMsg);
            }
            catch (Exception e)
            {
                Console.Write(e.Message);
            }

        }
    }
}
