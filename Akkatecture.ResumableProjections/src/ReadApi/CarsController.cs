using System;
using System.Threading.Tasks;
using Domain.Model.Car;
using Domain.Model.Car.Commands;
using Microsoft.AspNetCore.Mvc;

namespace ReadApi
{
    public class CarsController : Controller
    {
        [HttpGet("cars/{id:Guid}")]
        public async Task<IActionResult>  GetCar([FromRoute] Guid id)
        {
            throw new NotImplementedException();
        }
    }
}