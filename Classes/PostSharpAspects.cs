using NLog;
using NLog.Config;
using NLog.Targets;
using PostSharp.Extensibility;
using PostSharp.Patterns.Diagnostics;
using PostSharp.Patterns.Diagnostics.Backends.NLog;
using LogLevel = NLog.LogLevel;

#pragma warning disable CS0618

[assembly:
    Log(AttributePriority = 1,
        AttributeTargetMemberAttributes =
            MulticastAttributes.Protected | MulticastAttributes.Internal | MulticastAttributes.Public)]
[assembly: Log(AttributePriority = 2, AttributeExclude = true, AttributeTargetMembers = "get_*")]

namespace GadzzaaTB.Classes
{
    public static class PostSharpAspects
    {
        public static void StartNLog()
        {
            var target = new NLogCollectingTarget("toPostSharp"); // the name does not matter
            var sourceConfiguration = new LoggingConfiguration();
            sourceConfiguration.AddTarget(target);
            sourceConfiguration.LoggingRules.Add(new LoggingRule("*", LogLevel.Trace, target)); // Capture all events.
            LogManager.Configuration = sourceConfiguration; // Set it as the default configuration
            LogManager.EnableLogging();

            var nlogConfig = new LoggingConfiguration();

            var fileTarget = new FileTarget("file")
            {
                FileName =
                    @"${specialfolder:folder=LocalApplicationData}/GadzzaaTB/logs/${date:format=yyyy-MM-dd}.log",
                Layout = "${time} ${uppercase:${level}} ${message}",
                KeepFileOpen = true,
                ConcurrentWrites = true
            };
            nlogConfig.AddTarget(fileTarget);
            nlogConfig.LoggingRules.Add(new LoggingRule("*", LogLevel.Trace, fileTarget));

            var consoleTarget = new ConsoleTarget("console");
            nlogConfig.AddTarget(consoleTarget);
            nlogConfig.LoggingRules.Add(new LoggingRule("*", LogLevel.Trace, consoleTarget));


            // Configure PostSharp Logging to use NLog.
            LoggingServices.DefaultBackend = new NLogLoggingBackend(new LogFactory(nlogConfig));
        }
    }
}