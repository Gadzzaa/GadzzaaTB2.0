using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using GadzzaaTB.Windows;

namespace GadzzaaTB.Pages
{
    public partial class Main : Page
    {
        private string _oldText;
        public readonly MainWindow MainWindow = (MainWindow)Application.Current.MainWindow;
        public Main()
        {
            InitializeComponent();
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
    }
}