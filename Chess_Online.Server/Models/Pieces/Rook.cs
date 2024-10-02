namespace Chess_Online.Server.Models.Pieces;

public class Rook : Piece
{
    private new static readonly int[,] Moves = { { -1, 0 }, { 1, 0 }, { 0, -1 }, { 0, 1 } };
    public Rook(TeamEnum Team) : base(Team, PieceTypeEnum.Rook,7, 7,Moves)
    {
    }
}
