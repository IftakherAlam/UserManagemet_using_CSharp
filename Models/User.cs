using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace usermangement.Models
{
    public class User
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required]
        public string Name { get; set; }

        [Required, EmailAddress]

        public string Email { get; set; }

        [Required]

        public string PasswordHash { get; set; }

        public DateTime LastLoginTime { get; set; } = DateTime.UtcNow;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public bool IsBlocked { get; set; } = false;
    }
    
}