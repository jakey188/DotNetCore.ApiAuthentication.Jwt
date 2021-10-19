using System;
using System.Collections.Generic;
using System.Text;

namespace DotNetCore.Authentication.JwtBearer
{
    public class ClaimPayload
    {
        public string Key { get; set; }
        public object Value { get; set; }
    }
}
