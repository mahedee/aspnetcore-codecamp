using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MyCodeCamp.Data;
using MyCodeCamp.Data.Model;
using MyCodeCamp.Filters;
using MyCodeCamp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyCodeCamp.Controllers
{
    public class AuthController : Controller
    {
        private EFDataContext _context;
        private SignInManager<CampUser> _signInMgr;
        private ILogger<AuthController> _logger;

        public AuthController(EFDataContext  context, SignInManager<CampUser> signInMgr, 
            ILogger<AuthController> logger)
        {
            _context = context;
            _signInMgr = signInMgr;
            _logger = logger;
        }

        [HttpPost("api/auth/login")]
        [ValidateModel]
        public async Task<IActionResult> Login([FromBody] CredentialModel model)
        {
            try
            {
                var result = await _signInMgr.PasswordSignInAsync(model.UserName, model.Password, false, false);
                if(result.Succeeded)
                {
                    return Ok();
                }
            }
            catch (Exception exp)
            {

                _logger.LogError($"Exception thrown while login in {exp}");
            }

            return BadRequest("Failde to login");
        }
    }
}
