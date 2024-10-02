namespace Chess_Online.Server.Models.Pieces;

public class Pawn : Piece
{
    public Pawn(TeamEnum Team) : base(Team, PieceTypeEnum.Pawn,1,1, DirectMovesMap(Team), DirectAttackMap(Team))
    {
    }

    private static int[,] DirectMovesMap(TeamEnum Team)
    {
        int ActionDirectionMod = Team.Equals(TeamEnum.White) ? -1 : 1;
        return new int[,] { { 1 * ActionDirectionMod,0 } };
    }
    private static int[,] DirectAttackMap(TeamEnum Team)
    {
        int ActionDirectionMod = Team.Equals(TeamEnum.White) ? -1 : 1;
        return new int[,] { { 1 * ActionDirectionMod, 1  }, { 1 * ActionDirectionMod, - 1} };
    }
    public override int GetJumpsOnMove()
    {
        if (!Moved)
            return 2;
        else
            return 1;
    }
}
