using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Akka.Actor;
using Akka.Persistence.Query;
using Akka.Streams;
using Akka.Streams.Dsl;
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
        public IActionResult Rebuild()
        {
            Test();
            return Ok();
        }


        private void Test()
        {   
            var bidProjection = new MyResumableProjection("bid");

            var mat = ActorMaterializer.Create(_actorSystem);

            var readJournal =
                PersistenceQuery.Get(_actorSystem)
            
                    .ReadJournalFor<SqlReadJournal>(SqlReadJournal.Identifier);

            //note - readmodels will use tags to locate aggregates
            
            readJournal
                .EventsByTag("CarAggregate", Sequence.Sequence(0L))
                .Select(e => e.Event)
                .Select(bidProjection.Save)
                .RunForeach(o => bidProjection.Save(o), mat)
                .Wait();
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
            public IList<object> Events { get; }


            public MyResumableProjection(string id)
            {
                Id = id;
                Events = new List<object>();
                Offset = Offset.NoOffset();
            }

            public object Save(object o)
            {
                Console.WriteLine(o.ToString());
                Events.Add(o);
                return o;
            }

            public Task SaveProgress(Offset offset)
            {
                return Task.CompletedTask;
            }
        }
    }
}
