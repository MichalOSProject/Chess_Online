using System.Net.WebSockets;

namespace Chess_Online.Server.Services.Interfaces
{
    public interface IGameService
    {
        Task GameAction(WebSocket webSocket, string token);
    }
}
