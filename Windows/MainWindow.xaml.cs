using System;
using GadzzaaTB.Pages;
using NLog;
using PostSharp.Patterns.Diagnostics;
using PostSharp.Patterns.Diagnostics.Backends.NLog;

namespace GadzzaaTB.Windows
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        private readonly Main _page = new Main();

        public MainWindow()
        {
            InitializeComponent();
            NavigationFrame.NavigationService.Navigate(_page);
            Closed += OnClosed;
        }

        private void OnClosed(object sender, EventArgs e)
        {
            NLog.LogManager.Shutdown();
        }
    }
}