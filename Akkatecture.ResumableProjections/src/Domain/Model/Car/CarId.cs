using Akkatecture.Core;
using Akkatecture.ValueObjects;
using Newtonsoft.Json;

namespace Domain.Model.Car
{
    [JsonConverter(typeof(SingleValueObjectConverter))]
    public class CarId: Identity<CarId>
    {
        public CarId(string value)
            : base(value)
        {  
        }
    }
}