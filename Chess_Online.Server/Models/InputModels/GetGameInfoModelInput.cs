using System.ComponentModel.DataAnnotations;

namespace Chess_Online.Server.Models.InputModels
{
    public class GetGameInfoModelInput
    {
        [Required]
        public int gameId { get; set; }
    }
}
