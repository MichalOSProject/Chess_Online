using Chess_Online.Server.Data.Entity;
using Chess_Online.Server.Models.Pieces;

namespace Chess_Online.Server.Data
{
    public partial class GameInstance
    {
        public GameInstance(int id, Piece[,] pieces, string playerTeamWhite, string playerTeamBlack, CheckmateStatusEnum checkByWhite, CheckmateStatusEnum checkByBlack, bool gameEnded, TeamEnum playerTurn)
        {
            Id = id;
            Pieces = pieces;
            PlayerTeamWhite = playerTeamWhite;
            PlayerTeamBlack = playerTeamBlack;
            CheckByWhite = checkByWhite;
            CheckByBlack = checkByBlack;
            GameEnded = gameEnded;
            PlayerTurn = playerTurn;
        }

        public int Id { get; set; }

        public Piece[,] Pieces { get; set; }

        public string PlayerTeamWhite { get; set; }

        public string PlayerTeamBlack { get; set; }

        public CheckmateStatusEnum CheckByWhite { get; set; }

        public CheckmateStatusEnum CheckByBlack { get; set; }

        public bool GameEnded { get; set; }

        public TeamEnum PlayerTurn { get; set; }
    }
}
