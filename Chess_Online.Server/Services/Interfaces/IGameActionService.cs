using System.Net.WebSockets;

namespace Chess_Online.Server.Services.Interfaces
{
    public interface IGameActionService
    {
        Task GameAction(WebSocket webSocket, string token);
    }
}
