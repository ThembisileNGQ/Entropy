using Akkatecture.Aggregates;
using Akkatecture.Commands;

namespace SimpleDomain.Model.UserAccount
{
    public class UserAccountAggregateManager : AggregateManager<UserAccountAggregate,UserAccountId, Command<UserAccountAggregate, UserAccountId>>
    {
    }
}