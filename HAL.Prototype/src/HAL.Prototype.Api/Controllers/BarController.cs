using Halcyon.HAL;
using Halcyon.Web.HAL;
using Microsoft.AspNetCore.Mvc;

namespace HAL.Prototype.Api.Controllers
{
    public class BarController : Controller 
    {
        [HttpGet("bar/{id:int}")]
        public IActionResult GetById(int id)
        {
            var barModel = new {
                id = id,
                type = "bar",
                does = $"Bar {id}"
            };
            
            return this.HAL(barModel, new Link[]
            {
                new Link("self", "bar/{id}")
            });
        }
    }
}