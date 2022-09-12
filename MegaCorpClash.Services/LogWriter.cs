﻿namespace MegaCorpClash.Services;

public static class LogWriter
{
    private const string CHAT_LOG_DIRECTORY = "./ChatLogs";

    static LogWriter()
    {
        if (!Directory.Exists(CHAT_LOG_DIRECTORY))
        {
            Directory.CreateDirectory(CHAT_LOG_DIRECTORY);
        }
    }

    public static void WriteMessage(string message)
    {
        File.AppendAllText(
            Path.Combine(CHAT_LOG_DIRECTORY, $"MegaCorpClash-{DateTime.Now:yyyy-MM-dd}.log"),
            $"{message}{Environment.NewLine}");
    }
}