using System;
using System.Collections.Generic;
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
            Type = type;
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
        public string Type { get; set; }
        /// <summary>
        /// 值
        /// </summary>
        public string Value { get; set; }
    }
}
