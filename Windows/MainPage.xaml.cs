using GadzzaaTB.Classes;
using OsuMemoryDataProvider;
using OsuMemoryDataProvider.OsuMemoryModels;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Navigation;
using TwitchLib.Api.Helix.Models.Search;

// ReSharper disable RedundantCheckBeforeAssignment

// ReSharper disable MemberCanBePrivate.Global

namespace GadzzaaTB.Windows;

public partial class MainPage : INotifyPropertyChanged
{
    private readonly int _readDelay = 500;

    // ReSharper disable once InconsistentNaming
    public readonly StructuredOsuMemoryReader _sreader;
    public readonly OsuBaseAddresses BaseAddresses = new();
    private string _osuStatus = "Disconnected";
    private bool _settingsLoaded;
    private string _twitchButton = "Loading...";
    private string _twitchStatus = "Loading...";
    private string channelNameTxt;
    private static readonly MainWindow _mainWindow = (MainWindow)Application.Current.MainWindow;

    public MainPage()
    {
        InitializeComponent();
        DataContext = this;
        _sreader = StructuredOsuMemoryReader.Instance;
    }
    public String ChannelNameTxt
    {
        get { return channelNameTxt; }
        set { channelNameTxt = value; OnPropertyChanged(); }
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
    

    public void RenderMain()
    {
        ChannelNameBox.Text = "a";
        VerifyChannelNameBox();
        _mainWindow.DebugOsu.UpdateModsText();
        ExecuteLabels();
        _settingsLoaded = true;
        // ReSharper disable once FunctionNeverReturns
    }


    private void ChannelNameBox_OnTextChanged(object sender, TextChangedEventArgs e)
    {
        if (!_settingsLoaded) return;
        Settings.Default.Verified = false;
        TwitchStatus = "Verification Required";
    }
    private void VerifyChannelNameBox()
    {            
        var myBindingExpression = ChannelNameBox.GetBindingExpression(TextBox.TextProperty);
        var myBinding = myBindingExpression.ParentBinding;
        myBinding.UpdateSourceExceptionFilter = ReturnExceptionHandler;
        myBindingExpression.UpdateSource();
        
    }

    private async void ConnectionButton_OnClick(object sender, RoutedEventArgs e)
    {
        asd.ValidatesOnTargetUpdated = true;
        VerifyChannelNameBox();
        if (string.IsNullOrWhiteSpace(ChannelNameBox.Text)) return;
        if (TwitchConnect == "Abort")
        {
            Abort();
            asd.ValidatesOnTargetUpdated = false;
            return;
        }

        if (!Settings.Default.Verified)
        {
            Verify();
            asd.ValidatesOnTargetUpdated = false;
            return;
        }

        if (TwitchConnect == "Connect")
        {
            await JoinChannel();
            asd.ValidatesOnTargetUpdated = false;
        }
        else await _mainWindow.Twitch.Client.LeaveChannelAsync(ChannelNameBox.Text);
        asd.ValidatesOnTargetUpdated = false;
    }

    private object ReturnExceptionHandler(object bindingExpression, Exception exception) => "This is from the UpdateSourceExceptionFilterCallBack.";

    private async void Verify()
    {
        await JoinChannel();
        TwitchConnect = "Abort";
        ChannelNameBox.IsEnabled = false;
        TwitchStatus = "Awaiting verification...";
        await _mainWindow.Twitch.Client.SendMessageAsync(ChannelNameBox.Text,
            "Please type '!verify' in order to confirm that you are the owner of the channel.");
        Console.WriteLine(@"Verification message has been sent, awaiting confirmation...");
    }

    private async Task JoinChannel()
    {
        if (!_mainWindow.Twitch.Client.IsConnected)
        {
            Console.WriteLine(@"Twitch connection error!");
            return;
        }

        if (_mainWindow.Twitch.JoinedChannel != null) await _mainWindow.Twitch.Client.LeaveChannelAsync(_mainWindow.Twitch.JoinedChannel);
        await _mainWindow.Twitch.Client.JoinChannelAsync(ChannelNameBox.Text);
    }

    private async void Abort()
    {
        await _mainWindow.Twitch.Client.SendMessageAsync(ChannelNameBox.Text, "Verification process aborted.");
        Console.WriteLine(@"Verification process aborted.");
        await _mainWindow.Twitch.Client.LeaveChannelAsync(_mainWindow.Twitch.JoinedChannel);
    }

    public async Task GetOsuData()
    {
        await Task.Delay(_readDelay);
        if (_mainWindow.Twitch.JoinedChannel == null) return;
        if (!_sreader.CanRead)
        {
            if (OsuStatus != "Process not found!") OsuStatus = "Process not found!";
            await Task.Delay(_readDelay);
        }
        else
        {
            if (OsuStatus != "Running") OsuStatus = "Running";
            _sreader.TryRead(BaseAddresses.Beatmap);
            //      _sreader.TryRead(BaseAddresses.Skin);
            _sreader.TryRead(BaseAddresses.GeneralData);
            if (BaseAddresses.GeneralData.OsuStatus == OsuMemoryStatus.SongSelect)
                _sreader.TryRead(BaseAddresses.SongSelectionScores);
            else
                BaseAddresses.SongSelectionScores.Scores.Clear();
            if (BaseAddresses.GeneralData.OsuStatus == OsuMemoryStatus.ResultsScreen)
                _sreader.TryRead(BaseAddresses.ResultsScreen);
            new UpdateValue().UpdateValues();
        }
    }
    

    private void ExecuteLabels()
    {
        TwitchStatus = "Disconnected";
        if (!Settings.Default.Verified) TwitchStatus = "Verification Required";
        TwitchConnect = "Connect";
        ChannelNameBox.Text = Settings.Default.Username;
    }

    public async void Disconnected()
    {
        Settings.Default.Verified = false;
        if (_mainWindow.Twitch.JoinedChannel != null) await _mainWindow.Twitch.Client.LeaveChannelAsync(_mainWindow.Twitch.JoinedChannel);
        TwitchStatus = "Disconnected";
        if (!Settings.Default.Verified) TwitchStatus = "Verification Required";
        TwitchConnect = "Connect";
        OsuStatus = "Disconncted";
    }
    

    private void Grid_OnMouseDown(object sender, MouseButtonEventArgs mouseButtonEventArgs)
    {
        Grid.Focus();
    }


    private void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
