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
            var nlogConfig = new LoggingConfiguration();

            var fileTarget = new FileTarget("myLoggerTarget")
            {
                FileName =
                    @"${specialfolder:folder=LocalApplicationData}/GadzzaaTB/logs/${date:format=yyyy-MM-dd HH.mm}.log",
                Layout = "${time} ${uppercase:${level}} ${message}",
                KeepFileOpen = true,
                ConcurrentWrites = true
            };
            nlogConfig.AddTarget(fileTarget);
            nlogConfig.LoggingRules.Add(new LoggingRule("*", LogLevel.Debug, fileTarget));

            var consoleTarget = new ConsoleTarget("console");
            nlogConfig.AddTarget(consoleTarget);
            nlogConfig.LoggingRules.Add(new LoggingRule("*", LogLevel.Debug, consoleTarget));

            LogManager.EnableLogging();
            // Configure PostSharp Logging to use NLog.
            LoggingServices.DefaultBackend = new NLogLoggingBackend(new LogFactory(nlogConfig));
        }
    }
}