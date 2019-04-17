using System;
using System.Threading.Tasks;
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
