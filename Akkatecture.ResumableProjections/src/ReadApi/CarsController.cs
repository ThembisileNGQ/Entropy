using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Akka.Actor;
using Akka.Persistence.Query;
using Akka.Streams;
using Akka.Streams.Dsl;
using Domain.Model.Car;
using Domain.Model.Car.Commands;
using Akka.Persistence.Query.Sql;
using Microsoft.AspNetCore.Mvc;

namespace ReadApi
{
    public class CarsController : Controller
    {
        private readonly ActorSystem _actorSystem;

        public CarsController(ActorSystem actorSystem)
        {
            _actorSystem = actorSystem;
        }

        [HttpGet("cars/{id:Guid}")]
        public async Task<IActionResult> GetCar([FromRoute] Guid id)
        {
            throw new NotImplementedException();
        }

        [HttpGet("rebuild")]
        public async Task<IActionResult> Rebuild()
        {
            throw new NotImplementedException();
        }


        private void Test()
        {
            
            var bidProjection = new MyResumableProjection("bid");

            var writerProps = Props.Create(typeof(TheOneWhoWritesToQueryJournal), "bid");
            var writer = _actorSystem.ActorOf(writerProps, "bid-projection-writer");
            var mat = ActorMaterializer.Create(_actorSystem);

            var readJournal =
                PersistenceQuery.Get(_actorSystem).ReadJournalFor<SqlReadJournal>("");

            readJournal
                .EventsByTag("bid", Sequence.Sequence(0L))
                .Select(e => e.Event)
                .Select()
                //.SelectAsync(8, envelope => writer.Ask(envelope.Event).ContinueWith(t => envelope.Offset,TaskContinuationOptions.OnlyOnRanToCompletion))
                //.SelectAsync(1, offset => bidProjection.SaveProgress(offset))
                //.RunWith(Sink.Ignore<object>(), mat);
        }

        public class TheOneWhoWritesToQueryJournal : ActorBase
        {

            public TheOneWhoWritesToQueryJournal(string id)
            {
            }


            protected override bool Receive(object message)
            {
                return true;
            }

        }

        public class MyResumableProjection
        {
            public int LatestOffset { get; }
            public string Id { get; }
            public Offset Offset { get; }
            public IList<object> 


            public MyResumableProjection(string id)
            {
                Id = id;
                Offset = Offset.NoOffset();
            }

            public Task SaveProgress(Offset offset)
            {
                return Task.CompletedTask;
            }
        }
    }
}
