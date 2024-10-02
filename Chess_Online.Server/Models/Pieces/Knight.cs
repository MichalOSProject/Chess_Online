namespace Chess_Online.Server.Models.Pieces;

public class Knight : Piece
{
    private new static readonly int[,] Moves = { { -2, -1 }, { -2, 1 }, { 2, -1 }, { 2, 1 }, { -1, -2 }, { 1, -2 }, { -1, 2 }, { 1, 2 } };
    public Knight(TeamEnum Team) : base(Team, PieceTypeEnum.Knight,1,1,Moves)
    {
    }
}
