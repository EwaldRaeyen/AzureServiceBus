using Azure.Core.Amqp;
using Azure.Messaging.ServiceBus;
using AzureServiceBus.Message;
using AzureServiceBus.Services.Interfaces;
using AzureServiceBus.Settings;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System.Text;

namespace AzureServiceBus.Services
{
    public class AzureServiceBusSender : IMessageSender, IAsyncDisposable
    {
        private readonly ServiceBusClient _client;
        private readonly AzureServiceBusSettings _azureServiceBusSettings;

        public AzureServiceBusSender(ServiceBusClient client, IOptions<AzureServiceBusSettings> azureServiceBusSettings)
        {
            _client = client ?? throw new ArgumentNullException(nameof(client));
            _azureServiceBusSettings = azureServiceBusSettings?.Value ?? throw new ArgumentNullException(nameof(azureServiceBusSettings));
        }

        public async ValueTask DisposeAsync()
        {
            await _client.DisposeAsync();
        }

        public async Task SendAsync(string text)
        {
            var serviceBusSender = _client.CreateSender(_azureServiceBusSettings.TodoItemQueue);

            text = text ?? throw new ArgumentNullException(nameof(text));

            ServiceBusMessage message = new ServiceBusMessage(text);

            await serviceBusSender.SendMessageAsync(message);
        }

        public async Task SendEventAsync(Event @event, string topicName, CancellationToken cancellationToken)
        {
            if (@event == null)
                throw new ArgumentNullException(nameof(@event));
            if (string.IsNullOrWhiteSpace(topicName))
                throw new ArgumentException(nameof(topicName));
                        
            ServiceBusMessage message = new ServiceBusMessage
            {
                ContentType = "application/json",
                Body = new BinaryData(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(@event)))
            };

            await using var serviceBusSender = _client.CreateSender(topicName);

            // how to know if topic exist
            await serviceBusSender.SendMessageAsync(message, cancellationToken);
        }
    }
}
