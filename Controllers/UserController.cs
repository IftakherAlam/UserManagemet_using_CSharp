using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using usermangement.Data;
using usermangement.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace usermangement.Controllers
{
    [Route("api/users")]
    [ApiController]
    [Authorize]  // 🔐 Only authenticated users can access
    public class UserController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public UserController(ApplicationDbContext context)
        {
            _context = context;
        }

        // 📌 GET all users (sorted by last login time)
        [HttpGet]
        public async Task<IActionResult> GetUsers()
        {
            var users = await _context.Users
                .OrderByDescending(u => u.LastLoginTime)
                .Select(u => new
                {
                    u.Id,
                    u.Name,
                    u.Email,
                    u.LastLoginTime,
                    u.IsBlocked,
                    u.CreatedAt // Added CreatedAt field for more data visibility
                })
                .ToListAsync();

            return Ok(users);
        }

        // 📌 PUT to block multiple users
        [HttpPut("block")]
        public async Task<IActionResult> BlockUsers([FromBody] List<Guid> userIds)
        {
            if (userIds == null || userIds.Count == 0)
            {
                return BadRequest("No users selected to block.");
            }

            var users = await _context.Users.Where(u => userIds.Contains(u.Id)).ToListAsync();
            if (users.Count == 0)
            {
                return NotFound("No users found for the given IDs.");
            }

            users.ForEach(u => u.IsBlocked = true);
            await _context.SaveChangesAsync();

            return Ok("Users blocked.");
        }

        // 📌 PUT to unblock multiple users
        [HttpPut("unblock")]
        public async Task<IActionResult> UnblockUsers([FromBody] List<Guid> userIds)
        {
            if (userIds == null || userIds.Count == 0)
            {
                return BadRequest("No users selected to unblock.");
            }

            var users = await _context.Users.Where(u => userIds.Contains(u.Id)).ToListAsync();
            if (users.Count == 0)
            {
                return NotFound("No users found for the given IDs.");
            }

            users.ForEach(u => u.IsBlocked = false);
            await _context.SaveChangesAsync();

            return Ok("Users unblocked.");
        }

        // 📌 DELETE to remove multiple users
        [HttpDelete]
        public async Task<IActionResult> DeleteUsers([FromBody] List<Guid> userIds)
        {
            if (userIds == null || userIds.Count == 0)
            {
                return BadRequest("No users selected to delete.");
            }

            var users = await _context.Users.Where(u => userIds.Contains(u.Id)).ToListAsync();
            if (users.Count == 0)
            {
                return NotFound("No users found for the given IDs.");
            }

            _context.Users.RemoveRange(users);
            await _context.SaveChangesAsync();

            return Ok("Users deleted.");
        }
    }
}
