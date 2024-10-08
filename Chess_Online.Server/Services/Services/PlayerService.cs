using Chess_Online.Server.Data.Entity;
using Chess_Online.Server.Models.OutputModels;
using Chess_Online.Server.Models.Pieces;
using Microsoft.AspNetCore.Identity;
using Chess_Online.Server.Services.Interfaces;
using Chess_Online.Server.Data;
using Microsoft.AspNetCore.Mvc;
using System.IO;
using System.Threading.Tasks;
using Azure.Core;

namespace Chess_Online.Server.Services.Services
{
    public class PlayerService : IPlayerService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationDbContext _context;

        public PlayerService(UserManager<ApplicationUser> userManager, ApplicationDbContext context)
        {
            _userManager = userManager;
            _context = context;
        }

        public async Task<string> GetProfilePicture(string username)
        {
            var user = await _userManager.FindByNameAsync(username);
            if (user == null)
            {
                return null;
            }

            var photoUrl = user.PhotoUrl;
            if (string.IsNullOrEmpty(photoUrl))
            {
                return null;
            }

            return $"/StaticFiles/{Path.GetFileName(photoUrl)}";
        }

        public async Task<string> UploadProfilePicture(IFormFile file, string userId)
        {
            if (file == null || file.Length == 0)
            {
                return "No file uploaded.";
            }

            const long maxFileSize = 2 * 1024 * 1024; // 2 MB
            if (file.Length > maxFileSize)
            {
                return "File size exceeds the maximum limit of 2MB.";
            }

            var uploadsFolderPath = Path.Combine(Directory.GetCurrentDirectory(), "Uploads");
            if (!Directory.Exists(uploadsFolderPath))
            {
                Directory.CreateDirectory(uploadsFolderPath);
            }

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return "User not found.";
            }

            if (!string.IsNullOrEmpty(user.PhotoUrl))
            {
                var existingFilePath = Path.Combine(Directory.GetCurrentDirectory(), "Uploads", Path.GetFileName(user.PhotoUrl));
                if (File.Exists(existingFilePath))
                {
                    File.Delete(existingFilePath);
                }
            }

            var uniqueFileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
            var filePath = Path.Combine(uploadsFolderPath, uniqueFileName);

            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(fileStream);
            }

            user.PhotoUrl = $"/Uploads/{uniqueFileName}";

            await _context.SaveChangesAsync();

            return null;
        }


        public async Task<PlayerStats> GetPlayerStats(string username)
        {
            if (string.IsNullOrEmpty(username))
                return null;
            ApplicationUser user = await _userManager.FindByNameAsync(username);

            if (user == null)
                return null;

            PlayerStats playerStats = new PlayerStats(username);

            GameInstanceEntity[] playerGames = _context.GameInstancesEntity
                .Where(x => x.PlayerTeamWhite == user.Id || x.PlayerTeamBlack == user.Id)
                .ToArray();

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
}
