namespace Chess_Online.Server.Models.Pieces;

public class Queen : Piece
{
    private new static readonly int[,] Moves = { { -1, 0 }, { 1, 0 }, { 0, -1 }, { 0, 1 }, { -1, -1 }, { -1, 1 }, { 1, -1 }, { 1, 1 } };
    public Queen(TeamEnum Team) : base(Team, PieceTypeEnum.Queen,7,7, Moves)
    {
    }
}
