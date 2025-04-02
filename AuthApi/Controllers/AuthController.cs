using AuthApi.DTO.Login;
using AuthApi.DTO.Token;
using AuthApi.Repositories.IRepositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace AuthApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IJwtRepository _jwtRepository;
        private readonly IUserRepository _userRepository;
        private readonly IConfiguration _configuration; 
        public AuthController(IJwtRepository jwtRepository, IUserRepository userRepository, IConfiguration configuration)
        {
            _jwtRepository = jwtRepository;
            _userRepository = userRepository;
            _configuration = configuration;
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
                        Expires = DateTime.Now.AddMinutes(60)
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

        //VALIDATE TOKEN 
        //POST: /api/Auth/validate
        [HttpPost("validate")]
        public IActionResult ValidateToken([FromBody] TokenRequestDto request)
        {
            if (string.IsNullOrEmpty(request.Token))
            {
                return BadRequest(new { message = "Token is required" });
            }
            Console.WriteLine(request.Token);
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]);
            try
            {
                var principal = tokenHandler.ValidateToken(request.Token, new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero
                }, out SecurityToken validatedToken);

                var jwtToken = (JwtSecurityToken)validatedToken;
                var username = jwtToken.Claims.First(x => x.Type == ClaimTypes.Name).Value;
                var role = jwtToken.Claims.First(x => x.Type == ClaimTypes.Role).Value;

                return Ok(new { username, role });
            }
            catch
            {
                return Unauthorized(new { message = "Invalid token" });
            }
        }



    }
}
