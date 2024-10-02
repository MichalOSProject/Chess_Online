using System.Net.WebSockets;

namespace Chess_Online.Server.Services.Interfaces;
    public interface ILobbyService
    {
    Task HandleWebSocketAsync(WebSocket webSocket, string token);
    }
