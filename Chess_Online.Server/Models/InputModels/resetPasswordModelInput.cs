using Microsoft.AspNetCore.Mvc;

namespace Chess_Online.Server.Models.InputModels
{
    public class resetPasswordModelInput
    {
        public string login { get; set; }
        public string Password { get; set; }
        public string newPassword { get; set; }
    }
}
