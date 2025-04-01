using AuthApi.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AuthApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly ApplicationDbContext _db;
        public UserController(ApplicationDbContext db)
        {
                _db = db;   
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var list = await _db.Users.ToListAsync();
            return Ok(list);
        }


    }
}
