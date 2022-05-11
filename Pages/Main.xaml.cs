using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;
using GadzzaaTB.Windows;
using NLog;
using OsuMemoryDataProvider;
using OsuMemoryDataProvider.OsuMemoryModels;

// ReSharper disable FunctionNeverReturns
// ReSharper disable RedundantCheckBeforeAssignment
// ReSharper disable MemberCanBePrivate.Global

namespace GadzzaaTB.Pages
{
    public sealed partial class Main : INotifyPropertyChanged
    {
        private static readonly OsuBaseAddresses BaseAddresses = new OsuBaseAddresses();
        private readonly Logger _logger = LogManager.GetLogger("toPostSharp");
        private readonly MainWindow _mainWindow = (MainWindow) Application.Current.MainWindow;
        private readonly int _readDelay = 33;
        private readonly StructuredOsuMemoryReader _sreader;
        private string _oldText;
        private string _osuStatus;

        public Main()
        {
            InitializeComponent();
            DataContext = this;
            _sreader = StructuredOsuMemoryReader.Instance;
            Loaded += OnLoaded;
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

        public event PropertyChangedEventHandler PropertyChanged;

        private async void OnLoaded(object sender, RoutedEventArgs e)
        {
            while (true)
            {
                _logger.Info("INITIALIZED!");
                if (!_sreader.CanRead)
                {
                    OsuStatus = "Process not found!";
                    await Task.Delay(_readDelay);
                }
                else
                {
                    OsuStatus = "Running";
                    _sreader.TryRead(BaseAddresses.GeneralData);
                    await Task.Delay(_readDelay);
                }
            }
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
            _mainWindow.BugReport.Show();
        }

        private void DiscordButton_OnClick(object sender, RoutedEventArgs e)
        {
            Process.Start("https://discord.gg/TtSQa944Ky");
        }

        private void TwitchButton_OnClick(object sender, RoutedEventArgs e)
        {
            Process.Start("https://twitch.tv/gadzzaa");
        }

        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}