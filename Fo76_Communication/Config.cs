using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Serilog;

namespace Fo76_Communication
{
    public class Config
    {
        public const bool IgnoreToken = true;
        public static void InitializeLogger()
        {
            LoggerConfiguration loggerConfiguration = new LoggerConfiguration();
            loggerConfiguration.MinimumLevel.Debug();
            loggerConfiguration.WriteTo.Console();
            loggerConfiguration.WriteTo.File("fo76_communication.log", rollingInterval: RollingInterval.Day);
            Log.Logger = loggerConfiguration.CreateLogger();

            Log.Information("Start Logging");
        }
    }
}
