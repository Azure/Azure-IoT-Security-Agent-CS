// <copyright file="SimpleLogger.cs" company="Microsoft">
// Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>

using Microsoft.Azure.IoT.Agent.Core.Configuration;
using Microsoft.Azure.IoT.Agent.Core.Diagnostics;
using Microsoft.Azure.IoT.Agent.Core.Utils;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace Microsoft.Azure.IoT.Agent.Core.Logging
{
    /// <summary>
    /// A very simple logger class to be used for initial debugging
    /// TODO: replace with other more advanced logging method
    /// </summary>
    public static class SimpleLogger
    {
        private static readonly object _logFileLock;
        private static StreamWriter _logFileStreamWriter;
        private static LogLevel _fileLogLevel;
        private static LogLevel _logLevel;
        private static bool _logFileInitialized;
        private static DiagnosticVerbosity _diagnosticVerbosity;
        private static string _logFilePath;

        private static string ResolveLogFilePath()
        {
            string path = LocalConfiguration.General.LogFilePath;

            if (ConfigurationUtils.IsFullyQualifiedPath(path))
                return path;

            string resolvedPath = ConfigurationUtils.AppendToProcessPath(path);
            return resolvedPath;
        }

        /// <summary>
        /// Log a debug message
        /// </summary>
        /// <param name="message">the log line</param>
        /// <param name="sendAsDiagnostic">whether or not go generate a diagnostic event due to this log</param>
        public static void Debug(string message, bool sendAsDiagnostic = false)
        {
            Log(message, LogLevel.Debug, sendAsDiagnostic, null);
        }

        /// <summary>
        /// Log an Information message
        /// </summary>
        /// <param name="message">The log line</param>
        /// <param name="sendAsDiagnostic">whether or not go generate a diagnostic event due to this log</param>
        public static void Information(string message, bool sendAsDiagnostic = false)
        {
            Log(message, LogLevel.Information, sendAsDiagnostic, null);
        }

        /// <summary>
        /// Log a warning message
        /// </summary>
        /// <param name="message">The log line</param>
        /// <param name="sendAsDiagnostic">whether or not go generate a diagnostic event due to this log</param>
        /// <param name="exception">The exception, if any</param>
        public static void Warning(string message, bool sendAsDiagnostic = false, Exception exception = null)
        {
            Log(message, LogLevel.Warning, sendAsDiagnostic, exception);
        }

        /// <summary>
        /// Log an error message
        /// </summary>
        /// <param name="message">The log line</param>
        /// <param name="sendAsDiagnostic">whether or not go generate a diagnostic event due to this log</param>
        /// <param name="exception">The exception, if any</param>
        public static void Error(string message, bool sendAsDiagnostic = true, Exception exception = null)
        {
            Log(message, LogLevel.Error, sendAsDiagnostic, exception);
        }

        /// <summary>
        /// Log a fatal error message
        /// </summary>
        /// <param name="message">The log line</param>
        /// <param name="sendAsDiagnostic">whether or not go generate a diagnostic event due to this log</param>
        /// <param name="exception">The exception, if any</param>
        public static void Fatal(string message, bool sendAsDiagnostic = true, Exception exception = null)
        {
            Log(message, LogLevel.Fatal, sendAsDiagnostic, exception);
        }

        /// <summary>
        /// Clean up logger resources
        /// </summary>
        public static void CleanupLogger()
        {
            if (_logFileStreamWriter == null)
                return;

            lock (_logFileLock)
            {
                if (_logFileStreamWriter == null)
                    return;

                _logFileStreamWriter.Dispose();
                _logFileStreamWriter = null;
            }
        }

        /// <summary>
        /// Logs a message according to the LogLevel, 
        /// Also sends an operational event according to DiagnosticEventLevel
        /// </summary>
        /// <param name="message">The log line</param>
        /// <param name="level">The level at which to log this message</param>
        /// <param name="sendAsDiagnostic">whether or not go generate a diagnostic event due to this log</param>
        /// <param name="exception">The exception, if any</param>
        private static void Log(string message, LogLevel level, bool sendAsDiagnostic, Exception exception = null)
        {
            if (level == LogLevel.Off)
            {
                throw new ArgumentException(
                    "tried logging a message with severity [off], something went terribly wrong https://www.youtube.com/watch?v=t3otBjVZzT0");
            }

            if (_logLevel < level)
                return;

            Guid correlationId = ThreadContext.Get().ExecutionId;

            string formattedMessage = exception == null ? message : exception.FormatExceptionMessage(message);
            string logLine = $"{DateTime.Now} | CorrelationId: {correlationId} | {level.ToString()}: {formattedMessage}";
            Trace.WriteLine(logLine);

            if (_fileLogLevel >= level && _logFileInitialized)
            {
                lock (_logFileLock)
                {
                    _logFileStreamWriter?.WriteLine(logLine);
                    _logFileStreamWriter?.Flush();
                }
            }

            if (AgentConfiguration.IsInitialized())
            {
                if (_diagnosticVerbosity == DiagnosticVerbosity.All || _diagnosticVerbosity == DiagnosticVerbosity.Some && sendAsDiagnostic)
                {
                    SendDiagnosticEvent(formattedMessage, level, correlationId);
                }
            }
        }

        static SimpleLogger()
        {
            _logFileLock = new object();
            if (!TryInitLoggerFromLocalConfiguration())
            {
                _fileLogLevel = LogLevel.Off;
                _logLevel = LogLevel.Fatal;
                _diagnosticVerbosity = DiagnosticVerbosity.None;
            }

            if (_fileLogLevel != LogLevel.Off)
            {
                _logFileInitialized = InitLogFile(_logFilePath);
            }
        }

        /// <summary>
        /// Send a diagnostic event corresponding to a log
        /// </summary>
        /// <param name="message">The message</param>
        /// <param name="level">The severity level of the diagnostic event</param>
        /// <param name="correlationId">current correlation id</param>
        private static void SendDiagnosticEvent(string message, LogLevel level, Guid correlationId)
        {
            //the logger could potentially reach a circular call when sending diagnostic events, don't send yet another diagnostic event if that happens
            var stackTraceFrames = new StackTrace().GetFrames();
            int currentMethodCount = stackTraceFrames.Count(frame => frame.GetMethod().Equals(stackTraceFrames[0].GetMethod()));

            if (currentMethodCount > 1)
            {
                return;
            }

            SystemEvents.DispatchDiagnosicEvent(message, level);
        }

        private static bool InitLogFile(string path)
        {
            try
            {
                _logFileStreamWriter = new StreamWriter(path);
                return true;
            }
            catch (Exception ex)
            {
                Trace.TraceError($"Could not init log file at path: {path}, exception: {ex}");
                return false;
            }
        }

        private static bool TryInitLoggerFromLocalConfiguration()
        {
            try
            {
                _logLevel = LocalConfiguration.General.LogLevel;
                _logFilePath = ResolveLogFilePath();
                _fileLogLevel = LocalConfiguration.General.FileLogLevel;
                _diagnosticVerbosity = LocalConfiguration.General.DiagnosticVerbosityLevel;
                return true;
            }
            catch (Exception ex)
            {
                Trace.TraceError($"Could not init logger configuration: {ex}");
                return false;
            }
        }
    }
}