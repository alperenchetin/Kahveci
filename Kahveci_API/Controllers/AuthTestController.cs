using Kahveci_API.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Kahveci_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthTestController : ControllerBase
    {
        [HttpGet,Authorize]
        public async Task<ActionResult<string>> GetSmt()
        {
            return "authenticated";
        }
        [HttpGet("{id:int}"),Authorize(Roles = SD.Role_Admin)]
        public async Task<ActionResult<string>> Getsmt(int smth)
        {
            return "authorized admin";
        }
    }
}
