using System;
using System.Collections.Generic;
using System.Text;

namespace DotNetCore.Authentication.JwtBearer.Responses
{
    public class ErrorResponse : Response
    {
        public ErrorResponse(string message) : base(true, message)
        {
        }
    }
}
