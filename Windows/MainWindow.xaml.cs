using System;
using System.Diagnostics;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using GadzzaaTB.Classes;

namespace GadzzaaTB.Windows;

public partial class MainWindow : Window
{
    public MainPage Main;
    public SettingsPage SettingsP;
    public DebugOsu DebugOsu;
    public BugReport BugReport;
    public Bot Twitch;
    public MainWindow()
    {
        InitializeComponent();
        ContentRendered += OnContentRendered;
        Closing += OnClosing;
    }
    private async void OnContentRendered(object sender, EventArgs eventArgs)
    {
        ExecuteWindows();
        Console.WriteLine(@"Awaiting internet connection.");
        if (!IsConnectedToInternet()) return;
        await Twitch.Client.ConnectAsync();
        Grid.IsEnabled = true;
        Main.RenderMain();
        BugReport.RenderBugReport();
        Console.WriteLine(@"INITIALIZED!");
        while (true) await Main.GetOsuData();
        // ReSharper disable once FunctionNeverReturns
    }
    private async void OnClosing(object sender, EventArgs e)
    {
        Settings.Default.Username = Main.ChannelNameBox.Text;
        Settings.Default.Save();
        if (Twitch.JoinedChannel != null) await Twitch.Client.LeaveChannelAsync(Twitch.JoinedChannel);
    }
    private void ExecuteWindows()
    {
        Main = new MainPage();
        Twitch = new Bot();
        BugReport = new BugReport();
        DebugOsu = new DebugOsu();
        SettingsP = new SettingsPage();
        frame.NavigationService.Navigate(Main);
    }

    private void HomeButton_OnClick(object sender, RoutedEventArgs e)
    {
        frame.NavigationService.Navigate(Main);
    }

    private void SettingsButton_OnClick(object sender, RoutedEventArgs e)
    {
        MenuToggler.IsChecked = false;
        frame.NavigationService.Navigate(SettingsP);
    }
    
    private void BugButton_OnClick(object sender, RoutedEventArgs e)
    {
        MenuToggler.IsChecked = false;
        frame.NavigationService.Navigate(BugReport);
    }

    private void DebugButton_OnClick(object sender, RoutedEventArgs e)
    {
        MenuToggler.IsChecked = false;
        frame.NavigationService.Navigate(DebugOsu);
    }
    

    public bool IsConnectedToInternet()
    {
        var isConnected = NetworkInterface.GetIsNetworkAvailable();

        if (isConnected)
            try
            {
                using (new TcpClient("www.google.com", 80))
                {
                    isConnected = true;
                }
            }
            catch (Exception)
            {
                isConnected = false;
            }

        return isConnected;
    }
    private void DiscordButton_OnClick(object sender, RoutedEventArgs e)
    {
        MenuToggler.IsChecked = false;
        var psi = new ProcessStartInfo
        {
            FileName = "https://discord.gg/TtSQa944Ky",
            UseShellExecute = true
        };
        Process.Start(psi);
    }

    private void TwitchButton_OnClick(object sender, RoutedEventArgs e)
    {
        MenuToggler.IsChecked = false;

        var psi = new ProcessStartInfo
        {
            FileName = "https://twitch.tv/gadzzaa",
            UseShellExecute = true
        };
        Process.Start(psi);
    }

    private void GithubButton_OnClick(object sender, RoutedEventArgs e)
    {
        MenuToggler.IsChecked = false;

        var psi = new ProcessStartInfo
        {
            FileName = "https://github.com/Gadzzaa/GadzzaaTB2.0",
            UseShellExecute = true
        };
        Process.Start(psi);
    }

    private void YoutubeButton_OnClick(object sender, RoutedEventArgs e)
    {
        MenuToggler.IsChecked = false;

        var psi = new ProcessStartInfo
        {
            FileName = "https://www.youtube.com/@gadzzaa",
            UseShellExecute = true
        };
        Process.Start(psi);
    }
}