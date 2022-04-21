using System;
using System.Windows;
using NLog;
using NLog.Config;
using NLog.Targets;
using PostSharp.Patterns.Diagnostics;
using PostSharp.Patterns.Diagnostics.Backends.NLog;
#pragma warning disable CS0618

namespace GadzzaaTB
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App
    {
        public App()
        {
            Startup += OnStartup;
        }

        private void OnStartup(object sender, StartupEventArgs e)
        {
            var nlogConfig = new LoggingConfiguration();
            
            var fileTarget = new FileTarget("myLoggerTarget")
            {
                FileName = @"${specialfolder:folder=LocalApplicationData}/GadzzaaTB/logs/${date:format=yyyy-MM-dd HH.mm.ss}.log", 
                Layout = "${time} ${uppercase:${level}} ${message}",
                KeepFileOpen = true,
                ConcurrentWrites = false,
            };
            nlogConfig.AddTarget(fileTarget);
            nlogConfig.LoggingRules.Add(new LoggingRule("*", NLog.LogLevel.Debug, fileTarget));

            var consoleTarget = new ConsoleTarget("console");
            nlogConfig.AddTarget(consoleTarget);
            nlogConfig.LoggingRules.Add(new LoggingRule("*", NLog.LogLevel.Debug, consoleTarget));
            
            LogManager.EnableLogging();
            // Configure PostSharp Logging to use NLog.
            LoggingServices.DefaultBackend = new NLogLoggingBackend(new LogFactory(nlogConfig));
        }
    }
}