using GadzzaaTB.Classes;
using OsuMemoryDataProvider;
using OsuMemoryDataProvider.OsuMemoryModels;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

// ReSharper disable RedundantCheckBeforeAssignment

// ReSharper disable MemberCanBePrivate.Global

namespace GadzzaaTB.Windows;

public partial class MainPage : INotifyPropertyChanged
{
    private static readonly MainWindow MainWindow = (MainWindow)Application.Current.MainWindow;
    private readonly int _readDelay = 500;

    // ReSharper disable once InconsistentNaming
    public readonly StructuredOsuMemoryReader _sreader;
    public readonly OsuBaseAddresses BaseAddresses = new();
    private string _osuStatus = "Disconnected";
    private bool _settingsLoaded;
    private string _twitchButton = "Loading...";
    private string _twitchStatus = "Loading...";
    private string _channelNameTxt;

    public MainPage()
    {
        InitializeComponent();
        DataContext = this;
        _sreader = StructuredOsuMemoryReader.Instance;
    }

    public string ChannelNameTxt
    {
        get => _channelNameTxt;
        set
        {
            _channelNameTxt = value;
            OnPropertyChanged();
        }
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
        ExecuteLabels();
        _settingsLoaded = true;
        // ReSharper disable once FunctionNeverReturns
    }


    private void ChannelNameBox_OnTextChanged(object sender, TextChangedEventArgs e)
    {
        if (!_settingsLoaded) return;
        Settings.Default.Verified = false;
        MainWindow.SettingsP.AutoC.IsChecked = false;
        TwitchStatus = "Verification Required";
    }

    public void VerifyChannelNameBox()
    {
        var myBindingExpression = ChannelNameBox.GetBindingExpression(TextBox.TextProperty);
        if (myBindingExpression != null)
        {
            var myBinding = myBindingExpression.ParentBinding;
            myBinding.UpdateSourceExceptionFilter = ReturnExceptionHandler;
        }

        myBindingExpression.UpdateSource();
    }

    private async void ConnectionButton_OnClick(object sender, RoutedEventArgs e)
    {
        ValidOnTargetUpd.ValidatesOnTargetUpdated = true;
        VerifyChannelNameBox();
        if (string.IsNullOrWhiteSpace(ChannelNameBox.Text)) return;
        if (TwitchConnect == "Abort")
        {
            Abort();
            ValidOnTargetUpd.ValidatesOnTargetUpdated = false;
            return;
        }

        if (!Settings.Default.Verified)
        {
            Verify();
            ValidOnTargetUpd.ValidatesOnTargetUpdated = false;
            return;
        }

        if (TwitchConnect == "Connect")
        {
            await JoinChannel();
            ValidOnTargetUpd.ValidatesOnTargetUpdated = false;
        }
        else
        {
            await MainWindow.Twitch.Client.LeaveChannelAsync(ChannelNameBox.Text);
            ValidOnTargetUpd.ValidatesOnTargetUpdated = false;
        }
    }

    private object ReturnExceptionHandler(object bindingExpression, Exception exception)
    {
        return "This is from the UpdateSourceExceptionFilterCallBack.";
    }

    public async void Verify()
    {
        await JoinChannel();
        TwitchConnect = "Abort";
        ChannelNameBox.IsEnabled = false;
        TwitchStatus = "Awaiting verification...";
        await MainWindow.Twitch.Client.SendMessageAsync(ChannelNameBox.Text,
            "Please type '!verify' in order to confirm that you are the owner of the channel.");
        Console.WriteLine(@"Verification message has been sent, awaiting confirmation...");
    }

    public async Task JoinChannel()
    {
        if (!MainWindow.Twitch.Client.IsConnected)
        {
            Console.WriteLine(@"Twitch connection error!");
            return;
        }

        if (MainWindow.Twitch.JoinedChannel != null)
            await MainWindow.Twitch.Client.LeaveChannelAsync(MainWindow.Twitch.JoinedChannel);
        await MainWindow.Twitch.Client.JoinChannelAsync(ChannelNameBox.Text);
    }

    private async void Abort()
    {
        await MainWindow.Twitch.Client.SendMessageAsync(ChannelNameBox.Text, "Verification process aborted.");
        Console.WriteLine(@"Verification process aborted.");
        await MainWindow.Twitch.Client.LeaveChannelAsync(MainWindow.Twitch.JoinedChannel);
    }

    public async Task GetOsuData()
    {
        await Task.Delay(_readDelay);
        if (MainWindow.Twitch.JoinedChannel == null) return;
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
        if (MainWindow.Twitch.JoinedChannel != null)
            await MainWindow.Twitch.Client.LeaveChannelAsync(MainWindow.Twitch.JoinedChannel);
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