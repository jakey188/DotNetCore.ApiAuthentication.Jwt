using System;
using System.Collections.Generic;
using System.Text;

namespace DotNetCore.Authentication.JwtBearer
{
    public class AppConst
    {
        /// <summary>
        /// 用户id
        /// </summary>
        public const string ClaimUserId = "id";
        /// <summary>
        /// RefreshToken
        /// </summary>
        public const string ClaimRefreshToken = "token";
        /// <summary>
        /// AccessToken缓存Key
        /// </summary>
        public const string ClaimAccessTokenCacheKey = "key";
    }
}
