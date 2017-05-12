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

namespace FeedBackService
{
    /// <summary>
    /// An instance of this class is created for each service replica by the Service Fabric runtime.
    /// </summary>
    internal sealed class FeedBackService : StatefulService
    {
        public FeedBackService(StatefulServiceContext serviceContext) : base(serviceContext)
        {
        }

        public FeedBackService(StatefulServiceContext serviceContext, IReliableStateManagerReplica reliableStateManagerReplica) : base(serviceContext, reliableStateManagerReplica)
        {
        }

        protected override IEnumerable<ServiceReplicaListener> CreateServiceReplicaListeners()
        {

            string listenQueueName = CloudConfigurationManager.GetSetting("feedbackQueueName");

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
        QueueClient _feedbackQueueClient;


        public Handler(StatefulService service)
        {

            string sendConnString = CloudConfigurationManager.GetSetting("Microsoft.ServiceBus.ConnectionString.Send");

            string errorQueueName = CloudConfigurationManager.GetSetting("errorQueueName");           
            string feedbackQueueName = CloudConfigurationManager.GetSetting("feedbackQueueName");

            _errorQueueClient = QueueClient.CreateFromConnectionString(sendConnString, errorQueueName);
            _feedbackQueueClient = QueueClient.CreateFromConnectionString(sendConnString, feedbackQueueName);

            _service = service;
        }

        protected override Task ReceiveMessageImplAsync(BrokeredMessage message, MessageSession session, CancellationToken cancellationToken)
        {
            ServiceEventSource.Current.ServiceMessage(_service, $"Handling queue message {message.MessageId} in session {session?.SessionId ?? "none"}");

            RPA.RPATicket ticket;
            try
            {
                ticket = message.GetBody<RPA.RPATicket>();
                sendFeedbackWS(ticket);                
            }
            catch (Exception)
            {
                sendToErrorQueue(message);
                return Task.FromResult(false);
            }
            return Task.FromResult(true);
        }
        
        private RPA.RPATicket sendFeedbackWS(RPA.RPATicket ticket)
        {
            SharedService.lucene.SearchWebService service = new SharedService.lucene.SearchWebService();

            //RPA.RPAResult m = ticket.Matches[0];
            RPA.RPAResult m = new RPA.RPAResult();
            String desc = ticket.TicketDescription;
            m.ScriptText += " " + desc;
            m.ScriptID = 1;//to be removed

            service.UpdateLucene(m);
            
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

                _feedbackQueueClient.Send(ticketMsg);
            }
            catch (Exception e)
            {
                Console.Write(e.Message);
            }

        }
    }
}
