﻿using Microsoft.AspNetCore.Mvc;
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

            var payload = new List<ClaimPayload>() { new ClaimPayload { Key = "data", Value = new { appId = "1", userId = "22" } } };

            var token = await _builder.CreateTokenAsync(claims.ToList(), payload);
            return Ok(token);
        }

        [HttpGet("refresh")]
        public async Task<IActionResult> refresh(string refreshtoken)
        {
            var dic = new Dictionary<string, string>() 
            {
                {"appid","yyyy" }
            };
            var token = await _builder.CreateRefreshTokenAsync(refreshtoken, dic);

            if (token.IsError)
                return Ok(token.ErrorMessage);

            return Ok(token);
        }
    }
}
