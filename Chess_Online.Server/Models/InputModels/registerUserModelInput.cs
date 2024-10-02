using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Chess_Online.Server.Models.InputModels
{
    public class registerUserModelInput
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        [Required]
        public string Username { get; set; } = null!;
        [Required]
        [PasswordPropertyText]
        public string? Password { get; set; } = null!;
    }
}



