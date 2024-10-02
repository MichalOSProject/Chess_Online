namespace Chess_Online.Server.Models.Pieces;

public class EmptyPiece : Piece
{
    private new static readonly int[,] Moves = { { 0, 0 }, { 0, 0 } };
    public EmptyPiece() : base(TeamEnum.NoMansLand, PieceTypeEnum.Nomad,1,1,Moves)
    {
    }
}
