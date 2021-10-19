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
            var appId = "xxxx";

            var claims = new UserClaimIdentity[] 
            {
                 new UserClaimIdentity(AppConst.ClaimUserId,id,true,true),
                 new UserClaimIdentity("appid",appId,true),
                 new UserClaimIdentity("name","赵四")
            };

            var payload = new Dictionary<string, object>()
            {
                { "data",  new LoginDto { AppId = appId, UserId = id  } }
            };

            var token = await _builder.CreateTokenAsync(claims.ToList(), payload);
            return Ok(token);
        }

        [HttpGet("refresh")]
        public async Task<IActionResult> refresh(string refreshtoken)
        {
            var replace = new Dictionary<string, string>() 
            {
                {"appid","yyyy" }
            };

            var reToken =  await _builder.GetRefreshTokenAsync(refreshtoken);

            var json = Newtonsoft.Json.JsonConvert.SerializeObject(reToken.ClaimPayload["data"]);

            var dto = Newtonsoft.Json.JsonConvert.DeserializeObject<LoginDto>(json);

            dto.AppId = replace["appid"];

            var payload = new Dictionary<string, object>()
            {
                { "data",  dto }
            };

            var token = await _builder.CreateRefreshTokenAsync(refreshtoken, replace, payload);

            if (token.IsError)
                return Ok(token.ErrorMessage);

            return Ok(token);
        }
    }

    public class LoginDto
    {
        public string AppId { get; set; }
        public string UserId { get; set; }
    }
}
