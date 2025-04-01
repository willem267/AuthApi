using System.ComponentModel.DataAnnotations;

namespace AuthApi.DTO.Login
{
    public class LoginResponseDto
    {
        
        public string Username { get; set; }
        public string Jwt { get; set; }
    }
}
