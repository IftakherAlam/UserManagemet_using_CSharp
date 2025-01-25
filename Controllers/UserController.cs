using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using usermanagement.Data;
using usermanagement.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace usermanagement.Controllers
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

       [HttpPut("block")]
public async Task<IActionResult> BlockUsers([FromBody] List<Guid> userIds)
{
    if (userIds != null && userIds.Count > 0)
    {
        var usersToBlock = await _context.Users.Where(u => userIds.Contains(u.Id)).ToListAsync();
        if (usersToBlock.Count == 0)
        {
            return NotFound("No users found for the given IDs.");
        }

        usersToBlock.ForEach(u => u.IsBlocked = true);
        await _context.SaveChangesAsync();

        return Ok("Users blocked.");
    }
    else
    {
        // Block all users if no user is selected
        var users = await _context.Users.Where(u => u.IsBlocked == false).ToListAsync();
        if (users.Count == 0)
        {
            return BadRequest("No users to block.");
        }

        foreach (var user in users)
        {
            user.IsBlocked = true;
        }

        await _context.SaveChangesAsync();
        return Ok(new { message = "All users have been blocked. Please log in again." });
    }
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
