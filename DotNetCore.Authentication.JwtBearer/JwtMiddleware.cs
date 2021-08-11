//using Microsoft.AspNetCore.Http;
//using Microsoft.Extensions.Options;
//using Microsoft.IdentityModel.Tokens;
//using System;
//using System.IdentityModel.Tokens.Jwt;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace DotNetCore.Authentication.JwtBearer
//{
//    public class JwtMiddleware
//    {
//        private readonly RequestDelegate _next;
//        private readonly JwtOptions _options;

//        public JwtMiddleware(RequestDelegate next,
//            IOptions<JwtOptions> options)
//        {
//            _next = next;
//            _options = options.Value;
//        }

//        public async Task Invoke(HttpContext context)
//        {
//            var token = context.Request.Headers["Authorization"].FirstOrDefault()?.Replace("Bearer ", "");

//            if (token != null)
//                ValidateToken(context, token);

//            await _next(context);
//        }

//        private void ValidateToken(HttpContext context, string token)
//        {
//            try
//            {
//                var tokenHandler = new JwtSecurityTokenHandler();
//                var key = Encoding.ASCII.GetBytes(_options.SignKey);
//                tokenHandler.ValidateToken(token, new TokenValidationParameters
//                {
//                    ValidateIssuerSigningKey = true,
//                    IssuerSigningKey = new SymmetricSecurityKey(key),
//                    ValidateIssuer = true,
//                    ValidateAudience = false,
//                    // set clockskew to zero so tokens expire exactly at token expiration time (instead of 5 minutes later)
//                    ClockSkew = TimeSpan.Zero
//                }, out SecurityToken validatedToken);

//                var jwtToken = (JwtSecurityToken)validatedToken;
//                var userId = int.Parse(jwtToken.Claims.First(x => x.Type == "id").Value);
//            }
//            catch
//            {
   
//            }
//        }
//    }
//}
