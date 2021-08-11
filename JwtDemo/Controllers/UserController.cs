using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace JwtDemo.Controllers
{
    [ApiController]
    [Authorize]
    public class UserController : ControllerBase
    {

        public UserController()
        {
        }

        [HttpGet("user/info")]
        public IActionResult UserInfo()
        {
            var user = Request.HttpContext.User;
            return Ok(user);
        }
    }
}
