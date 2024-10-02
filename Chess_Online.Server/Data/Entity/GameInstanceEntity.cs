using Chess_Online.Server.Models.Pieces;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Chess_Online.Server.Data.Entity
{
    public class GameInstanceEntity
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        public int ChessBoardId { get; set; }

        [ForeignKey("ChessBoardId")]
        public virtual ChessBoard ChessBoardMap { get; set; }

        [Required]
        public string PlayerTeamWhite { get; set; }

        [Required]
        public string PlayerTeamBlack { get; set; }

        [Required]
        public CheckmateStatusEnum CheckByWhite { get; set; }

        [Required]
        public CheckmateStatusEnum CheckByBlack { get; set; }

        [Required]
        public bool GameEnded { get; set; }

        [Required]
        public TeamEnum PlayerTurn { get; set; }
    }
}
