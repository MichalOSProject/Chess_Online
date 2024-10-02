using Chess_Online.Server.Models.Pieces;
using System.ComponentModel.DataAnnotations;

namespace Chess_Online.Server.Models.InputModels
{
    public class CreateNewGameModelInput
    {
        [Required]
        public string playerTeamWhite { get; set; }
        [Required]
        public string playerTeamBlack { get; set; }
        [Required]
        public TeamEnum firstTeam { get; set; }
    }
}
