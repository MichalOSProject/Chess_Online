using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Chess_Online.Server.Data.Entity
{
    public partial class JwtTokens
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [Required]
        public string Token { get; set; } = null!;
        [Required]
        public string Jti { get; set; } = null!;
        [Required]
        public string UserId { get; set; }
        [Required]
        public DateTime Expiration { get; set; }
        [Required]
        public bool Enabled { get; set; }
    }
}

