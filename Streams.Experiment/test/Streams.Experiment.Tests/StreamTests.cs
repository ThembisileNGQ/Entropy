using System;
using System.Threading.Tasks;
using Akka.Actor;
using Akka.Streams;
using Akka.TestKit.Xunit2;
using Akka.Streams.Dsl;
using Akka.Streams.TestKit;
using Streams.Experiment.Domain;
using Streams.Experiment.Domain.Aggregates;
using Streams.Experiment.Domain.Services;
using Xunit;

namespace Streams.Experiment.Tests
{
    public class StreamTests : TestKit
    {
        [Fact]
        public async Task When_flow_is_opened_user_is_assigned_to_a_club()
        {
            var aggregateManager = Sys.ActorOf(Props.Create(() => new AggregateManager()));
            var aggregateManagerRef = new AggregateManagerRef(aggregateManager);
            var userClubAssignmentService = new UserClubAssignmentService(aggregateManagerRef,Sys);
            var flowUnderTest = userClubAssignmentService.AssignmentFlow;
            var queue = userClubAssignmentService.Queue;
            var materializer = Sys.Materializer();
            var probe = this.SinkProbe<AssignmentSummary>();
            var id = new Guid("1c041a27-0c61-414e-a002-30706b3411fd");
            var name = $"testUser";
            var email = $"usertestUser@foo.com";
            var registerUser = new RegisterUser(id,name,email,"SE","1234567890");
            var savings = 20;
            var credit = 14;
            var openAccount = new OpenBankAccount(id,savings,credit);
                        
            aggregateManager.Tell(registerUser, ActorRefs.NoSender);
            aggregateManager.Tell(openAccount, ActorRefs.NoSender);

            var assignmentCommand = new AssignUserToClubFlow
            {
                UserId = id,
                AggregateManager = aggregateManager
            };
            
            using (materializer)
            {
                var flow = queue
                    .Via(flowUnderTest)
                    .ToMaterialized(probe, Keep.Both)
                    .Run(materializer);

                var q = flow.Item1;
                var sink = flow.Item2;
                var result = await q.OfferAsync(assignmentCommand);

                sink.Request(1);
                //sink.RequestNext();
                sink.ExpectNext<AssignmentSummary>(x =>
                    x.Email == email &&
                    x.NetWorth == savings - credit);
                


            }
        }
    }
}