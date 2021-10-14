using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DotNetCore.Authentication.JwtBearer
{
    public interface IAuthorizationFilter
    {
        /// <summary>
        /// 允许跳过授权检测
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        Task<bool> AllowAsync(HttpContext context);
    }
}
