using AzureServiceBus.Message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AzureServiceBus.Services.Interfaces
{
    public interface ITopicMessageReceiver
    {
        Task<Event> ReceiveMessageAsync(string topicName, string subscriptionName, CancellationToken cancellationToken);
    }
}
