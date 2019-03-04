using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
[assembly: log4net.Config.XmlConfigurator(Watch = true)]
namespace APIProject
{
    public static class Logging
    {
        public enum LogLevel
        {
            Fatal,
            Error,
            Warn,
            Info,
            Debug

        }

        public static void LogMessage(MethodBase sender, LogLevel level, string message)
        {

            log4net.ILog log;
            if (sender != null)
            {
                log = log4net.LogManager.GetLogger(string.Format("{0}.{1}", sender.DeclaringType.ToString(), sender.Name));
            }
            else
            {
                log = log4net.LogManager.GetLogger("Default");
            }

            if (System.Diagnostics.Debugger.IsAttached)
            {
                string outMessage = string.Format("LOGGER: {0} => {1}", level, message);
                System.Diagnostics.Debug.WriteLine(outMessage);
            }

            switch (level)
            {
                case LogLevel.Debug:
                    log.Debug(message);
                    break;
                case LogLevel.Error:
                    log.Error(message);
                    break;
                case LogLevel.Fatal:
                    log.Fatal(message);
                    break;
                case LogLevel.Info:
                    log.Info(message);
                    break;
                case LogLevel.Warn:
                    log.Warn(message);
                    break;
            }

        }

        public static void LogException(MethodBase sender, LogLevel level, Exception ex)
        {
            LogException(sender, level, ex, false);
        }
        public static void LogException(MethodBase sender, LogLevel level, Exception ex, bool LogBusinessException)
        {
            if (ex is BusinessException && LogBusinessException == false)
            {
                //skip logging the error message that was displayed to the client.
                return;
            }
            log4net.ILog log;
            if (sender != null)
            {
                log = log4net.LogManager.GetLogger(string.Format("{0}.{1}", sender.DeclaringType.ToString(), sender.Name));
            }
            else
            {
                log = log4net.LogManager.GetLogger("Default");
            }

            string message = "";

            message += ex.ToString();


            switch (level)
            {
                case LogLevel.Debug:
                    log.Debug(message);
                    break;
                case LogLevel.Error:
                    log.Error(message);
                    break;
                case LogLevel.Fatal:
                    log.Fatal(message);
                    break;
                case LogLevel.Info:
                    log.Info(message);
                    break;
                case LogLevel.Warn:
                    log.Warn(message);
                    break;
            }

        }


        public static void LogMessageFormat(MethodBase sender, LogLevel level, string format, object arg0)
        {

            log4net.ILog log;
            if (sender != null)
            {
                log = log4net.LogManager.GetLogger(string.Format("{0}.{1}", sender.DeclaringType.ToString(), sender.Name));
            }
            else
            {
                log = log4net.LogManager.GetLogger("Default");
            }

            if (System.Diagnostics.Debugger.IsAttached)
            {
                string outMessage = string.Format("LOGGER: {0} => {1}", level, string.Format(format, arg0));
                System.Diagnostics.Debug.WriteLine(outMessage);
            }

            switch (level)
            {
                case LogLevel.Debug:
                    log.DebugFormat(format, arg0);
                    break;
                case LogLevel.Error:
                    log.ErrorFormat(format, arg0);
                    break;
                case LogLevel.Fatal:
                    log.FatalFormat(format, arg0);
                    break;
                case LogLevel.Info:
                    log.InfoFormat(format, arg0);
                    break;
                case LogLevel.Warn:
                    log.WarnFormat(format, arg0);
                    break;
            }

        }
        public static void LogMessageFormat(MethodBase sender, LogLevel level, string format, object[] args)
        {
            log4net.ILog log;
            if (sender != null)
            {
                log = log4net.LogManager.GetLogger(string.Format("{0}.{1}", sender.DeclaringType.ToString(), sender.Name));
            }
            else
            {
                log = log4net.LogManager.GetLogger("Default");
            }

            if (System.Diagnostics.Debugger.IsAttached)
            {
                string outMessage = string.Format("LOGGER: {0} => {1}", level, string.Format(format, args));
                System.Diagnostics.Debug.WriteLine(outMessage);
            }
            switch (level)
            {
                case LogLevel.Debug:
                    log.DebugFormat(format, args);
                    break;
                case LogLevel.Error:
                    log.ErrorFormat(format, args);
                    break;
                case LogLevel.Fatal:
                    log.FatalFormat(format, args);
                    break;
                case LogLevel.Info:
                    log.InfoFormat(format, args);
                    break;
                case LogLevel.Warn:
                    log.WarnFormat(format, args);
                    break;
            }

        }
    }
}