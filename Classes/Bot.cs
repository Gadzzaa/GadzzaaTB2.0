using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using GadzzaaTB.Windows;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using TwitchLib.Client;
using TwitchLib.Client.Events;
using TwitchLib.Client.Models;
using TwitchLib.Communication.Events;
using OnConnectedEventArgs = TwitchLib.Client.Events.OnConnectedEventArgs;

namespace GadzzaaTB.Classes
{
    public class Bot
    {
        private readonly MainWindow? _mainWindow = (MainWindow)Application.Current.MainWindow;
        public readonly TwitchClient Client;
        public string? JoinedChannel;

        public Bot()
        {
            var loggerFactory = LoggerFactory.Create(c => c.Services.AddLogging());
            Client = new TwitchClient(loggerFactory: loggerFactory);
            var credentials = new ConnectionCredentials("gadzzaaBot", Passwords.twitchOAuth);
            Client.Initialize(credentials);
            
            Client.OnConnected += Client_OnConnected;
            Client.OnJoinedChannel += Client_OnJoinedChannel;
            Client.OnMessageReceived += Client_OnMessageReceived;
            Client.OnLeftChannel += ClientOnOnLeftChannel;
            Client.OnDisconnected += ClientOnOnDisconnected;
            Client.OnReconnected += ClientOnOnReconnected;
            
        }

        async Task ClientOnOnReconnected(object? sender, OnConnectedEventArgs onConnectedEventArgs)
        {
            if (JoinedChannel != null) await Client.JoinChannelAsync(JoinedChannel);
        }

        async Task ClientOnOnDisconnected(object? sender, OnDisconnectedEventArgs e)
        {
            if (_mainWindow != null && _mainWindow.BugReport.IsClosing) return;
            await Client.ReconnectAsync();
            Thread.Sleep(5000);
        }

        Task Client_OnConnected(object? sender, OnConnectedEventArgs onConnectedEventArgs)
        {
            Console.WriteLine(@"Connected to Twitch!");
            return Task.CompletedTask;
        }

        async Task Client_OnJoinedChannel(object? sender, OnJoinedChannelArgs e)
        {
            JoinedChannel = e.Channel;
            if (!Settings.Default.Verified) return;
            Console.WriteLine(@"Connected to channel: " + e.Channel);
            await Client.SendMessageAsync(e.Channel, "Bot online!");
            await Task.Factory.StartNew(() =>
            {
                if (_mainWindow != null)
                {
                    var _ = _mainWindow.Dispatcher.BeginInvoke((Action)(() =>
                    {
                        {
                            _mainWindow.TwitchStatus = "Connected";
                            _mainWindow.TwitchConnect = "Disconnect";
                            _mainWindow.ChannelNameBox.IsEnabled = false;
                        }
                    }));
                }
            });
        }

        Task ClientOnOnLeftChannel(object? sender, OnLeftChannelArgs e)
        {
            Console.WriteLine(@"Left channel: " + e.Channel);
            JoinedChannel = null;
            Task.Factory.StartNew(() =>
            {
                if (_mainWindow != null)
                {
                    var _ = _mainWindow.Dispatcher.BeginInvoke((Action)(() =>
                    {
                        {
                            _mainWindow.TwitchStatus = "Disconnected";
                            _mainWindow.TwitchConnect = "Connect";
                            _mainWindow.ChannelNameBox.IsEnabled = true;
                        }
                    }));
                }
            });
            return Task.CompletedTask;
        }

        async Task Client_OnMessageReceived(object? sender, OnMessageReceivedArgs e)
        {
            if (!Settings.Default.Verified && e.ChatMessage.Message == "!verify" && e.ChatMessage.Username == e.ChatMessage.Channel)
            {
                Settings.Default.Verified = true;
                if (_mainWindow != null)
                {
                    _mainWindow.TwitchStatus = "Connected";
                    _mainWindow.TwitchConnect = "Disconnect";
                }

                await Client.SendMessageAsync(e.ChatMessage.Channel,
                    "Verification process completed! Thank you for using my bot!");
                return;
            }

            if (_mainWindow != null && e.ChatMessage.Message == "!np" && !_mainWindow._sreader.CanRead) 
            {
                await Client.SendMessageAsync(e.ChatMessage.Channel,
                    @"Process 'osu.exe' could not be found running. Please launch the game before using the command");
                return;
            }

            if (_mainWindow != null)
                await Client.SendMessageAsync(e.ChatMessage.Channel,
                   _mainWindow.DebugOsu.mStars + "\u2b50 | " +  _mainWindow.DebugOsu.mapInfo + " | Mods: " + _mainWindow.DebugOsu.modsText + " | Download: " +
                    _mainWindow.DebugOsu.dl);
        }
    }
}