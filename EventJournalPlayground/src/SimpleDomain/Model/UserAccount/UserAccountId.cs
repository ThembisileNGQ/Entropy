using Akkatecture.Core;
using Akkatecture.ValueObjects;
using Newtonsoft.Json;

namespace SimpleDomain.Model.UserAccount
{
    [JsonConverter(typeof(SingleValueObjectConverter))]
    public class UserAccountId : Identity<UserAccountId>
    {
        public UserAccountId(string value)
            : base(value)
        {
            
        }
    }
}