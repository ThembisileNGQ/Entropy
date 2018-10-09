using System.Collections.Generic;
using Halcyon.HAL;
using Halcyon.Web.HAL;
using Microsoft.AspNetCore.Mvc;

namespace HAL.Prototype.Api.Controllers
{
    public class FooController : Controller 
    {
        [HttpGet("foo/{id:int}")]
        public IActionResult GetById(int id)
        {
            var fooModel = new {
                id = id,
                type = "foo"
            };
            
            // Return a simple resource with links to related resources
            return this.HAL(fooModel, new Link[]
            {
                new Link("self", "foo/{id}"),
                new Link("foo:bar", "foo/{id}/bars")
            });
        }
        
        [HttpGet("foo/{fooId:int}/bars")]
        public IActionResult GetBar(int fooId)
        {
            // A collection of bars related to foo
            var bars = new List<object> {
                new { id = 1, fooId = fooId, type = "bar" },
                new { id = 2, fooId = fooId, type = "bar" }
            };
    
            // data about the bars related to foo
            var fooBarModel = new {
                fooId = fooId,
                count = bars.Count
            };
            
            // Return a fooBar resource with embedded bars
            return this.HAL(
                fooBarModel,
                new Link[] {
                    new Link("self", "foo/{fooId}/bars")
                },
                "bars",
                bars,
                new Link[]
                {
                    new Link("self", "bar/{id}")
                }
            );
        }
    }
}