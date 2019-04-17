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
    public class GiftCardsController : Controller
    {

        [HttpGet("giftcards/{id:Guid}")]
        public async Task<IActionResult> GetGiftCard([FromRoute] Guid id)
        {
            throw new NotImplementedException();
        }
    }
}
