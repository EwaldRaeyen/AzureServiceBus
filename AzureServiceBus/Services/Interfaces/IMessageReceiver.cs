using AzureServiceBus.Message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AzureServiceBus.Services.Interfaces
{
    public interface IMessageReceiver
    {
        Task<string> ConsumeAsync();
        Task<IBaseMessage> ConsumeEventAsync(string topicName, CancellationToken cancellationToken);
    }
}
