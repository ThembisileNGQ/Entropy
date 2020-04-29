using System;
using System.Threading.Tasks;
using Akka;
using Akka.Actor;
using Akka.Streams;
using Akka.Streams.Dsl;
using Streams.Experiment.Domain.Aggregates;

namespace Streams.Experiment.Domain.Services
{
    public class UserClubAssignmentService
    {
        public ActorSystem ActorSystem { get; }
        public IActorRef AggregateManager { get; }
        public Source<AssignUserToClubFlow, ISourceQueueWithComplete<AssignUserToClubFlow>> Queue { get; }
        public Flow<AssignUserToClubFlow,AssignmentSummary,NotUsed> AssignmentFlow { get; }
        
        
        public UserClubAssignmentService(
            AggregateManagerRef aggregateManagerRef,
            ActorSystem actorSystem)
        {
            AggregateManager = aggregateManagerRef.Ref;
            ActorSystem = actorSystem;
            
            Queue = Source.Queue<AssignUserToClubFlow>(5, OverflowStrategy.Backpressure);

            AssignmentFlow = Flow.Create<AssignUserToClubFlow>()
                .Ask<GetUserState>()
                .SelectAsync(4, async x =>
                {
                    var getUserState = new GetUserState(x.UserId);
                    var userState = await x.AggregateManager.Ask<UserState>(getUserState, TimeSpan.FromMilliseconds(500));
                    x.UserState = userState;

                    return x;
                })
                .SelectAsync(4, async x =>
                {
                    var getBankAccountState = new GetBankAccountState(x.UserId);
                    var bankAccountState = await x.AggregateManager.Ask<BankAccountState>(getBankAccountState, TimeSpan.FromMilliseconds(500));
                    x.BankAccountState = bankAccountState;

                    return x;
                })
                .Select(x =>
                {
                    var networth = x.BankAccountState.Savings - x.BankAccountState.Credit;

                    AddMember addMemberCommand = null;
                    
                    if(networth < 4)
                        addMemberCommand = new AddGoldMember(x.UserId);
                    else if(networth < 6)
                        addMemberCommand = new AddPlatinumMember(x.UserId);
                    else
                        addMemberCommand = new AddBlackMember(x.UserId);

                    AggregateManager.Tell(addMemberCommand);
                    
                    return new AssignmentSummary
                    {
                        UserId =  x.UserId,
                        Name = x.UserState.Name,
                        Email = x.UserState.Email,
                        NetWorth =  networth
                    };
                });
        }
        
        public async Task<AssignmentSummary> Assign(Guid userId)
        {
            var assignmentCommand = new AssignUserToClubFlow
            {
                UserId = userId,
                AggregateManager = AggregateManager
            };

            using (var mat = ActorSystem.Materializer())
            {
                var a = Queue
                    .Via(AssignmentFlow)
                    .ToMaterialized(Sink.First<AssignmentSummary>(), Keep.Left)
                    .Run(mat);
                
                
                var result = await a.OfferAsync(assignmentCommand);

            }
                
            return new AssignmentSummary();
        }
        
    }
    

    public class AssignmentSummary
    {
        public Guid UserId { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public int NetWorth { get; set; }
    }
    
    public class AssignUserToClubFlow
    {
        public Guid UserId { get; set; }
        public UserState UserState { get; set; }
        public BankAccountState BankAccountState { get; set; }
        public IActorRef AggregateManager { get; set; }
    }
    
}