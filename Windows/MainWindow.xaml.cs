using System.ComponentModel;
using GadzzaaTB.Pages;
using NLog;

// ReSharper disable MemberCanBePrivate.Global

namespace GadzzaaTB.Windows
{
    /// <summary>
    ///     Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        public readonly BugReport BugReport;
        public readonly DebugOsu DebugOsu;
        public readonly Main Main;

        public MainWindow()
        {
            InitializeComponent();
            Main = new Main();
            BugReport = new BugReport();
            DebugOsu = new DebugOsu();
            Closing += OnClosing;
            DebugOsu.UpdateModsText();
            NavigationFrame.NavigationService.Navigate(Main);
        }

        private void OnClosing(object sender, CancelEventArgs e)
        {
            var logger = LogManager.GetLogger("toPostSharp");
            logger.Debug("### LOGGING SESSION FINISHED ###");
            if (Main.twitch.JoinedChannel != null) Main.twitch.Client.LeaveChannel(Main.twitch.JoinedChannel);
            Main.twitch.Client.Disconnect();
            BugReport.IsClosing = true;
            BugReport.Close();
            DebugOsu.Close();
            LogManager.Shutdown();
        }
    }
}