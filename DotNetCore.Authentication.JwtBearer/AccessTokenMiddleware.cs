using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotNetCore.Authentication.JwtBearer
{
    public class AccessTokenMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly JwtOptions _options;
        private readonly ITokenService _tokenService;

        public AccessTokenMiddleware(RequestDelegate next,
            IOptions<JwtOptions> options,
            ITokenService tokenService)
        {
            _next = next;
            _options = options.Value;
            _tokenService = tokenService;
        }

        public async Task Invoke(HttpContext context)
        {
            if (!context.User.Identity.IsAuthenticated)
            {
                await _next(context);
                return;
            }

            var inputToken = await context.GetAccessTokenAsync();

            var userId = await context.GetUserIdAsync();

            var token = await _tokenService.GetAccessTokenAsync(userId);

            if (token == null)
            {
                await WriteAsync(context, "用户信息已失效或未登录");
                return;
            }

            if (DateTime.UtcNow > token.Expiration)
            {
                await WriteAsync(context, "用户信息已过期");
                return;
            }

            if (!token.Token.Equals(inputToken, StringComparison.OrdinalIgnoreCase))
            {
                await WriteAsync(context, "用户信息已失效");
                return;
            }

            if (!token.Token.Equals(inputToken, StringComparison.OrdinalIgnoreCase))
            {
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
