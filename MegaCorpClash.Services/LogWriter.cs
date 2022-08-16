namespace MegaCorpClash.Services;

public class LogWriter
{
    private const string CHAT_LOG_DIRECTORY = "./ChatLogs";

    public LogWriter()
    {
        if (!Directory.Exists(CHAT_LOG_DIRECTORY))
        {
            Directory.CreateDirectory(CHAT_LOG_DIRECTORY);
        }
    }

    public void WriteMessage(string message)
    {
        File.AppendAllText(
            Path.Combine(CHAT_LOG_DIRECTORY, $"MegaCorpClash-{DateTime.Now:yyyy-MM-dd}.log"),
            $"{message}{Environment.NewLine}");
    }
}