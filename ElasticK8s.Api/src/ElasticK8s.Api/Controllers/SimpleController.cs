using System;
using Microsoft.AspNetCore.Mvc;

namespace ElasticK8s.Api.Controllers
{
    public class SimpleController : Controller
    {

        [HttpGet("simple")]
        public IActionResult Get()
        {
            return Ok(new {date = DateTime.UtcNow});
        }
    }
}