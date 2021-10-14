using System;
using System.Collections.Generic;
using System.Text;

namespace DotNetCore.Authentication.JwtBearer
{
    public class AppConst
    {
        public const string ClaimUserId = "uid";
        public const string ClaimCachePrefix = "cache-";
        public const string ClaimRefreshToken = "key";
    }
}
