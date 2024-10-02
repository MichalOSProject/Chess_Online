namespace Chess_Online.Server.Models.Pieces;

public class Bishop : Piece
{
    private new static readonly int[,] Moves = { { -1, -1 }, { -1, 1 }, { 1, -1 }, { 1, 1 } };
    public Bishop(TeamEnum Team) : base(Team, PieceTypeEnum.Bishop,7,7,Moves)
    {
    }
}
