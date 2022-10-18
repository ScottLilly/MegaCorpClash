using MegaCorpClash.Models.CustomEventArgs;

namespace MegaCorpClash.Models.ChatConnectors;

public interface IChatConnector
{
    event EventHandler OnConnected;
    event EventHandler OnDisconnected;
    event EventHandler<ChattedEventArgs> OnPersonChatted;
    event EventHandler<GameCommandArgs> OnGameCommandReceived;
    event EventHandler<LogMessageEventArgs> OnLogMessagePublished;
    void Connect();
    void Disconnect();
    void SendChatMessage(string? message);
}