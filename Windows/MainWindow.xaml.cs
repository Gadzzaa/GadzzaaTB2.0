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
using System.Net.NetworkInformation;
using System.Net.Sockets;

// ReSharper disable RedundantCheckBeforeAssignment

// ReSharper disable MemberCanBePrivate.Global

namespace GadzzaaTB.Windows
{
    public partial class MainWindow : INotifyPropertyChanged
    {
        private readonly int _readDelay = 500;

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
        
        public bool IsConnectedToInternet()
        {
            bool isConnected = NetworkInterface.GetIsNetworkAvailable();

            if(isConnected){
                try{
                    using (TcpClient client = new TcpClient("www.google.com", 80))
                        isConnected = true;
                }
                catch(Exception){ isConnected = false; }
            }

            return isConnected;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private async void OnContentRendered(object sender, EventArgs e)
        {
            ExecuteWindows();
            DebugOsu.UpdateModsText();
            ExecuteLabels();
            _settingsLoaded = true;
            Console.WriteLine("Awaiting internet connection.");
            if (!IsConnectedToInternet()) return;
            await Twitch.Client.ConnectAsync();
            Grid.IsEnabled = true;
            Console.WriteLine("INITIALIZED!");
            while (true) await getOsuData();
            // ReSharper disable once FunctionNeverReturns
        }

        private async void OnClosing(object sender, EventArgs e)
        {
            Settings.Default.Username = ChannelNameBox.Text;
            LogManager.Shutdown();
            Settings.Default.Save();
            BugReport.IsClosing = true;
            DebugOsu.IsClosing = true;
            BugReport.Close();
            DebugOsu.Close();
            if (Twitch.JoinedChannel != null) await Twitch.Client.LeaveChannelAsync(Twitch.JoinedChannel);
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
            TwitchStatus = "Verification Required";
        }

        private async void ConnectionButton_OnClick(object sender, RoutedEventArgs e)
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

            if (TwitchConnect == "Connect") await JoinChannel();
            else await Twitch.Client.LeaveChannelAsync(ChannelNameBox.Text);
        }

        private async void Verify()
        {
            await JoinChannel();
            TwitchConnect = "Abort";
            ChannelNameBox.IsEnabled = false;
            TwitchStatus = "Awaiting verification...";
            await Twitch.Client.SendMessageAsync(ChannelNameBox.Text,
                "Please type '!verify' in order to confirm that you are the owner of the channel.");
            Console.WriteLine(@"Verification message has been sent, awaiting confirmation...");
        }

        private async Task JoinChannel()
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

            if (Twitch.JoinedChannel != null) await Twitch.Client.LeaveChannelAsync(Twitch.JoinedChannel);
            await Twitch.Client.JoinChannelAsync(ChannelNameBox.Text);
        }

        private async void Abort()
        {
            await Twitch.Client.SendMessageAsync(ChannelNameBox.Text, "Verification process aborted.");
            Console.WriteLine(@"Verification process aborted.");
            await Twitch.Client.LeaveChannelAsync(Twitch.JoinedChannel);
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
          //      _sreader.TryRead(BaseAddresses.Skin);
          //      _sreader.TryRead(BaseAddresses.GeneralData);
                if (BaseAddresses.GeneralData.OsuStatus == OsuMemoryStatus.SongSelect)
                    _sreader.TryRead(BaseAddresses.SongSelectionScores);
                else
                    BaseAddresses.SongSelectionScores.Scores.Clear();
                if (BaseAddresses.GeneralData.OsuStatus == OsuMemoryStatus.ResultsScreen)
                    _sreader.TryRead(BaseAddresses.ResultsScreen);
                new UpdateValue().UpdateValues();
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
            if (!Settings.Default.Verified) TwitchStatus = "Verification Required";
            TwitchConnect = "Connect";
            ChannelNameBox.Text = Settings.Default.Username;
        }

        private void BugButton_OnClick(object sender, RoutedEventArgs e)
        {
            BugReport.Show();
        }

        private void DiscordButton_OnClick(object sender, RoutedEventArgs e)
        {
            Process.Start("https://discord.gg/TtSQa944Ky");
        }

        private void TwitchButton_OnClick(object sender, RoutedEventArgs e)
        {
            Process.Start("https://twitch.tv/gadzzaa");
        }
        
        private void DebugButton_OnClick(object sender, RoutedEventArgs e)
        {
            DebugOsu.Show();
        }

        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}