using Common;
using Microsoft.AspNetCore.Mvc;
using RealWorldNew.DAL.Entities;

namespace RealWorldNewAPI.Controllers
{
    [Route("api/test")]
    [ApiController]
    public class RealWorldController : ControllerBase
    {
        private IRealWorldService _realWorldService;

        public RealWorldController(IRealWorldService realWorldService)
        {
            _realWorldService = realWorldService;
        }

        [HttpGet]
        public ActionResult<IEnumerable<User>> GetAllUsers()
        {
            var result = _realWorldService.GetAllUsers();
            return Ok(result);
        }
    }
}
