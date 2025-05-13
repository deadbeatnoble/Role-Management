using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using RoleManagement.Data;
using RoleManagement.Dtos;
using RoleManagement.Models;
using RoleManagement.Services;

namespace RoleManagement.Controllers
{
    [Route("api/auth")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IConfiguration _config;
        private readonly Jwt _jwt;

        public AuthController(IConfiguration config, Jwt jwt)
        {
            _config = config;
            _jwt = jwt;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto dto)
        {
            var users = await SqlQuery.QueryAsync<User>(
                "SELECT Id, Username, HashedPassword FROM Users WHERE Username = @username",
                reader => new User
                {
                    Id = reader.GetInt32(0),
                    Username = reader.GetString(1),
                    HashedPassword = reader.GetString(2)
                },
                new SqlParameter("@username", dto.Username));

            var user = users.FirstOrDefault();

            if (user == null || !BCrypt.Net.BCrypt.Verify(dto.Password, user.HashedPassword))
                return Unauthorized(new { message = "Invalid username or password" });

            var token = _jwt.GenerateToken(user);
            return Ok(new { Token = token });
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto dto)
        {
            var count = await SqlQuery.QueryScalarAsync<int>(
                "SELECT COUNT(*) FROM Users WHERE Username = @username",
                new SqlParameter("@username", dto.Username));

            if (count > 0)
                return BadRequest("Username already exists");

            var hashedPassword = BCrypt.Net.BCrypt.HashPassword(dto.Password);

            var sql = "INSERT INTO Users (Username, HashedPassword) VALUES (@username, @hashedPassword)";
            await SqlQuery.ExecuteNonQueryAsync(sql,
                new SqlParameter("@username", dto.Username),
                new SqlParameter("@hashedPassword", hashedPassword));

            return Ok(new { message = "User created successfully" });
        }
    }
}
