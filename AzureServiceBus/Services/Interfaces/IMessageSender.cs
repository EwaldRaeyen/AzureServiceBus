using AzureServiceBus.Message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AzureServiceBus.Services.Interfaces
{
    public interface IMessageSender
    {
        Task SendAsync(string message);
        Task SendEventAsync(Event @event, string topicName, CancellationToken cancellationToken);
    }
}
