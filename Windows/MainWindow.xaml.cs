using System;
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
            Closed += OnClosed;
            DebugOsu.UpdateModsText();
            NavigationFrame.NavigationService.Navigate(Main);
        }

        private void OnClosed(object sender, EventArgs e)
        {
            var logger = LogManager.GetLogger("toPostSharp");
            logger.Debug("### LOGGING SESSION FINISHED ###");
            Settings.Default.Username = Main.ChannelNameBox.Text;
            if (Main.Twitch.JoinedChannel != null) Main.Twitch.Client.LeaveChannel(Main.Twitch.JoinedChannel);
            Main.Twitch.Client.Disconnect();
            BugReport.IsClosing = true;
            BugReport.Close();
            DebugOsu.Close();
            LogManager.Shutdown();
            Settings.Default.Save();
        }
    }
}