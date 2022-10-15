using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Aunth.Controllers
{
    [ApiController]
    [Route("test")]
    public class TestController : Controller
    {
        [HttpGet("admin")]
        [Authorize(Roles = RoleNames.Administrator)]
        public IActionResult GetAdmin()
        {
            return Ok("Страница администратора");
        }
        
        [HttpGet("moderator")]
        [Authorize(Roles = RoleNames.Moderator)]
        public IActionResult GetModerator()
        {
            return Ok("Страница модератора");
        }
        
        [HttpGet("user")]
        [Authorize(Roles = RoleNames.User)]
        public IActionResult GetUser()
        {
            return Ok("Страница пользователя");
        }
        
        [HttpGet("worker")]
        [Authorize(Roles = RoleNames.Worker)]
        public IActionResult GetWorker()
        {
            return Ok("Страница сотрудника");
        }
    }
}