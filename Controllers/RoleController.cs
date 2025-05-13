using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using RoleManagement.Data;
using RoleManagement.Models;

namespace RoleManagement.Controllers
{
    [Route("api/role")]
    [ApiController]
    [Authorize]
    public class RoleController : ControllerBase
    {
        private readonly IConfiguration _config;
        public RoleController(IConfiguration config) { _config = config; }

        [HttpGet("getAll")]
        public async Task<IActionResult> GetAll()
        {
            var sql = "SELECT Id, Name FROM Roles";
            var roles = await SqlQuery.QueryAsync<Role>(sql, reader =>
                new Role
                {
                    Id = reader.GetInt32(0),
                    Name = reader.GetString(1)
                });
            return Ok(roles);
        }

        [HttpPost("create")]
        public async Task<IActionResult> Create(Role role)
        {
            var sql = "INSERT INTO Roles (Name) VALUES (@name)";
            await SqlQuery.ExecuteNonQueryAsync(sql,
                new SqlParameter("@name", role.Name));

            return CreatedAtAction(nameof(GetAll), new { id = role.Id }, role);
        }

        [HttpPut("update/{id}")]
        public async Task<IActionResult> Update(int id, Role updatedRole)
        {
            var sql = "UPDATE Roles SET Name = @name WHERE Id = @id";
            await SqlQuery.ExecuteNonQueryAsync(sql,
                new SqlParameter("@name", updatedRole.Name),
                new SqlParameter("@id", id));

            return NoContent();
        }

        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var sql = "DELETE FROM Roles WHERE Id = @id";
            await SqlQuery.ExecuteNonQueryAsync(sql,
                new SqlParameter("@id", id));

            return NoContent();
        }
    }
}
