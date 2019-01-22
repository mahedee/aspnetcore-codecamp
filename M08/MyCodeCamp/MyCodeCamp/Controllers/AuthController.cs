using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using MyCodeCamp.Data;
using MyCodeCamp.Data.Model;
using MyCodeCamp.Filters;
using MyCodeCamp.Models;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace MyCodeCamp.Controllers
{
    public class AuthController : Controller
    {
        private EFDataContext _context;
        private SignInManager<CampUser> _signInMgr;
        private ILogger<AuthController> _logger;
        private UserManager<CampUser> _userMgr;
        private IPasswordHasher<CampUser> _hasher;
        //private IConfigurationRoot _config;
        private IConfiguration _config;
        public AuthController(EFDataContext context,
            SignInManager<CampUser> signInMgr,
            UserManager<CampUser> userMgr,
            IPasswordHasher<CampUser> hasher,
            ILogger<AuthController> logger
            , IConfiguration config
            //,IConfigurationRoot config
            )
        {
            _context = context;
            _signInMgr = signInMgr;
            _logger = logger;
            _userMgr = userMgr;
            _hasher = hasher;
            _config = config;
        }

        [HttpPost("api/auth/login")]
        [ValidateModel]
        public async Task<IActionResult> Login([FromBody] CredentialModel model)
        {
            try
            {
                var result = await _signInMgr.PasswordSignInAsync(model.UserName, model.Password, false, false);
                if (result.Succeeded)
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


        [HttpPost("api/auth/token")]
        [ValidateModel]
        public async Task<IActionResult> CreateToken([FromBody] CredentialModel model)
        {
            try
            {
                var user = await _userMgr.FindByNameAsync(model.UserName);

                if (user != null)
                {
                    var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Tokens:Key"]));

                    var userClaims = await _userMgr.GetClaimsAsync(user);
                    var claims = new[]
                        {
                        new Claim(JwtRegisteredClaimNames.Sub,model.UserName),
                        new Claim(JwtRegisteredClaimNames.Jti,Guid.NewGuid().ToString()),
                        new Claim(JwtRegisteredClaimNames.GivenName, user.FirstName),
                        new Claim(JwtRegisteredClaimNames.FamilyName, user.LastName),
                        new Claim(JwtRegisteredClaimNames.Email, user.Email)
                    }.Union(userClaims); ;
                    var token = new JwtSecurityToken(
                            issuer: _config["Tokens:Issuer"],
                            audience: _config["Tokens:Audience"],
                            expires: DateTime.Now.AddHours(1),
                            claims: claims,
                            signingCredentials: new Microsoft.IdentityModel.Tokens.SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256)
                            );
                    return Ok(new
                    {
                        token = new JwtSecurityTokenHandler().WriteToken(token),
                        expiration = token.ValidTo,
                        message = "Success!!"
                    });
                }
            }
            catch (Exception exp)
            {

                _logger.LogError($"Exception throwing when creating JWT {exp}");
            }

            return BadRequest("Failed to generate token");
        }
    }
}
