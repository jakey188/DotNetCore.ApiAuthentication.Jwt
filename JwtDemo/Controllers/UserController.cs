using DotNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace JwtDemo.Controllers
{
    [ApiController]
    [Authorize]
    public class UserController : ControllerBase
    {
        private readonly ITokenService _tokenService;
        public UserController(ITokenService tokenService)
        {
            _tokenService = tokenService;
        }

        [HttpGet("endpoint")]
        [AllowAnonymous]
        [AllowApi]
        public async Task<IActionResult> endpoint()
        {
            var endpoint = Request.HttpContext.GetEndpoint();
            
            var end = endpoint?.Metadata.GetMetadata<IAllowAnonymous>();

            if (end != null)
            {
                return Ok(0);
            }

            return Ok(1);
        }

        [HttpGet("user/info")]
        public async Task<IActionResult> UserInfo(string type)
        {
            var uid = await Request.HttpContext.GetUserIdAsync();
            var value = await Request.HttpContext.GetClaimValueAsync(type);
            var data = Request.HttpContext.User.Claims;
            return Ok(new { uid, value, data });
        }

        [HttpGet("user/logout")]
        public async Task<IActionResult> Logout()
        {
            return Ok(await Request.HttpContext.SignOutAsync());
        }
    }
}
