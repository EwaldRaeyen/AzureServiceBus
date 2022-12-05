using Azure.Messaging.ServiceBus;
using AzureServiceBus.Message;
using AzureServiceBus.Services.Interfaces;
using AzureServiceBus.Settings;
using Microsoft.Azure.Amqp.Framing;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AzureServiceBus.Services
{
    public class AzureServiceBusReceiver : IMessageReceiver, IAsyncDisposable
    {
        private readonly ServiceBusClient _client;
        private readonly AzureServiceBusSettings _azureServiceBusSettings;
        private readonly ServiceBusReceiver _serviceBusReceiver;

        public AzureServiceBusReceiver(ServiceBusClient client, IOptions<AzureServiceBusSettings> azureServiceBusSettings)
        {
            _client = client ?? throw new ArgumentNullException(nameof(client));
            _azureServiceBusSettings = azureServiceBusSettings?.Value ?? throw new ArgumentNullException(nameof(azureServiceBusSettings));
        }

        public async ValueTask DisposeAsync()
        {
            await _client.DisposeAsync();
        }

        public async Task<ServiceBusReceivedMessage> Peek(CancellationToken cancellationToken)
        {
            return await _serviceBusReceiver.PeekMessageAsync(cancellationToken: cancellationToken);
        }

        public async Task<string> ConsumeAsync()
        {
            ServiceBusReceivedMessage receivedMessage = await _serviceBusReceiver.ReceiveMessageAsync();
            string body = receivedMessage.Body.ToString();

            await CompleteMessage(receivedMessage);

            return body;
        }

        public async Task<IBaseMessage> ConsumeEventAsync(string topicName, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(topicName))
                throw new ArgumentException(nameof(topicName));

            await using var _serviceBusReceiver = _client.CreateReceiver(_azureServiceBusSettings.TodoItemQueue);
            
            ServiceBusReceivedMessage receivedMessage = await _serviceBusReceiver.ReceiveMessageAsync();
            if (receivedMessage == null)
                throw new Exception("no message");

            Event? @event = JsonConvert.DeserializeObject<Event>(receivedMessage.Body.ToString());
            if (@event == null)
                throw new Exception("event is null");

            await CompleteMessage(receivedMessage);

            return @event;
        }

        private async Task CompleteMessage(ServiceBusReceivedMessage receivedMessage)
        {
            await _serviceBusReceiver.CompleteMessageAsync(receivedMessage);
        }
    }
}
