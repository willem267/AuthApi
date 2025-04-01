using AuthApi.DTO.Login;
using AuthApi.Repositories.IRepositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AuthApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IJwtRepository _jwtRepository;
        private readonly IUserRepository _userRepository;
        public AuthController(IJwtRepository jwtRepository, IUserRepository userRepository)
        {
            _jwtRepository = jwtRepository;
            _userRepository = userRepository;
        }

        //LOGIN
        //POST: /api/Auth/login
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequestDto request)
        {
            //check login 
            var user = await _userRepository.getUserByUsername(request.Username);
            if (user !=null)
            {
                string passwordCheck = user.Password;
                if (passwordCheck == request.Password)
                {
                //generate access token
                    string accessToken = await _jwtRepository.generateJwtKey(user.Username);
                    var response = new LoginResponseDto
                    {
                        Username = user.Username,
                        Jwt = accessToken
                    };
                    var cookieOptions = new CookieOptions
                    {
                        HttpOnly = true,   // Chỉ gửi qua HTTP, không thể truy cập bằng JavaScript
                        Secure = true,    
                        SameSite = SameSiteMode.None,
                        //Domain = "localhost",
                        Expires = DateTime.Now.AddMinutes(30)
                    };
                    Response.Cookies.Append("jwt", accessToken, cookieOptions);
                    return Ok(response);    
                }
            }

              return BadRequest("Username or password is wrong");
          
        }
        //LOGOUT
        //POST: /api/Auth/logout
        [Authorize]
        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            if (Request.Cookies["jwt"] != null)
            {
                Response.Cookies.Delete("jwt"); // Xóa cookie chứa JWT
                return Ok(new { message = "Logged out successfully" });
            }

            return Unauthorized(new { message = "User not logged in" });
        }

    }
}
