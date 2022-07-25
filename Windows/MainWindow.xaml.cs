﻿using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading;
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
        public BugReport BugReport;
        public DebugOsu DebugOsu;
        
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

        private readonly Logger _logger = LogManager.GetLogger("toPostSharp");
        private readonly MainWindow _mainWindow = (MainWindow)Application.Current.MainWindow;
        private readonly int _readDelay = 33;
        // ReSharper disable once InconsistentNaming
        public readonly StructuredOsuMemoryReader _sreader;
        public readonly OsuBaseAddresses BaseAddresses = new OsuBaseAddresses();
        public Bot Twitch;
        private string _oldText;
        private string _osuStatus = "Loading..";
        private string _twitchButton = "Loading..";
        private string _twitchStatus = "Loading..";
        private bool _settingsLoaded;
        public event PropertyChangedEventHandler PropertyChanged;

        public MainWindow()
        {
            InitializeComponent();
            DataContext = this;
            _sreader = StructuredOsuMemoryReader.Instance;
            ContentRendered += OnContentRendered;
            Closed += OnClosed;
        }

        private async void OnContentRendered(object sender, EventArgs e)
        {
            Twitch = new Bot();
            BugReport = new BugReport();
            DebugOsu = new DebugOsu();
            DebugOsu.UpdateModsText();
            TwitchStatus = "Disconnected";
            if (!Settings.Default.Verified) TwitchStatus = "Un-Verified";
            TwitchConnect = "Connect";
            ChannelNameBox.Text = Settings.Default.Username;
            _settingsLoaded = true;
            Twitch.Client.Connect();
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
            // ReSharper disable once FunctionNeverReturns
        }

        private void OnClosed(object sender, EventArgs e)
        {
            var logger = LogManager.GetLogger("toPostSharp");
            logger.Debug("### LOGGING SESSION FINISHED ###");
            Settings.Default.Username = ChannelNameBox.Text;
            if (Twitch.JoinedChannel != null) Twitch.Client.LeaveChannel(Twitch.JoinedChannel);
            Twitch.Client.Disconnect();
            BugReport.IsClosing = true;
            BugReport.Close();
            DebugOsu.Close();
            LogManager.Shutdown();
            Settings.Default.Save();
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