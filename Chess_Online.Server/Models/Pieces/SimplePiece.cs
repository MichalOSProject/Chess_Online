namespace Chess_Online.Server.Models.Pieces;
public class SimplePiece
{
    public List<(int column, int row)>? CheckedMoves { get; private set; }
    public TeamEnum Team { get; private set; }
    public PieceTypeEnum PieceType { get; private set; }

    public SimplePiece(TeamEnum Team, PieceTypeEnum PieceType, List<(int column, int row)> CheckedMoves)
    {
        this.Team = Team;
        this.PieceType = PieceType;
        this.CheckedMoves = CheckedMoves;
    }
}
