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
using RPA;

namespace ValidationService
{
    /// <summary>
    /// An instance of this class is created for each service replica by the Service Fabric runtime.
    /// </summary>
    internal sealed class ValidationService : StatefulService
    {
        public ValidationService(StatefulServiceContext serviceContext) : base(serviceContext)
		{
        }
        
        protected override IEnumerable<ServiceReplicaListener> CreateServiceReplicaListeners()
        {

            string listenQueueName = CloudConfigurationManager.GetSetting("acquisitionQueueName");

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

        public Handler(StatefulService service)
        {
            string errorQueueName = CloudConfigurationManager.GetSetting("errorQueueName");
            string validQueueName = CloudConfigurationManager.GetSetting("validQueueName");
            string sendConnString = CloudConfigurationManager.GetSetting("Microsoft.ServiceBus.ConnectionString.Send");

            _errorQueueClient = QueueClient.CreateFromConnectionString(sendConnString, errorQueueName);
            _validQueueClient = QueueClient.CreateFromConnectionString(sendConnString, validQueueName);

            _service = service;
        }

        protected override Task ReceiveMessageImplAsync(BrokeredMessage message, MessageSession session, CancellationToken cancellationToken)
        {
            ServiceEventSource.Current.ServiceMessage(_service, $"Handling queue message {message.MessageId} in session {session?.SessionId ?? "none"}");

            RPATicket ticket;
            try
            {
                ticket = message.GetBody<RPATicket>();
                ticket.TicketId = Guid.NewGuid().ToString();
                bool isValid = validate(ticket);
                if (isValid)
                {
                    sendToValidQueue(new BrokeredMessage(ticket));
                }
                else
                {
                    sendToErrorQueue(message);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                sendToErrorQueue(message);
                return Task.FromResult(false);
            }

            return Task.FromResult(true);
        }

        private void sendToValidQueue(BrokeredMessage message)
        {
            try
            {
                BrokeredMessage ticketMsg = message.Clone();

                _validQueueClient.Send(ticketMsg);
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
    }
}
