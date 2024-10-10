using Chess_Online.Server.Models.OutputModels;
using Microsoft.AspNetCore.Mvc;
using System.Net.WebSockets;

namespace Chess_Online.Server.Services.Interfaces;
public interface IPlayerService
{
    Task<PlayerStats> GetPlayerStats(string username);
    Task<string> UploadProfilePicture(IFormFile file, string userId);
    Task<string> GetProfilePicture(string username);
    Task<List<PlayerGame>> GetPlayerGamesList(string username);
}
