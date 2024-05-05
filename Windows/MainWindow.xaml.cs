using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using GadzzaaTB.Classes;

namespace GadzzaaTB.Windows;

public partial class MainWindow : Window, INotifyPropertyChanged
{
    public BugReport BugReport;
    public DebugOsu DebugOsu;
    public MainPage Main;
    public SettingsPage SettingsP;
    public Bot Twitch;
    public bool closing = false;

    public MainWindow()
    {
        InitializeComponent();
        DataContext = this;
        ContentRendered += OnContentRendered;
        Closing += OnClosing;
    }


    private async void OnContentRendered(object sender, EventArgs eventArgs)
    {
        ExecuteWindows();
        Console.WriteLine(@"Awaiting internet connection.");
        if (!IsConnectedToInternet()) return;
        await Twitch.Client.ConnectAsync();
        Main.RenderMain();
        BugReport.RenderBugReport();
        Loading.Visibility = Visibility.Collapsed;
        await Task.Delay(100);
        frame.NavigationService.Navigate(Main);
        Grid.IsEnabled = true;
        Console.WriteLine(@"INITIALIZED!");
        if (Settings.Default.AutoConnect) await Main.JoinChannel();
        while (true) await Main.GetOsuData();
        // ReSharper disable once FunctionNeverReturns
    }

    private async void OnClosing(object sender, CancelEventArgs e)
    {
        if (Settings.Default.MinimizeX && !closing)
        {
            App.Me._notifyIcon.Visible = true;
            Hide();
            e.Cancel = true;
        }
        Settings.Default.Username = Main.ChannelNameBox.Text;
        Settings.Default.Save();
        if (Twitch.JoinedChannel != null) await Twitch.Client.LeaveChannelAsync(Twitch.JoinedChannel);    }

    private void ExecuteWindows()
    {
        Main = new MainPage();
        Twitch = new Bot();
        BugReport = new BugReport();
        DebugOsu = new DebugOsu();
        SettingsP = new SettingsPage();
    }

    private async void HomeButton_OnClick(object sender, RoutedEventArgs e)
    {
        await Task.Delay(100);
        frame.NavigationService.Navigate(Main);
    }

    private async void SettingsButton_OnClick(object sender, RoutedEventArgs e)
    {
        await Task.Delay(100);
        MenuToggler.IsChecked = false;
        frame.NavigationService.Navigate(SettingsP);
    }

    private async void BugButton_OnClick(object sender, RoutedEventArgs e)
    {
        await Task.Delay(100);
        MenuToggler.IsChecked = false;
        frame.NavigationService.Navigate(BugReport);
    }

    private async void DebugButton_OnClick(object sender, RoutedEventArgs e)
    {
        await Task.Delay(100);
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
    public event PropertyChangedEventHandler PropertyChanged;
    private void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}