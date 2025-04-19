using BlogASP.API.Models;
using BlogASP.API.Repository.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BlogASP.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RoleController : ControllerBase
    {
        private readonly IRoleRepository _roleRepository;

        public RoleController(IRoleRepository roleRepository)
        {
            _roleRepository = roleRepository;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllRoles()
        {
            var roles = await _roleRepository.GetAllAsync();
            return Ok(roles);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetRoleById(string id)
        {
            var role = await _roleRepository.GetRoleByIdAsync(id);
            if (role == null)
            {
                return NotFound("Role not found.");
            }

            return Ok(role);
        }

        [HttpPost]
        public async Task<IActionResult> CreateRole([FromBody] Role role)
        {
            if (role == null)
            {
                return BadRequest("Role data is invalid.");
            }

            role = await _roleRepository.CreateAndReturnEntityAsync(role);
            return CreatedAtAction(nameof(GetRoleById), new { id = role.RoleId }, role);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateRole(string id, [FromBody] Role role)
        {
            if (role == null)
            {
                return BadRequest("Role data is invalid.");
            }

            var existingRole = await _roleRepository.GetByIdAsync(id);
            if (existingRole == null)
            {
                return NotFound("Role not found.");
            }

            await _roleRepository.UpdateAsync(id, role);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteRole(string id)
        {
            var role = await _roleRepository.GetByIdAsync(id);
            if (role == null)
            {
                return NotFound("Role not found.");
            }

            await _roleRepository.DeleteAsync(id);
            return NoContent(); 
        }
    }
}
