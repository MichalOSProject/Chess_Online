using Chess_Online.Server.Models.Pieces;
using System.ComponentModel.DataAnnotations;

namespace Chess_Online.Server.Models.OutputModels;

public class LobbyInfoModelOutput
{
    [Required]
    public int Id { get; set; }
    [Required]
    public string PlayerOneId { get; set; }
    public string? PlayerTwoId { get; set; }
    [Required]
    public string PlayerOne { get; set; }
    public string? PlayerTwo { get; set; }

}
