using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AzureServiceBus.Message
{
    public interface IBaseMessage
    {
        public Guid Id { get; init; }
        public Guid CorrelationId { get; init; }
        public string Causation { get; init; }
        public object Data { get; init; }
    }
}
