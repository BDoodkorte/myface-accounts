using Microsoft.AspNetCore.Mvc;
using MyFace.Models.Request;
using MyFace.Models.Response;
using MyFace.Models.Database;
using MyFace.Repositories;
using System.Net.Http.Headers;
using System.Net.Http;
using System.Text;
using System;

namespace MyFace.Controllers
{
    //we are creating a path/endpoint here;
    [Route("/login")]
    public class LoginController : ControllerBase
    {
        [HttpGet("")]
        public IActionResult GetUserData()
        {
            string authHeader = HttpContext.Request.Headers["Authorization"];
            if (authHeader != null && authHeader.StartsWith("Basic"))
            {
                return Ok();
            }
            return Ok();
        }
    }
}