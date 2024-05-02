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

namespace GadzzaaTB.Classes;

public class Bot
{
    private readonly MainWindow _mainWindow = (MainWindow)Application.Current.MainWindow;
    public readonly TwitchClient Client;
    public string JoinedChannel;

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

    private async Task ClientOnOnReconnected(object sender, OnConnectedEventArgs onConnectedEventArgs)
    {
        if (JoinedChannel != null) await Client.JoinChannelAsync(JoinedChannel);
    }

    private async Task ClientOnOnDisconnected(object sender, OnDisconnectedEventArgs e)
    {
        await Client.ReconnectAsync();
        Thread.Sleep(5000);
    }

    private Task Client_OnConnected(object sender, OnConnectedEventArgs onConnectedEventArgs)
    {
        Console.WriteLine(@"Connected to Twitch!");
        return Task.CompletedTask;
    }

    private async Task Client_OnJoinedChannel(object sender, OnJoinedChannelArgs e)
    {
        JoinedChannel = e.Channel;
        if (!Settings.Default.Verified) return;
        Console.WriteLine(@"Connected to channel: " + e.Channel);
        await Client.SendMessageAsync(e.Channel, "Bot online!");
        await Task.Factory.StartNew(() =>
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

    private Task ClientOnOnLeftChannel(object sender, OnLeftChannelArgs e)
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
        return Task.CompletedTask;
    }

    private async Task Client_OnMessageReceived(object sender, OnMessageReceivedArgs e)
    {
        Console.WriteLine(Settings.Default.Verified);
        Console.WriteLine(e.ChatMessage.Message);
        Console.WriteLine(e.ChatMessage.Username);
        Console.WriteLine(e.ChatMessage.Channel);
        if (!Settings.Default.Verified)
        {
            if (e.ChatMessage.Message != "!verify") return;
            if (e.ChatMessage.Username != e.ChatMessage.Channel) return;
            Verifica();
            await Client.SendMessageAsync(e.ChatMessage.Channel,
                "Verification process completed! Thank you for using my bot!");
            return;
        }

        if (e.ChatMessage.Message == "!np")
        {
            if (_mainWindow.DebugOsu == null) return;
            if (!_mainWindow._sreader.CanRead)
            {
                await Client.SendMessageAsync(e.ChatMessage.Channel,
                    @"Process 'osu.exe' could not be found running. Please launch the game before using the command");
                return;
            }

            await Client.SendMessageAsync(e.ChatMessage.Channel,
                _mainWindow.DebugOsu.mStars + "\u2b50 | " + _mainWindow.DebugOsu.mapInfo + " | Mods: " +
                _mainWindow.DebugOsu.modsText + " | Download: " +
                _mainWindow.DebugOsu.dl);
        }
    }

    private void Verifica()
    {
        Settings.Default.Verified = true;
        _mainWindow.TwitchStatus = "Connected";
        _mainWindow.TwitchConnect = "Disconnect";
    }
}