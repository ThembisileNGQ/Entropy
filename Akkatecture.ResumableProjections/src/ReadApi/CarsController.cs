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

        [HttpGet("cars/{id:Guid}")]
        public async Task<IActionResult> GetCar([FromRoute] Guid id)
        {
            throw new NotImplementedException();
        }
    }
}
