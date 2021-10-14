using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DotNetCore.Authentication.JwtBearer
{
    public class DefaultAuthorizationFilter : IAuthorizationFilter
    {
        public async Task<bool> AllowAsync(HttpContext context)
        {
            return await Task.FromResult(true);
        }
    }
}
