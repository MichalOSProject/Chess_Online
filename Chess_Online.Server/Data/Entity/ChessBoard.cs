using Chess_Online.Server.Models.Pieces;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Chess_Online.Server.Data.Entity
{
    public partial class ChessBoard
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        [Column(TypeName = "nvarchar(max)")]
        public string CB_8A { get; set; }

        [Required]
        [Column(TypeName = "nvarchar(max)")]
        public string CB_8B { get; set; }

        [Required]
        [Column(TypeName = "nvarchar(max)")]
        public string CB_8C { get; set; }

        [Required]
        [Column(TypeName = "nvarchar(max)")]
        public string CB_8D { get; set; }

        [Required]
        [Column(TypeName = "nvarchar(max)")]
        public string CB_8E { get; set; }

        [Required]
        [Column(TypeName = "nvarchar(max)")]
        public string CB_8F { get; set; }

        [Required]
        [Column(TypeName = "nvarchar(max)")]
        public string CB_8G { get; set; }

        [Required]
        [Column(TypeName = "nvarchar(max)")]
        public string CB_8H { get; set; }

        [Required]
        [Column(TypeName = "nvarchar(max)")]
        public string CB_7A { get; set; }

        [Required]
        [Column(TypeName = "nvarchar(max)")]
        public string CB_7B { get; set; }

        [Required]
        [Column(TypeName = "nvarchar(max)")]
        public string CB_7C { get; set; }

        [Required]
        [Column(TypeName = "nvarchar(max)")]
        public string CB_7D { get; set; }

        [Required]
        [Column(TypeName = "nvarchar(max)")]
        public string CB_7E { get; set; }

        [Required]
        [Column(TypeName = "nvarchar(max)")]
        public string CB_7F { get; set; }

        [Required]
        [Column(TypeName = "nvarchar(max)")]
        public string CB_7G { get; set; }

        [Required]
        [Column(TypeName = "nvarchar(max)")]
        public string CB_7H { get; set; }

        [Required]
        [Column(TypeName = "nvarchar(max)")]
        public string CB_6A { get; set; }

        [Required]
        [Column(TypeName = "nvarchar(max)")]
        public string CB_6B { get; set; }

        [Required]
        [Column(TypeName = "nvarchar(max)")]
        public string CB_6C { get; set; }

        [Required]
        [Column(TypeName = "nvarchar(max)")]
        public string CB_6D { get; set; }

        [Required]
        [Column(TypeName = "nvarchar(max)")]
        public string CB_6E { get; set; }

        [Required]
        [Column(TypeName = "nvarchar(max)")]
        public string CB_6F { get; set; }

        [Required]
        [Column(TypeName = "nvarchar(max)")]
        public string CB_6G { get; set; }

        [Required]
        [Column(TypeName = "nvarchar(max)")]
        public string CB_6H { get; set; }

        [Required]
        [Column(TypeName = "nvarchar(max)")]
        public string CB_5A { get; set; }

        [Required]
        [Column(TypeName = "nvarchar(max)")]
        public string CB_5B { get; set; }

        [Required]
        [Column(TypeName = "nvarchar(max)")]
        public string CB_5C { get; set; }

        [Required]
        [Column(TypeName = "nvarchar(max)")]
        public string CB_5D { get; set; }

        [Required]
        [Column(TypeName = "nvarchar(max)")]
        public string CB_5E { get; set; }

        [Required]
        [Column(TypeName = "nvarchar(max)")]
        public string CB_5F { get; set; }

        [Required]
        [Column(TypeName = "nvarchar(max)")]
        public string CB_5G { get; set; }

        [Required]
        [Column(TypeName = "nvarchar(max)")]
        public string CB_5H { get; set; }

        [Required]
        [Column(TypeName = "nvarchar(max)")]
        public string CB_4A { get; set; }

        [Required]
        [Column(TypeName = "nvarchar(max)")]
        public string CB_4B { get; set; }

        [Required]
        [Column(TypeName = "nvarchar(max)")]
        public string CB_4C { get; set; }

        [Required]
        [Column(TypeName = "nvarchar(max)")]
        public string CB_4D { get; set; }

        [Required]
        [Column(TypeName = "nvarchar(max)")]
        public string CB_4E { get; set; }

        [Required]
        [Column(TypeName = "nvarchar(max)")]
        public string CB_4F { get; set; }

        [Required]
        [Column(TypeName = "nvarchar(max)")]
        public string CB_4G { get; set; }

        [Required]
        [Column(TypeName = "nvarchar(max)")]
        public string CB_4H { get; set; }

        [Required]
        [Column(TypeName = "nvarchar(max)")]
        public string CB_3A { get; set; }

        [Required]
        [Column(TypeName = "nvarchar(max)")]
        public string CB_3B { get; set; }

        [Required]
        [Column(TypeName = "nvarchar(max)")]
        public string CB_3C { get; set; }

        [Required]
        [Column(TypeName = "nvarchar(max)")]
        public string CB_3D { get; set; }

        [Required]
        [Column(TypeName = "nvarchar(max)")]
        public string CB_3E { get; set; }

        [Required]
        [Column(TypeName = "nvarchar(max)")]
        public string CB_3F { get; set; }

        [Required]
        [Column(TypeName = "nvarchar(max)")]
        public string CB_3G { get; set; }

        [Required]
        [Column(TypeName = "nvarchar(max)")]
        public string CB_3H { get; set; }

        [Required]
        [Column(TypeName = "nvarchar(max)")]
        public string CB_2A { get; set; }

        [Required]
        [Column(TypeName = "nvarchar(max)")]
        public string CB_2B { get; set; }

        [Required]
        [Column(TypeName = "nvarchar(max)")]
        public string CB_2C { get; set; }

        [Required]
        [Column(TypeName = "nvarchar(max)")]
        public string CB_2D { get; set; }

        [Required]
        [Column(TypeName = "nvarchar(max)")]
        public string CB_2E { get; set; }

        [Required]
        [Column(TypeName = "nvarchar(max)")]
        public string CB_2F { get; set; }

        [Required]
        [Column(TypeName = "nvarchar(max)")]
        public string CB_2G { get; set; }

        [Required]
        [Column(TypeName = "nvarchar(max)")]
        public string CB_2H { get; set; }

        [Required]
        [Column(TypeName = "nvarchar(max)")]
        public string CB_1A { get; set; }

        [Required]
        [Column(TypeName = "nvarchar(max)")]
        public string CB_1B { get; set; }

        [Required]
        [Column(TypeName = "nvarchar(max)")]
        public string CB_1C { get; set; }

        [Required]
        [Column(TypeName = "nvarchar(max)")]
        public string CB_1D { get; set; }

        [Required]
        [Column(TypeName = "nvarchar(max)")]
        public string CB_1E { get; set; }

        [Required]
        [Column(TypeName = "nvarchar(max)")]
        public string CB_1F { get; set; }

        [Required]
        [Column(TypeName = "nvarchar(max)")]
        public string CB_1G { get; set; }

        [Required]
        [Column(TypeName = "nvarchar(max)")]
        public string CB_1H { get; set; }
    }
}

