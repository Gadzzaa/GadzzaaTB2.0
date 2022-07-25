using System;
using System.Globalization;
using System.Threading.Tasks;
using System.Windows;
using GadzzaaTB.Windows;
using TwitchLib.Client;
using TwitchLib.Client.Events;
using TwitchLib.Client.Models;
using TwitchLib.Communication.Clients;
using TwitchLib.Communication.Models;

namespace GadzzaaTB.Classes
{
    public class Bot
    {
        private readonly MainWindow _mainWindow = (MainWindow)Application.Current.MainWindow;
        public TwitchClient Client;
        public string JoinedChannel;

        public Bot()
        {
            var credentials = new ConnectionCredentials("gadzzaaBot", Passwords.twitchOAuth);
            var clientOptions = new ClientOptions
            {
                MessagesAllowedInPeriod = 750,
                ThrottlingPeriod = TimeSpan.FromSeconds(30)
            };
            var customClient = new WebSocketClient(clientOptions);
            Client = new TwitchClient(customClient);
            Client.Initialize(credentials);

            Client.OnLog += Client_OnLog;
            Client.OnJoinedChannel += Client_OnJoinedChannel;
            Client.OnMessageReceived += Client_OnMessageReceived;
            Client.OnConnected += Client_OnConnected;
            Client.OnLeftChannel += ClientOnOnLeftChannel;
        }


        private void Client_OnLog(object sender, OnLogArgs e)
        {
            Console.WriteLine($@"{e.DateTime.ToString(CultureInfo.CurrentCulture)}: {e.BotUsername} - {e.Data}");
        }

        private void Client_OnConnected(object sender, OnConnectedArgs e)
        {
            Console.WriteLine(@"Connected to Twitch!");
        }

        private void Client_OnJoinedChannel(object sender, OnJoinedChannelArgs e)
        {
            JoinedChannel = e.Channel;
            if (!Settings.Default.Verified) return;
            Console.WriteLine(@"Connected to channel: " + e.Channel);
            Client.SendMessage(e.Channel, "Bot online!");
            Task.Factory.StartNew(() =>
            {
                var _ = _mainWindow.Dispatcher.BeginInvoke((Action)(() =>
                {
                    {
                        _mainWindow.TwitchStatus = "Connected";
                        _mainWindow.TwitchConnect = "Disconnect";
                        _mainWindow.ChannelNameBox.IsEnabled = false;
                    }
                }));
            });
        }

        private void ClientOnOnLeftChannel(object sender, OnLeftChannelArgs e)
        {
            Console.WriteLine(@"Left channel: " + e.Channel);
            JoinedChannel = null;
            Task.Factory.StartNew(() =>
            {
                var _ = _mainWindow.Dispatcher.BeginInvoke((Action)(() =>
                {
                    {
                        _mainWindow.TwitchStatus = "Disconnected";
                        _mainWindow.TwitchConnect = "Connect";
                        _mainWindow.ChannelNameBox.IsEnabled = true;
                    }
                }));
            });
        }

        private void Client_OnMessageReceived(object sender, OnMessageReceivedArgs e)
        {
            if (!Settings.Default.Verified)
            {
                if (e.ChatMessage.Message != "!verify") return;
                if (e.ChatMessage.Username != e.ChatMessage.Channel) return;
                Settings.Default.Verified = true;
                _mainWindow.TwitchStatus = "Connected";
                _mainWindow.TwitchConnect = "Disconnect";
                Client.SendMessage(e.ChatMessage.Channel,
                    "Verification process completed! Thank you for using my bot!");
                return;
            }

            if (e.ChatMessage.Message != "!np") return;
            if (!_mainWindow._sreader.CanRead)
            {
                Client.SendMessage(e.ChatMessage.Channel,
                    @"Process 'osu.exe' could not be found running. Please launch the game before using the command");
                return;
            }

            Client.SendMessage(e.ChatMessage.Channel,
                _mainWindow.DebugOsu.mapInfo + " | Mods: " + _mainWindow.DebugOsu.modsText + " | Download: " +
                _mainWindow.DebugOsu.dl);
        }
    }
}