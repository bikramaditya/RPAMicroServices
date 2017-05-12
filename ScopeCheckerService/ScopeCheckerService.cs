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
using SharedService.lucene;
using Microsoft.Azure; 
using Microsoft.WindowsAzure.Storage; 
using Microsoft.WindowsAzure.Storage.Blob;
using System.Xml.Serialization;
using System.Xml;

namespace ScopeCheckerService
{
    /// <summary>
    /// An instance of this class is created for each service replica by the Service Fabric runtime.
    /// </summary>
    internal sealed class ScopeCheckerService : StatefulService
    {
        public ScopeCheckerService(StatefulServiceContext serviceContext) : base(serviceContext)
        {
        }

        public ScopeCheckerService(StatefulServiceContext serviceContext, IReliableStateManagerReplica reliableStateManagerReplica) : base(serviceContext, reliableStateManagerReplica)
        {
        }

        protected override IEnumerable<ServiceReplicaListener> CreateServiceReplicaListeners()
        {

            string listenQueueName = CloudConfigurationManager.GetSetting("validQueueName");

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
        QueueClient _validQueueClient;
        QueueClient _inScopeQueueClient;
        QueueClient _ooScopeQueueClient;
        
        public Handler(StatefulService service)
        {

            string sendConnString = CloudConfigurationManager.GetSetting("Microsoft.ServiceBus.ConnectionString.Send");

            string errorQueueName = CloudConfigurationManager.GetSetting("errorQueueName");
            string validQueueName = CloudConfigurationManager.GetSetting("validQueueName");
            string inScopeQueueName = CloudConfigurationManager.GetSetting("inScopeQueueName");
            string ooScopeQueueName = CloudConfigurationManager.GetSetting("ooScopeQueueName");

            _errorQueueClient = QueueClient.CreateFromConnectionString(sendConnString, errorQueueName);
            _validQueueClient = QueueClient.CreateFromConnectionString(sendConnString, validQueueName);
            _inScopeQueueClient = QueueClient.CreateFromConnectionString(sendConnString, inScopeQueueName);
            _ooScopeQueueClient = QueueClient.CreateFromConnectionString(sendConnString, ooScopeQueueName);

            _service = service;
        }

        protected override Task ReceiveMessageImplAsync(BrokeredMessage message, MessageSession session, CancellationToken cancellationToken)
        {
            ServiceEventSource.Current.ServiceMessage(_service, $"Handling queue message {message.MessageId} in session {session?.SessionId ?? "none"}");

            RPA.RPATicket ticket;
            try
            {
                ticket = message.GetBody<RPA.RPATicket>();
                ticket = checkScope(ticket);
                
                if (ticket.Matches.Count > 0)
                {
                    sendToInScopeQueue(ticket);
                }
                else
                {
                    sendToOutOfScopeQueue(message);
                }
            }
            catch (Exception)
            {
                sendToErrorQueue(message);
                return Task.FromResult(false);
            }

            return Task.FromResult(true);
        }
        
        private void sendToInScopeQueue(RPA.RPATicket ticket)
        {
            try
            {
                BrokeredMessage ticketMsg = new BrokeredMessage(ticket);

                _inScopeQueueClient.Send(ticketMsg);
            }
            catch (Exception e)
            {
                Console.Write(e.Message);
            }
        }

        private RPA.RPATicket checkScope(RPA.RPATicket ticket)
        {
            SharedService.lucene.SearchWebService service = new SharedService.lucene.SearchWebService();
            
            ticket = service.GetSearchResults(ticket);
            
            return ticket;
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
        private void sendToOutOfScopeQueue(BrokeredMessage message)
        {
            try
            {
                BrokeredMessage ticketMsg = message.Clone();

                _ooScopeQueueClient.Send(ticketMsg);
            }
            catch (Exception e)
            {
                Console.Write(e.Message);
            }

        }
    }
}
