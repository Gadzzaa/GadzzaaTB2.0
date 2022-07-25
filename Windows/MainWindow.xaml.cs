using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using GadzzaaTB.Classes;
using NLog;
using OsuMemoryDataProvider;
using OsuMemoryDataProvider.OsuMemoryModels;

// ReSharper disable RedundantCheckBeforeAssignment

// ReSharper disable MemberCanBePrivate.Global

namespace GadzzaaTB.Windows
{
    public partial class MainWindow : INotifyPropertyChanged
    {
        private readonly Logger _logger = LogManager.GetLogger("toPostSharp");
        private readonly MainWindow _mainWindow = (MainWindow)Application.Current.MainWindow;
        private readonly int _readDelay = 33;

        // ReSharper disable once InconsistentNaming
        public readonly StructuredOsuMemoryReader _sreader;
        public readonly OsuBaseAddresses BaseAddresses = new OsuBaseAddresses();
        private string _oldText;
        private string _osuStatus = "Loading...";
        private bool _settingsLoaded;
        private string _twitchButton = "Loading...";
        private string _twitchStatus = "Loading...";
        public BugReport BugReport;
        public DebugOsu DebugOsu;
        public Bot Twitch;

        public MainWindow()
        {
            InitializeComponent();
            DataContext = this;
            _sreader = StructuredOsuMemoryReader.Instance;
            ContentRendered += OnContentRendered;
            Closing += OnClosing;
        }

        public string OsuStatus
        {
            get => _osuStatus;
            set
            {
                if (_osuStatus != value) _osuStatus = value;
                OnPropertyChanged();
            }
        }

        public string TwitchStatus
        {
            get => _twitchStatus;
            set
            {
                if (_twitchStatus != value) _twitchStatus = value;
                OnPropertyChanged();
            }
        }

        public string TwitchConnect
        {
            get => _twitchButton;
            set
            {
                if (_twitchButton != value) _twitchButton = value;
                OnPropertyChanged();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private async void OnContentRendered(object sender, EventArgs e)
        {
            ExecuteWindows();
            DebugOsu.UpdateModsText();
            ExecuteLabels();
            _settingsLoaded = true;
            Twitch.Client.Connect();
            Grid.IsEnabled = true;
            _logger.Info("INITIALIZED!");
            while (true)
            {
                await getOsuData();
            }
            // ReSharper disable once FunctionNeverReturns
        }

        private void OnClosing(object sender, EventArgs e)
        {
            var logger = LogManager.GetLogger("toPostSharp");
            logger.Debug("### LOGGING SESSION FINISHED ###");
            Settings.Default.Username = ChannelNameBox.Text;
            LogManager.Shutdown();
            Settings.Default.Save();
            BugReport.IsClosing = true;
            BugReport.Close();
            DebugOsu.Close();
            if (Twitch.JoinedChannel != null) Twitch.Client.LeaveChannel(Twitch.JoinedChannel);
        }

        private void ChannelNameBox_OnGotFocus(object sender, RoutedEventArgs e)
        {
            _oldText = ChannelNameBox.Text;
            ChannelNameBox.Text = "";
        }

        private void ChannelNameBox_OnLostFocus(object sender, RoutedEventArgs e)
        {
            if (ChannelNameBox.Text == "") ChannelNameBox.Text = _oldText;
        }

        private void ChannelNameBox_OnTextChanged(object sender, TextChangedEventArgs e)
        {
            if (!_settingsLoaded) return;
            Settings.Default.Verified = false;
            TwitchStatus = "Un-Verified";
        }

        private void ConnectionButton_OnClick(object sender, RoutedEventArgs e)
        {
            if (TwitchConnect == "Abort")
            {
                Abort();
                return;
            }

            if (!Settings.Default.Verified)
            {
                Verify();
                return;
            }

            if (TwitchConnect == "Connect") JoinChannel();
            else Twitch.Client.LeaveChannel(ChannelNameBox.Text);
        }

        private void Verify()
        {
            JoinChannel();
            TwitchConnect = "Abort";
            ChannelNameBox.IsEnabled = false;
            TwitchStatus = "Awaiting verification...";
            Twitch.Client.SendMessage(ChannelNameBox.Text,
                "Please type '!verify' in order to confirm that you are the owner of the channel.");
            Console.WriteLine(@"Verification message has been sent, awaiting confirmation...");
        }

        private void JoinChannel()
        {
            if (ChannelNameBox.Text == "Channel Name")
            {
                Console.WriteLine(@"Channel Name invalid!");
                return;
            }

            if (!Twitch.Client.IsConnected)
            {
                Console.WriteLine(@"Twitch connection error!");
                return;
            }

            if (Twitch.JoinedChannel != null) Twitch.Client.LeaveChannel(Twitch.JoinedChannel);
            Twitch.Client.JoinChannel(ChannelNameBox.Text);
        }

        private void Abort()
        {
            Twitch.Client.SendMessage(ChannelNameBox.Text, "Verification process aborted.");
            Console.WriteLine(@"Verification process aborted.");
            Twitch.Client.LeaveChannel(Twitch.JoinedChannel);
        }

        private async Task getOsuData()
        {
            if (!_sreader.CanRead)
            {
                if (OsuStatus != "Process not found!") OsuStatus = "Process not found!";
                await Task.Delay(_readDelay);
            }
            else
            {
                if (OsuStatus != "Running") OsuStatus = "Running";
                _sreader.TryRead(BaseAddresses.Beatmap);
                _sreader.TryRead(BaseAddresses.Skin);
                _sreader.TryRead(BaseAddresses.GeneralData);
                if (BaseAddresses.GeneralData.OsuStatus == OsuMemoryStatus.SongSelect)
                    _sreader.TryRead(BaseAddresses.SongSelectionScores);
                else
                    BaseAddresses.SongSelectionScores.Scores.Clear();
                if (BaseAddresses.GeneralData.OsuStatus == OsuMemoryStatus.ResultsScreen)
                    _sreader.TryRead(BaseAddresses.ResultsScreen);
                UpdateValue.UpdateValues();
                await Task.Delay(_readDelay);
            }
        }

        private void ExecuteWindows()
        {
            Twitch = new Bot();
            BugReport = new BugReport();
            DebugOsu = new DebugOsu();
        }

        private void ExecuteLabels()
        {
            TwitchStatus = "Disconnected";
            if (!Settings.Default.Verified) TwitchStatus = "Un-Verified";
            TwitchConnect = "Connect";
            ChannelNameBox.Text = Settings.Default.Username;
        }

        private void BugButton_OnClick(object sender, RoutedEventArgs e)
        {
            _mainWindow.BugReport.Show();
        }

        private void DiscordButton_OnClick(object sender, RoutedEventArgs e)
        {
            Process.Start("https://discord.gg/TtSQa944Ky");
        }

        private void TwitchButton_OnClick(object sender, RoutedEventArgs e)
        {
            Process.Start("https://twitch.tv/gadzzaa");
        }

        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}