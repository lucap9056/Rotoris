using System.IO;
using System.Text;

namespace Rotoris.Logger
{
    /// <summary>
    /// Provides static methods for logging application messages to an in-memory buffer 
    /// and the console. It also supports exporting the captured logs to a file.
    /// </summary>
    public static class Log
    {
        public static readonly StringBuilder LogBuffer = new();
        private const int MAX_CHAR_CAPACITY = 5 * 1024 * 1024;
        static Log()
        {
            Console.OutputEncoding = Encoding.UTF8;
        }
        public static void Write(string value)
        {
            if (LogBuffer.Length + value.Length > MAX_CHAR_CAPACITY)
            {
                int overflow = LogBuffer.Length + value.Length - MAX_CHAR_CAPACITY;
                LogBuffer.Remove(0, overflow);
            }

            LogBuffer.Append(value);
            Console.Write(value);
            EventAggregator.PublishWriteLogs(value);
        }
        public static void WriteLine(string value)
        {
            if (LogBuffer.Length + value.Length > MAX_CHAR_CAPACITY)
            {
                int overflow = LogBuffer.Length + value.Length - MAX_CHAR_CAPACITY;
                LogBuffer.Remove(0, overflow);
            }
            
            LogBuffer.AppendLine(value);
            Console.WriteLine(value);
            EventAggregator.PublishWriteLogs(value + "\n");
        }

        /// <summary>
        /// Writes an information message to the console and log buffer (with a line terminator).
        /// </summary>
        /// <param name="message">The information message to log.</param>
        public static void Info(string message)
        {
            string logMessage = $"[INFO] {DateTime.Now:HH:mm:ss} - {message}";
            WriteLine(logMessage);
        }

        /// <summary>
        /// Writes a warning message to the console (in Yellow) and log buffer (with a line terminator).
        /// </summary>
        /// <param name="message">The warning message to log.</param>
        public static void Warning(string message)
        {
            ConsoleColor originalColor = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.Yellow;

            string logMessage = $"[WARN] {DateTime.Now:HH:mm:ss} - {message}";
            WriteLine(logMessage);

            Console.ForegroundColor = originalColor;
        }

        /// <summary>
        /// Writes an error message to the console and log buffer (with a line terminator).
        /// </summary>
        /// <param name="message">The error message to log.</param>
        public static void Error(string message)
        {
            ConsoleColor originalColor = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.Red;

            string logMessage = $"[ERROR] {DateTime.Now:HH:mm:ss} - {message}";
            WriteLine(logMessage);

            Console.ForegroundColor = originalColor;
        }

        public static void ExportToFile()
        {
            string timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
            string directoryPath = "./logs";

            string filePath = Path.Combine(directoryPath, $"log_{timestamp}.txt");

            try
            {
                Directory.CreateDirectory(directoryPath);

                File.WriteAllText(filePath, LogBuffer.ToString(), Encoding.UTF8);

                Info($"Logs successfully exported to: {Path.GetFullPath(filePath)}");
            }
            catch (Exception ex)
            {
                Error($"Failed to export logs to file: {ex.Message}");
            }
        }
    }
}
