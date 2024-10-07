namespace Chess_Online.Server.Models.Pieces;

public abstract class Piece
{
    public Boolean Moved = false;
    public Boolean AttackOnKing = false;
    public int[,] Moves { get; protected set; }
    public int[,] Attack { get; protected set; }
    public List<(int column,int row)>? CheckedMoves { get; private set; }
    public int JumpsOnMove { get; protected set; }
    public int JumpsOnAttack { get; protected set; }
    public int DamageByWhite { get; private set; }
    public int DamageByBlack { get; private set; }
    public TeamEnum Team { get; private set; }
    public PieceTypeEnum PieceType { get; private set; }

    public Piece(TeamEnum Team, PieceTypeEnum PieceType, int JumpsOnMove, int JumpsOnAttack, int[,] Moves, int[,] Attack)
    {
        this.Team = Team;
        this.PieceType = PieceType;
        this.JumpsOnMove = JumpsOnMove;
        this.JumpsOnAttack = JumpsOnAttack;
        this.Moves = Moves;
        this.DamageByBlack = 0;
        this.DamageByWhite = 0;
        this.Attack = Attack;
        this.CheckedMoves = new List<(int column, int row)>();
    }
    public Piece(TeamEnum team, PieceTypeEnum pieceType, int jumpsOnMove, int jumpsOnAttack, int[,] moves)
    : this(team, pieceType, jumpsOnMove, jumpsOnAttack, moves, moves)
    {
    }
    public virtual int GetJumpsOnMove()
    {
        return JumpsOnMove;
    }
    public virtual void SetDamageByWhite(int dmg) {
        DamageByWhite += dmg;
    }
    public virtual void SetDamageByBlack(int dmg)
    {
        DamageByBlack += dmg;
    }
    public virtual void ResetDamage() { 
        DamageByWhite = 0;
        DamageByBlack = 0;
    }
    public virtual void ResetAttackOnKing() {
        AttackOnKing = false;
    }
    public virtual void AddCheckedMoves(int column,int row)
    {
        CheckedMoves.Add((column, row));
    }
    public virtual void ResetCheckedMoves()
    {
        if(CheckedMoves!=null)
        CheckedMoves.Clear();
    }
}
