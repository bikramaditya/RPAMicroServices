﻿using Microsoft.ServiceFabric.Services.Communication.Runtime;
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

namespace RemoteExecutor
{
    /// <summary>
    /// An instance of this class is created for each service replica by the Service Fabric runtime.
    /// </summary>
    internal sealed class RemoteExecutor : StatefulService
    {        
        public RemoteExecutor(StatefulServiceContext serviceContext) : base(serviceContext)
        {
        }

        protected override IEnumerable<ServiceReplicaListener> CreateServiceReplicaListeners()
        {

            string listenQueueName = CloudConfigurationManager.GetSetting("execution");

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
        QueueClient _executionQueueClient;
        QueueClient _moreInfoQueueClient;
        MySql.Data.MySqlClient.MySqlConnection _conn;
        string _myConnectionString;

        public Handler(StatefulService service)
        {
            string sendConnString = CloudConfigurationManager.GetSetting("Microsoft.ServiceBus.ConnectionString.Send");

            string errorQueueName = CloudConfigurationManager.GetSetting("errorQueueName");
            string executionQueueName = CloudConfigurationManager.GetSetting("executionQueueName");
            string moreInfoQueueName = CloudConfigurationManager.GetSetting("moreInfoQueueName");

            _errorQueueClient = QueueClient.CreateFromConnectionString(sendConnString, errorQueueName);
            _executionQueueClient = QueueClient.CreateFromConnectionString(sendConnString, executionQueueName);
            _moreInfoQueueClient = QueueClient.CreateFromConnectionString(sendConnString, moreInfoQueueName);

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
                bool isValid = checkInfo(ticket);

                ticket = populateVariables(ticket);

                if (isValid)
                {
                    sendToExecutionQueue(message);
                }
                else
                {
                    sendToMoreInfoQueue(message);
                }
            }
            catch (Exception e)
            {
                if (ticket != null)
                {

                }
                ticket.Error = e.Message;
                sendToErrorQueue(message);
                return Task.FromResult(false);
            }

            return Task.FromResult(true);
        }

        private RPATicket populateVariables(RPATicket ticket)
        {
            RPAResult result = ticket.Matches[0];

            String userMsg = result.UserConfirmationMsg;
            String scriptText = result.ScriptText;

            var pattern = @"\{(.*?)\}";
            
            var matches = Regex.Matches(userMsg, pattern);

            foreach (Match m in matches)
            {
                String variable = m.Groups[1].Value;
                String VarRegex = getPatternFromDB(variable);
                string value = getFirstMatchFrom_Ticket_Desc(ticket, VarRegex);
                userMsg = userMsg.Replace("{"+ variable+"}",value);
            }

            matches = Regex.Matches(scriptText, pattern);

            foreach (Match m in matches)
            {
                String variable = m.Groups[1].Value;
                String VarRegex = getPatternFromDB(variable);
                string value = getFirstMatchFrom_Ticket_Desc(ticket, VarRegex);
                scriptText = scriptText.Replace("{" + variable + "}", value);
            }

            ticket.Matches[0].UserConfirmationMsg = userMsg;
            ticket.Matches[0].ScriptText = scriptText;

            return ticket;
        }

        private string getFirstMatchFrom_Ticket_Desc(RPATicket ticket, string pattern)
        {
            String variable="";
            String desc = ticket.TicketDescription;            
            var matches = Regex.Matches(desc, pattern);

            foreach (Match m in matches)
            {
                variable = m.Groups[0].Value;
                break;
            }

            return variable;
        }

        private string getPatternFromDB(string variable)
        {
            String varRegex = "";
            try
            {
                checkAndOpenConn();
                MySqlCommand cmd = _conn.CreateCommand();

                cmd.CommandText = "select regex_pattern from variable_regex_map where variable_name='"+variable+"'";
                MySqlDataAdapter dap = new MySqlDataAdapter(cmd);
                MySqlDataReader dataReader = cmd.ExecuteReader();

                while (dataReader.Read())
                {
                    varRegex = dataReader.GetString("regex_pattern");
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

            return varRegex;
        }

        private void checkAndOpenConn()
        {
            _conn = new MySql.Data.MySqlClient.MySqlConnection();
            _conn.ConnectionString = _myConnectionString;
            _conn.Open();
        }

        private void sendToExecutionQueue(BrokeredMessage message)
        {
            try
            {
                BrokeredMessage ticketMsg = message.Clone();

                _executionQueueClient.Send(ticketMsg);
            }
            catch (Exception e)
            {
                Console.Write(e.Message);
            }
        }

        private bool checkInfo(RPATicket ticket)
        {
            return true;
        }

        private void sendToErrorQueue(BrokeredMessage message)
        {
            try
            {
                BrokeredMessage ticketMsg = message.Clone();

                _errorQueueClient.Send(ticketMsg);
            }
            catch (Exception e)
            {
                Console.Write(e.Message);
            }

        }
        private void sendToMoreInfoQueue(BrokeredMessage message)
        {
            try
            {
                BrokeredMessage ticketMsg = message.Clone();

                _moreInfoQueueClient.Send(ticketMsg);
            }
            catch (Exception e)
            {
                Console.Write(e.Message);
            }

        }
    }

}
