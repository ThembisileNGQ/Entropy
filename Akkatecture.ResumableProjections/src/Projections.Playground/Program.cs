using Domain.Model.GiftCard.Events;
using Projections.Prototype;
using System;
using System.Threading.Tasks;
using Domain.Model.GiftCard;

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

    public class GiftCardProjectionId : IProjectionId
    {
        public string Value { get; set; }
    }

    
    
    
    /*public class IssuedGiftCardsProjection : IProjection
    {
        public IProjection Id { get; set; }
        public int Count { get; set; }
    }
    
    
    public class DepletedGiftCardsProjection : IProjection
    {
        public IProjection Id { get; set; }
        public int Count { get; set; }
    }*/

    public static class IProjectionIdExtensions
    {
        public static GiftCardProjectionId From(GiftCardId id)
        {
            return new GiftCardProjectionId(){Value = id.Value};
        }
    }
}
