using Azure.Messaging.ServiceBus;
using AzureServiceBus.Message;
using AzureServiceBus.Services.Interfaces;
using AzureServiceBus.Settings;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AzureServiceBus.Services
{
    public class AzureServiceBusTopicReader : ITopicMessageReceiver
    {
        private readonly ServiceBusClient _client;
        private readonly AzureServiceBusSettings _azureServiceBusSettings;
        private readonly ServiceBusReceiver _serviceBusReceiver;

        public AzureServiceBusTopicReader(ServiceBusClient client, IOptions<AzureServiceBusSettings> azureServiceBusSettings)
        {
            _client = client ?? throw new ArgumentNullException(nameof(client));
            _azureServiceBusSettings = azureServiceBusSettings?.Value ?? throw new ArgumentNullException(nameof(azureServiceBusSettings));
        }

        public async ValueTask DisposeAsync()
        {
            await _client.DisposeAsync();
        }

        public async Task<Event> ReceiveMessageAsync(string topicName, string subscriptionName, CancellationToken cancellationToken)
        {
            //var subscriptionPath = SubscriptionClient.FormatSubscriptionPath(topicName, subscriptionName);
            //var receiver = new MessageReceiver(connectionString, subscriptionPath, ReceiveMode.PeekLock);

            await using var _serviceBusReceiver = _client.CreateReceiver(topicName, subscriptionName);

            ServiceBusReceivedMessage receivedMessage = await _serviceBusReceiver.ReceiveMessageAsync();
            if (receivedMessage == null)
                throw new Exception("no message");

            Event? @event = JsonConvert.DeserializeObject<Event>(receivedMessage.Body.ToString());
            if (@event == null)
                throw new Exception("event is null");

            await _serviceBusReceiver.DeferMessageAsync(receivedMessage);

            return @event;
        }
    }
}
