using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MyCodeCamp.Data;

namespace MyCodeCamp.Controllers
{
    [Route("api/[controller]")]
    public class CampsController : Controller
    {
        private ICampRepository _repo;

        public CampsController(ICampRepository repo)
        {
            _repo = repo;
        }
        //[HttpGet("api/camps")]
        [HttpGet("")]
        public IActionResult Get()
        {
            var camps = _repo.GetAllCamps();
            return Ok(camps);
        }
    }
}