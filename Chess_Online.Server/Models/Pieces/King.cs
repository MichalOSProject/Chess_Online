namespace Chess_Online.Server.Models.Pieces;

public class King : Piece
{
    private new static readonly int[,] Moves = { { -1, 0 }, { 1, 0 }, { 0, -1 }, { 0, 1 }, { -1, -1 }, { -1, 1 }, { 1, -1 }, { 1, 1 } };
    public King(TeamEnum Team) : base(Team, PieceTypeEnum.King,1,1,Moves)
    {
    }
}
