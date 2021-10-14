using System;
using System.Collections.Generic;
using System.Text;

namespace DotNetCore.Authentication.JwtBearer.Responses
{
    public class OkResponse : Response
    {
        public OkResponse() : base(false, "")
        { 

        }
    }
}
