using System;
using NLog;

namespace NanoLoanApi.Utils
{
    public class Util
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();

        public static void LogMessage(string message)
        {
            logger.Info(message);
        }

        public static void LogError(Exception e, string additionalMessage)
        {
            logger.Error(e,additionalMessage);
        }
        public static void LogError(string errorMessage)
        {
            logger.Error(errorMessage);
        }
    }
}
