using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AzureServiceBus.Message
{
    public class Event : IBaseMessage
    {
        public Guid Id { get; init; }
        public Guid CorrelationId { get; init; }
        public string Causation { get; init; }
        public object Data { get; init; }

        public Event(Guid correlationId, string causation, object data)
        {
            if (string.IsNullOrWhiteSpace(causation))
                throw new ArgumentNullException(nameof(causation));

            if (data == null)
                throw new ArgumentNullException(nameof(data));

            Id = Guid.NewGuid();
            CorrelationId = correlationId;
            Causation = causation;
            Data = data;
        }

        public override string ToString()
        {
            return $"{nameof(Id)}: {Id}, {nameof(Causation)}: {Causation}, {nameof(CorrelationId)}: {CorrelationId}, {nameof(Data)}: {Data}";
        }
    }
}
