using Akkatecture.Core;
using Akkatecture.ValueObjects;
using Newtonsoft.Json;

namespace Domain.Model.GiftCard
{
    [JsonConverter(typeof(SingleValueObjectConverter))]
    public class GiftCardId: Identity<GiftCardId>
    {
        public GiftCardId(string value)
            : base(value)
        {  
        }
    }
}