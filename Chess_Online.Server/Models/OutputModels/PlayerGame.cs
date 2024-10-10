namespace Chess_Online.Server.Models.OutputModels;
    public class PlayerGame
    {
    public PlayerGame()
    {
        this.gameId = 0;
        this.oponentUsername = "";
        this.playedAs = "";
        this.ended = true;
        this.winner = "";
    }

    public int gameId { get; set; }
    public string oponentUsername { get; set; }
    public string playedAs { get; set; }
    public bool ended { get; set; }
    public string winner { get; set; }
}

