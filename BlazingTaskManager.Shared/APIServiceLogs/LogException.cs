using Serilog;

namespace BlazingTaskManager.Shared.APIServiceLogs
{
    public static class LogException
    {
        public static void LogExceptions(Exception ex)
        {
            LogToFile(ex.Message);
            LogToConsole(ex.Message);
            LogToDebugger(ex.Message);
        }

        /// <summary>
        /// Log to file
        /// </summary>
        /// <param name="message"></param>
        public static void LogToFile(string message) => Log.Information(message);
        /// <summary>
        /// Log to console
        /// </summary>
        /// <param name="message"></param>
        public static void LogToConsole(string message) => Log.Warning(message);
        /// <summary>
        /// Log to debugger
        /// </summary>
        /// <param name="message"></param>
        public static void LogToDebugger(string message) => Log.Debug(message);
    }
}
