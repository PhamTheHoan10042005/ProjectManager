using CommentNotifyService.Data;
using CommentNotifyService.DTOs;
using CommentNotifyService.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace CommentNotifyService.Controllers
{
    [Route("api/auth")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IConfiguration _configuration;

        public AuthController(AppDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto dto)
        {
            if (await _context.Users.AnyAsync(u => u.Username == dto.Email))
            {
                return BadRequest(new { message = "Tài khoản đã tồn tại trong hệ thống!" });
            }

            var user = new User
            {
                Username = dto.Email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password),
                FullName = dto.FullName,
                Role = "Viewer"
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return Ok(BuildAuthResponse(user));
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto dto)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == dto.Email);
            if (user == null || !BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash))
            {
                return Unauthorized(new { message = "Tài khoản hoặc mật khẩu không chính xác!" });
            }

            return Ok(BuildAuthResponse(user));
        }

        [HttpGet("me")]
        [Authorize]
        public async Task<IActionResult> Me()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
            {
                return Unauthorized(new { message = "Token không hợp lệ!" });
            }

            var user = await _context.Users.FindAsync(userId);
            if (user == null)
            {
                return NotFound(new { message = "Không tìm thấy người dùng!" });
            }

            return Ok(new
            {
                id = user.Id,
                email = user.Username,
                fullName = user.FullName,
                role = user.Role
            });
        }

        private object BuildAuthResponse(User user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(GetJwtKey());
            var issuer = _configuration["Jwt:Issuer"] ?? "ProjectManagementSystem";
            var audience = _configuration["Jwt:Audience"] ?? "ProjectManagementSystem";

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                    new Claim(ClaimTypes.Name, user.Username),
                    new Claim(ClaimTypes.Role, user.Role)
                }),
                Expires = DateTime.UtcNow.AddDays(7),
                Issuer = issuer,
                Audience = audience,
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(key),
                    SecurityAlgorithms.HmacSha256Signature)
            };

            var tokenString = tokenHandler.WriteToken(tokenHandler.CreateToken(tokenDescriptor));

            return new
            {
                token = tokenString,
                user = new
                {
                    id = user.Id,
                    email = user.Username,
                    fullName = user.FullName,
                    role = user.Role
                }
            };
        }

        private string GetJwtKey()
        {
            return _configuration["Jwt:Key"]
                ?? _configuration["Jwt:Secret"]
                ?? "ProjectManagementSystem-JWT-Secret-Key-2024-Min32Chars!";
        }
    }
}
