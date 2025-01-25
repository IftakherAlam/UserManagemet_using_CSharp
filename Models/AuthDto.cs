namespace usermanagement.Models
{
    public class RegisterRequest
    {
        public required string Name { get; set; } // Add `required` modifier (C# 11+)
        public required string Email { get; set; } // Add `required` modifier (C# 11+)
        public required string Password { get; set; } // Add `required` modifier (C# 11+)
    }

    public class LoginRequest
    {
        public required string Email { get; set; } // Add `required` modifier (C# 11+)
        public required string Password { get; set; } // Add `required` modifier (C# 11+)
    }
}
