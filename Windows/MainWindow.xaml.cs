using System.ComponentModel;
using System.Windows;
using GadzzaaTB.Pages;
using NLog;

namespace GadzzaaTB.Windows
{
    /// <summary>
    ///     Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public readonly BugReport BugReport;
        public readonly DebugOsu DebugOsu;


        public MainWindow()
        {
            InitializeComponent();
            var main = new Main();
            BugReport = new BugReport();
            DebugOsu = new DebugOsu();
            Closing += OnClosing;
            NavigationFrame.NavigationService.Navigate(main);
        }

        private void OnClosing(object sender, CancelEventArgs e)
        {
            var logger = LogManager.GetLogger("");
            logger.Debug("### LOGGING SESSION FINISHED ###");
            BugReport.IsClosing = true;
            BugReport.Close();
            LogManager.Shutdown();
        }
    }
}