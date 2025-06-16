using System;
using System.IO;
using UnityEngine;

public static class SessionLogger
{
    private static string logDirectory = Path.Combine(Application.dataPath, "Sessions");
    private static string logFilePath;

    static SessionLogger()
    {
        if (!Directory.Exists(logDirectory))
            Directory.CreateDirectory(logDirectory);

        string timestamp = DateTime.Now.ToString("dd-MM-yyyy-HH-mm-ss");
        logFilePath = Path.Combine(logDirectory, $"Session-{timestamp}.txt");
    }

    public static void Log(string message)
    {
        string timestampedMessage = $"[{DateTime.Now:HH:mm:ss}] {message}";
        Debug.Log(timestampedMessage); 
        File.AppendAllText(logFilePath, timestampedMessage + Environment.NewLine);
    }
}
