using DotNetCore.Authentication.JwtBearer.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DotNetCore.Authentication.JwtBearer
{
    public interface ITokenStore
    {
        /// <summary>
        /// 获取Token
        /// </summary>
        /// <param name="refreshToken"></param>
        /// <returns></returns>
        Task<RefreshToken> GetTokenAsync(string refreshToken);

        /// <summary>
        /// 存储
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        Task<bool> AddAsync(RefreshToken token);

        Task<bool> UpdateAsync(RefreshToken token);
    }
}
