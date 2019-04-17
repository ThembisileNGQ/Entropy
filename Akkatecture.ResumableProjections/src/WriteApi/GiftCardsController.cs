using System;
using System.Threading.Tasks;
using Akkatecture.Aggregates.ExecutionResults;
using Akkatecture.Akka;
using Domain.Model.GiftCard;
using Domain.Model.GiftCard.Commands;
using Microsoft.AspNetCore.Mvc;

namespace WriteApi
{
    public class CreditsController : Controller
    {
        private readonly ActorRefProvider<GiftCardManager> _aggregateManager;
        public CreditsController(
            ActorRefProvider<GiftCardManager> aggregateManager)
        {
            _aggregateManager = aggregateManager;
        }

        [HttpPost("giftcards")]
        public async Task<IActionResult> IssueGiftCard([FromBody] GiftCardInputModel model)
        {
            var aggregateId = GiftCardId.New;

            var command = new IssueCommand(aggregateId, model.Credits);
            
            var executionResult = await _aggregateManager.Ask<ExecutionResult>(command);
            
            if(executionResult.IsSuccess)
                return Accepted(new { Id = aggregateId.GetGuid() });

            return BadRequest(executionResult.ToString());
        }
        
        [HttpPut("giftcards/{id:Guid}/redeem")]
        public async Task<IActionResult>  RedeemGiftCard([FromRoute] Guid id, [FromBody] GiftCardInputModel model)
        {
            var aggregateId = GiftCardId.With(id);

            var command = new RedeemCommand(aggregateId, model.Credits);
            
            var executionResult = await _aggregateManager.Ask<ExecutionResult>(command);
            
            if(executionResult.IsSuccess)
                return Accepted(new { Id = aggregateId.GetGuid() });

            return BadRequest(executionResult.ToString());
        }
        
        [HttpPut("giftcards/{id:Guid}/cancel")]
        public async Task<IActionResult>  CancelGiftCard([FromRoute] Guid id)
        {
            var aggregateId = GiftCardId.With(id);

            var command = new CancelCommand(aggregateId);
            
            var executionResult = await _aggregateManager.Ask<ExecutionResult>(command);
            
            if(executionResult.IsSuccess)
                return Accepted(new { Id = aggregateId.GetGuid() });

            return BadRequest(executionResult.ToString());
        }
    }
}