using Domain.Model.GiftCard.Events;
using Projections.Prototype;
using System;
using System.Threading.Tasks;

namespace Projections.Playground
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var mapBuilder = new EventMapBuilder<TestContext>();

            mapBuilder
                .Map<RedeemedEvent>()
                .When((evt, context) =>
                {
                    Console.WriteLine("in conditional");
                    return Task.FromResult(evt.Credits > 0);
                })
                .As((evt, context) => 
                {
                    Console.WriteLine("in as");
                    return Task.CompletedTask;
                });

            var projectorMap = new ProjectorMap<TestContext>
            {
                Custom = (context, projector) =>
                {
                    Console.WriteLine("in projector");

                    return projector();
                }
            };

            var map = mapBuilder.Build(projectorMap);

            var a = await map.Handle(new RedeemedEvent(5), new TestContext());

            Console.ReadLine();
            
        }

        public static async Task PlayThing()
        {
            var mapBuilder = new EventMapBuilder<TestContext>();

            mapBuilder
                .Map<RedeemedEvent>()
                .When((evt, context) =>
                {
                    Console.WriteLine("in conditional");
                    return Task.FromResult(evt.Credits > 0);
                })
                .As((evt, context) => 
                {
                    Console.WriteLine("in as");
                    return Task.CompletedTask;
                });

            var projectorMap = new ProjectorMap<TestContext>
            {
                Custom = (context, projector) =>
                {
                    Console.WriteLine("in projector");

                    return projector();
                }
            };

            var map = mapBuilder.Build(projectorMap);

            var a = await map.Handle(new RedeemedEvent(5), new TestContext());

            Console.ReadLine();
        }
    }

    public class TestContext : ProjectionContext
    {

    }

    public class GiftCardProjection
    {
        public DateTime Issued { get; set; }
        public int Credits { get; set; }
        public bool IsCancelled { get; set; }
    }
    
    
    public class IssuedGiftCardsProjection
    {
        public int Count { get; set; }
    }
    
    
    public class DepletedGiftCardsProjection
    {
        public int Count { get; set; }
    }
}
