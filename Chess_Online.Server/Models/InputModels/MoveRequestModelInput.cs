using System.ComponentModel.DataAnnotations;

namespace Chess_Online.Server.Models.InputModels
{
    public class MoveRequestModelInput
    {
        [Required]
        public int[] CoordsPiece { get; set; }

        [Required]
        public int[] CoordsDestination { get; set; }
    }
}
