using System;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using GadzzaaTB.Windows;
using OsuMemoryDataProvider;
using OsuMemoryDataProvider.OsuMemoryModels;

namespace GadzzaaTB.Pages
{
    public partial class Main : Page
    {
        private static readonly OsuBaseAddresses BaseAddresses = new OsuBaseAddresses();
        private readonly int _readDelay = 33;
        private readonly StructuredOsuMemoryReader _sreader;
        public readonly MainWindow MainWindow = (MainWindow) Application.Current.MainWindow;
        private string _oldText;


        public Main()
        {
            InitializeComponent();
            //   _sreader = StructuredOsuMemoryReader.Instance.GetInstanceForWindowTitleHint(osuWindowsTitle);
            //  Initialized += OnInitialized;
        }

        private async void OnInitialized(object sender, EventArgs e)
        {
            if (!_sreader.CanRead)
            {
                OsuConnection.Text = "Process not found";
                await Task.Delay(_readDelay);
            }

            _sreader.TryRead(BaseAddresses.GeneralData);
            if (BaseAddresses.GeneralData.OsuStatus != OsuMemoryStatus.NotRunning)
                OsuConnection.Text = "Online";
            await Task.Delay(_readDelay);
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

        private void BugButton_OnClick(object sender, RoutedEventArgs e)
        {
            MainWindow.BugReport.Show();
        }

        private void DiscordButton_OnClick(object sender, RoutedEventArgs e)
        {
            Process.Start("https://discord.gg/TtSQa944Ky");
        }

        private void TwitchButton_OnClick(object sender, RoutedEventArgs e)
        {
            Process.Start("https://twitch.tv/gadzzaa");
        }

        private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
        {
            string s = null;
            s.Trim();
        }
    }
}