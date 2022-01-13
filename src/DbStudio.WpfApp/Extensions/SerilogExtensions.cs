﻿using System;
using System.IO;
using Serilog;
using Serilog.Configuration;
using Serilog.Events;

namespace DbStudio.WpfApp.Extensions
{
    public static class SerilogExtensions
    {
        public static void ConfigureSerilog(this LoggerSinkConfiguration configuration)
        {
            configuration.Logger(lg => lg.ToFile(LogEventLevel.Debug))
                .WriteTo.Logger(lg => lg.ToFile(LogEventLevel.Information))
                .WriteTo.Logger(lg => lg.ToFile(LogEventLevel.Warning))
                .WriteTo.Logger(lg => lg.ToFile(LogEventLevel.Error))
                .WriteTo.Logger(lg => lg.ToFile(LogEventLevel.Fatal));
        }

        private static string OutputTemplate =>
            "{NewLine}{NewLine}Date：{Timestamp:yyyy-MM-dd HH:mm:ss.fff}{NewLine}LogLevel：{Level}{NewLine}Message：{Message}{NewLine}{Exception}" +
            new string('-', 50);

        private static string LogFilePath(LogEventLevel logEventLevel) =>
            Path.Combine(AppContext.BaseDirectory, "Logs", logEventLevel.ToString(), "log.log");

        private static void ToFile(
            this LoggerConfiguration loggerConfiguration,
            LogEventLevel logEventLevel)
        {
            loggerConfiguration.Filter.ByIncludingOnly(p => p.Level == logEventLevel)
                .WriteTo.File(
                    LogFilePath(logEventLevel),
                    rollingInterval: RollingInterval.Day,
                    outputTemplate: OutputTemplate);
        }
    }
}