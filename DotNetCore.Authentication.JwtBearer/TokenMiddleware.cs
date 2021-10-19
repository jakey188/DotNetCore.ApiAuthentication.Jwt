using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;

namespace DotNetCore.Authentication.JwtBearer
{
    public class TokenMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly JwtOptions _options;
        private readonly ITokenService _tokenService;
        private readonly IAuthorizationFilter _authorizationFilter;
        private readonly ILogger _logger;

        public TokenMiddleware(RequestDelegate next,
            IOptions<JwtOptions> options,
            ITokenService tokenService,
            IAuthorizationFilter authorizationFilter,
            ILogger<TokenMiddleware> logger)
        {
            _next = next;
            _options = options.Value;
            _tokenService = tokenService;
            _authorizationFilter = authorizationFilter;
            _logger = logger;
        }

        public async Task Invoke(HttpContext context)
        {
            if (!context.User.Identity.IsAuthenticated)
            {
                _logger.LogDebug("用户未登录");
                await _next(context);
                return;
            }

            var authorizationFilterResult = await _authorizationFilter.AllowAsync(context);

            if (authorizationFilterResult)
            {
                _logger.LogDebug("跳过登录验证");
                await _next(context);
                return;
            }

            var token = await _tokenService.GetAccessTokenAsync();

            if (token == null)
            {
                _logger.LogDebug("用户信息已失效或未登录");
                await WriteAsync(context, "用户信息已失效或未登录");
                return;
            }

            if (DateTime.UtcNow > token.Expiration)
            {
                _logger.LogDebug("用户信息已过期");
                await WriteAsync(context, "用户信息已过期");
                return;
            }

            var _token = await context.GetAccessTokenAsync();

            if (!token.Token.Equals(_token, StringComparison.OrdinalIgnoreCase))
            {
                _logger.LogDebug("用户信息已失效");
                await WriteAsync(context, "用户信息已失效");
                return;
            }

            #region Token的合法性由UseAuthorization(AuthorizationMiddleware)中间件验证
            //try
            //{
            //    var tokenHandler = new JwtSecurityTokenHandler();
            //    var key = Encoding.ASCII.GetBytes(_options.SignKey);
            //    tokenHandler.ValidateToken(token, new TokenValidationParameters
            //    {
            //        ValidateIssuerSigningKey = true,
            //        IssuerSigningKey = new SymmetricSecurityKey(key),
            //        ValidateIssuer = true,
            //        ValidateAudience = false,
            //        // set clockskew to zero so tokens expire exactly at token expiration time (instead of 5 minutes later)
            //        ClockSkew = TimeSpan.Zero
            //    }, out SecurityToken validatedToken);

            //    var jwtToken = (JwtSecurityToken)validatedToken;
            //    var userId = int.Parse(jwtToken.Claims.First(x => x.Type == "id").Value);
            //}
            //catch
            //{

            //}
            #endregion

            await _next(context);
        }

        async Task WriteAsync(HttpContext context, string message)
        {
            context.Response.ContentType = "application/json;charset=utf-8";
            context.Response.StatusCode = 401;
            await context.Response.WriteAsync(message);
        }
    }
}
