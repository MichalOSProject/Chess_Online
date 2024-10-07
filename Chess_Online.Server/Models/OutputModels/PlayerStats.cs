namespace Chess_Online.Server.Models.OutputModels;
    public class PlayerStats
    {
    public PlayerStats(string username)
    {
        this.username = username;
        this.totalGames = 0;
        this.endedGames = 0;
        this.winnings = 0;
        this.lostGames = 0;
        this.gamesAsWhite = 0;
        this.gamesAsBlack = 0;
    }

    public string username { get; set; }
    public int totalGames { get; set; }
    public int endedGames { get; set; }
    public int winnings { get; set; }
    public int lostGames { get; set; }
    public int gamesAsWhite { get; set; }
    public int gamesAsBlack { get; set; }

}

