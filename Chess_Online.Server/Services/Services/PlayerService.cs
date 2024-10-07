using Chess_Online.Server.Data.Entity;
using Chess_Online.Server.Models.OutputModels;
using Chess_Online.Server.Models.Pieces;
using Microsoft.AspNetCore.Identity;
using Chess_Online.Server.Services.Interfaces;
using Chess_Online.Server.Data;

namespace Chess_Online.Server.Services.Services;
public class PlayerService : IPlayerService
{
    private readonly UserManager<IdentityUser> _userManager;
    private readonly ApplicationDbContext _context;

    public PlayerService(UserManager<IdentityUser> userManager, ApplicationDbContext context)
    {
        _userManager = userManager;
        _context = context;
    }
    public async Task<PlayerStats> GetPlayerStats(string username)
    {
        if (string.IsNullOrEmpty(username))
            return null;
        IdentityUser user = await _userManager.FindByNameAsync(username);

        if (user == null)
            return null;

        PlayerStats playerStats = new PlayerStats(username);

        GameInstanceEntity[] playerGames = _context.GameInstancesEntity.Where(x => x.PlayerTeamWhite == user.Id || x.PlayerTeamBlack == user.Id).ToArray();

        if (!playerGames.Any())
            return playerStats;

        foreach (GameInstanceEntity game in playerGames)
        {
            playerStats.totalGames++;
            if (game.GameEnded)
            {
                playerStats.endedGames++;
                if (game.PlayerTeamWhite.Equals(user.Id))
                {
                    playerStats.gamesAsWhite++;
                    if (game.CheckByWhite.Equals(CheckmateStatusEnum.Defeated))
                        playerStats.winnings++;
                    else
                        playerStats.lostGames++;
                }
                else
                {
                    playerStats.gamesAsBlack++;
                    if (game.CheckByBlack.Equals(CheckmateStatusEnum.Defeated))
                        playerStats.winnings++;
                    else
                        playerStats.lostGames++;
                }
            }
            else
            {
                if (game.PlayerTeamWhite.Equals(user.Id))
                    playerStats.gamesAsWhite++;
                else
                    playerStats.gamesAsBlack++;
            }
        }

        return playerStats;


    }
}

