using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;

namespace DotNetCore.Authentication.JwtBearer
{
    public class UserClaimIdentity
    {
        public UserClaimIdentity(string type, string value, bool isCacheKey = false, bool isPrimaryKey = false)
        {
            IsCacheKey = isPrimaryKey ? true : isCacheKey;
            IsPrimaryKey = isPrimaryKey;
            Key = type;
            Value = value;
        }
        /// <summary>
        /// 是否主键
        /// </summary>
        public bool IsPrimaryKey { get; set; }
        /// <summary>
        /// 是否缓存Key
        /// </summary>
        public bool IsCacheKey { get; set; }
        /// <summary>
        /// 类型
        /// </summary>
        public string Key { get; set; }
        /// <summary>
        /// 值
        /// </summary>
        public string Value { get; set; }
    }

    public static class UserClaimIdentityExtensions
    {
        public static string GetClaimCacheKey(this List<UserClaimIdentity> claims)
        {
            var claimCache = claims.Where(c => c.IsCacheKey).OrderBy(c => c.Key).Select(c => c.Value).ToList();
            var key = string.Join(":", claimCache).ToMd5();
            return key;
        }
    }
}
