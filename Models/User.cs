using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace usermanagement.Models
{
    public class User
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required]
        public required string Name { get; set; } // Add `required` modifier (C# 11+)

        [Required, EmailAddress]
        public required string Email { get; set; } // Add `required` modifier (C# 11+)

        [Required]
        public required string PasswordHash { get; set; } // Add `required` modifier (C# 11+)

        public DateTime LastLoginTime { get; set; } = DateTime.UtcNow;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public bool IsBlocked { get; set; } = false;
    }
}
