using GadzzaaTB.Windows;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;
using TwitchLib.Client;
using TwitchLib.Client.Events;
using TwitchLib.Client.Models;
using TwitchLib.Communication.Events;
using Application = System.Windows.Application;
using OnConnectedEventArgs = TwitchLib.Client.Events.OnConnectedEventArgs;

namespace GadzzaaTB.Classes;

public class Bot
{
    private static readonly MainWindow MainWindow = (MainWindow)Application.Current.MainWindow;
    public readonly TwitchClient Client;
    private readonly MainPage _main = MainWindow.Main;
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
            var _ = MainWindow.Dispatcher.BeginInvoke((Action)(() =>
            {
                {
                    _main.TwitchStatus = "Connected";
                    _main.TwitchConnect = "Disconnect";
                    _main.ChannelNameBox.IsEnabled = false;
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
            var _ = _main.Dispatcher.BeginInvoke((Action)(() =>
            {
                {
                    _main.TwitchStatus = "Disconnected";
                    _main.TwitchConnect = "Connect";
                    _main.OsuStatus = "Disconnected";
                    _main.ChannelNameBox.IsEnabled = true;
                }
            }));
        });
        return Task.CompletedTask;
    }

    private async Task Client_OnMessageReceived(object sender, OnMessageReceivedArgs e)
    {
        if (e.ChatMessage.Message[0] != '!') return;

        if (e.ChatMessage.Message == "!verify")
        {
            if (Settings.Default.Verified)
            {
                await Client.SendMessageAsync(e.ChatMessage.Channel,
                    "You are already verified! If you wish to unlink, use the command: '!unlink'.");
                return;
            }

            if (e.ChatMessage.Username != e.ChatMessage.Channel)
            {
                await Client.SendMessageAsync(e.ChatMessage.Channel,
                    "The owner of the channel must execute this command.");
                return;
            }

            Verifica();
            await Client.SendMessageAsync(e.ChatMessage.Channel,
                "Verification process completed! Thank you for using my bot!");
            return;
        }

        if (e.ChatMessage.Message == "!commands" || e.ChatMessage.Message == "!help")
        {
            await Client.SendMessageAsync(e.ChatMessage.Channel,
                "This is the list of the available commands: !help, !commands, !verify, !unlink, !np\nIf you require assistance setting the bot up, i recommend checking my Discord server: https://discord.com/invite/TtSQa944Ky");
            return;
        }

        if (e.ChatMessage.Message == "!unlink")
        {
            if (!Settings.Default.Verified)
            {
                await Client.SendMessageAsync(e.ChatMessage.Channel,
                    "You are not verified! Please use '!verify' in order to access other commands.");
                return;
            }

            if (e.ChatMessage.Username != e.ChatMessage.Channel)
            {
                await Client.SendMessageAsync(e.ChatMessage.Channel,
                    "The owner of the channel must execute this command.");
                return;
            }

            await Client.SendMessageAsync(e.ChatMessage.Channel,
                "Succesfully unlinked!");
            _main.Disconnected();
        }

        if (e.ChatMessage.Message == "!np")
        {
            if (!Settings.Default.Verified)
            {
                await Client.SendMessageAsync(e.ChatMessage.Channel,
                    "You are not verified! Please use '!verify' in order to access other commands.");
                return;
            }

            if (MainWindow.DebugOsu == null) return;
            if (!_main._sreader.CanRead)
            {
                await Client.SendMessageAsync(e.ChatMessage.Channel,
                    @"Process 'osu.exe' could not be found running. Please launch the game before using the command");
                return;
            }

            await Client.SendMessageAsync(e.ChatMessage.Channel,
                MainWindow.DebugOsu.mStars + "\u2b50 | " + MainWindow.DebugOsu.mapInfo + " | Mods: " +
                MainWindow.DebugOsu.modsText + " | Download: " +
                MainWindow.DebugOsu.dl);
        }
    }

    private void Verifica()
    {
        Settings.Default.Verified = true;
        _main.TwitchStatus = "Connected";
        _main.TwitchConnect = "Disconnect";
    }
}