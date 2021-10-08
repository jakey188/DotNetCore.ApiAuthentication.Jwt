using DotNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
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

        [HttpGet("user/info")]
        public IActionResult UserInfo()
        {
            var data = Request.HttpContext.User.Claims;
            return Ok(data);
        }

        [HttpGet("user/logout")]
        public async Task<IActionResult> Logout()
        {
            return Ok(await Request.HttpContext.SignOut());
        }
    }
}
