using System;
using System.Threading.Tasks;
using Akkatecture.Aggregates.ExecutionResults;
using Akkatecture.Akka;
using Domain.Model.Car;
using Domain.Model.Car.Commands;
using Microsoft.AspNetCore.Mvc;

namespace WriteApi
{
    public class CarsController : Controller
    {
        private readonly ActorRefProvider<CarAggregateManager> _aggregateManager;
        public CarsController(
            ActorRefProvider<CarAggregateManager> aggregateManager)
        {
            _aggregateManager = aggregateManager;
        }

        [HttpPost("cars")]
        public async Task<IActionResult> PostCar()
        {
            var aggregateId = CarId.New;

            var command = new CreateCarCommand(aggregateId);
            
            var executionResult = await _aggregateManager.Ask<ExecutionResult>(command);
            
            if(executionResult.IsSuccess)
                return Accepted(new {Id = aggregateId});

            return BadRequest(executionResult.ToString());
        }
        
        [HttpPatch("cars/{id:Guid}/name")]
        public async Task<IActionResult>  PatchCar([FromRoute] Guid id, [FromBody] CarInputModel model)
        {
            var aggregateId = CarId.With(id);

            var command = new ChangeCarNameCommand(aggregateId, model.Name);
            
            var executionResult = await _aggregateManager.Ask<ExecutionResult>(command);
            
            if(executionResult.IsSuccess)
                return Accepted(new {Id = aggregateId});

            return BadRequest(executionResult.ToString());
        }
    }
}