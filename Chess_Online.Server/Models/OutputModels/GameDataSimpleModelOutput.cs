using Chess_Online.Server.Models.Pieces;
using System.ComponentModel.DataAnnotations;

namespace Chess_Online.Server.Models.OutputModels;

public class GameDataSimpleModelOutput
{
    [Required]
    public int Id { get; set; }
    [Required]
    public SimplePiece[,] Pieces { get; set; }
    [Required]
    public bool GameEnded { get; set; }
    [Required]
    public TeamEnum PlayerTurn { get; set; }
    public bool Warning { get; set; }
    public string Message { get; set; }
}
