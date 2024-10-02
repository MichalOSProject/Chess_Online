using System.ComponentModel.DataAnnotations;

namespace Chess_Online.Server.Models.InputModels
{
    public class LoginModelInput
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }
}
