using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace MyCodeCamp.Controllers
{
    [Route("api/[controller]")]
    public class CampsController : Controller
    {
        //[HttpGet("api/camps")]
        [HttpGet("")]
        public IActionResult Get()
        {
            return Ok(new { Name = "Mahedee", FavouriteColor = "Violet" });
        }
    }
}