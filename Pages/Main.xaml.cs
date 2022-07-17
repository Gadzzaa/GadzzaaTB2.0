using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;
using GadzzaaTB.Classes;
using GadzzaaTB.Windows;
using NLog;
using OsuMemoryDataProvider;
using OsuMemoryDataProvider.OsuMemoryModels;
using TwitchLib.Api.V5.Models.UploadVideo;

// ReSharper disable FunctionNeverReturns
// ReSharper disable RedundantCheckBeforeAssignment
// ReSharper disable MemberCanBePrivate.Global

namespace GadzzaaTB.Pages
{
    public sealed partial class Main : INotifyPropertyChanged
    {
        private readonly Logger _logger = LogManager.GetLogger("toPostSharp");
        private readonly MainWindow _mainWindow = (MainWindow) Application.Current.MainWindow;
        private readonly int _readDelay = 33;
        private readonly StructuredOsuMemoryReader _sreader;
        public readonly Bot twitch;
        public readonly OsuBaseAddresses BaseAddresses = new OsuBaseAddresses();
        private string _oldText;
        private string _osuStatus;
        private string _twitchStatus;
        private string _twitchButton;

        public Main()
        {
            InitializeComponent();
            DataContext = this;
            _sreader = StructuredOsuMemoryReader.Instance;
            twitch = new Bot();
            Loaded += OnLoaded;
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

        private async void OnLoaded(object sender, RoutedEventArgs e)
        {
            TwitchStatus = "Disconnected";
            TwitchConnect = "Connect";
            while (true)
            {
                _logger.Info("INITIALIZED!");
                if (!_sreader.CanRead)
                {
                    OsuStatus = "Process not found!";
                    await Task.Delay(_readDelay);
                }
                else
                {
                    OsuStatus = "Running";
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

        private void ConnectionButton_OnClick(object sender, RoutedEventArgs e)
        {
            if (TwitchConnect == "Connect") JoinChannel();
            else twitch.Client.LeaveChannel(ChannelNameBox.Text);
        }

        private void JoinChannel()
        {
            if (ChannelNameBox.Text == "Channel Name") {Console.WriteLine(@"Channel Name invalid!"); return;}
            if (!twitch.Client.IsConnected) {Console.WriteLine(@"Twitch connection error!"); return;}
            if(twitch.JoinedChannel != null) twitch.Client.LeaveChannel(twitch.JoinedChannel);
            twitch.Client.JoinChannel(ChannelNameBox.Text);
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