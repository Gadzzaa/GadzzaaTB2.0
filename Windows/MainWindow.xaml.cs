using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
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

        public MainWindow()
        {
            InitializeComponent();
            var main = new Main();
            BugReport = new BugReport();
            Closing += OnClosing;
            NavigationFrame.NavigationService.Navigate(main);
        }

        private void OnClosing(object sender, CancelEventArgs e)
        {
            BugReport.IsClosing = true;
            BugReport.Close();
            LogManager.Shutdown();
        }
    }
}