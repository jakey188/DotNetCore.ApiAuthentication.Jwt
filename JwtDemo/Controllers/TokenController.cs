using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using DotNetCore.Authentication.JwtBearer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace JwtDemo.Controllers
{
    [ApiController]
    public class TokenController : ControllerBase
    {
        private readonly ITokenService _builder;
        public TokenController(ITokenService builder)
        {
            _builder = builder;
        }

        [HttpGet("token")]
        public async Task<IActionResult> token(string id = "888")
        {
            var claims = new UserClaimIdentity[] 
            {
                 new UserClaimIdentity(AppConst.ClaimUserId,id,true,true),
                 new UserClaimIdentity("appid","xxxx",true),
                 new UserClaimIdentity("name","赵四")
            };

            var token = await _builder.CreateTokenAsync(claims.ToList());
            return Ok(token);
        }

        [HttpGet("refresh")]
        public async Task<IActionResult> refresh(string refreshtoken)
        {
            var token = await _builder.CreateRefreshTokenAsync(refreshtoken);

            if (token.IsError)
                return Ok(token.ErrorMessage);

            return Ok(token);
        }
    }
}
