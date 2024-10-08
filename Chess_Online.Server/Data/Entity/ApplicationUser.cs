using Microsoft.AspNetCore.Identity;

namespace Chess_Online.Server.Data.Entity
{
    public class ApplicationUser : IdentityUser
    {
        public string PhotoUrl { get; set; }
    }

}
