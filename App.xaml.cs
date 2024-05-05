using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Windows;
using System.Windows.Threading;
using GadzzaaTB.Windows;
using TwitchLib.Api.Helix.Models.ChannelPoints;
using Forms=System.Windows.Forms;


namespace GadzzaaTB;

/// <summary>
///     Interaction logic for App.xaml
/// </summary>
[SuppressMessage("Interoperability", "CA1416:Validate platform compatibility")]
public partial class App : Application
{
    public static App Me => ((App) Application.Current);

    public readonly Forms.NotifyIcon _notifyIcon;
    private MainWindow mainW;

    public App()
    {
        _notifyIcon = new Forms.NotifyIcon();
        Startup += OnStartup;
    }

    private void OnStartup(object sender, StartupEventArgs e)
    {
        _notifyIcon.Icon = new System.Drawing.Icon(    System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location)
        + "../../../../Images/gadzzaa_Dq1_icon.ico"); // TODO: Find simpler way to do this
        _notifyIcon.Text = "GadzzaaTB";
        _notifyIcon.DoubleClick += NotifyIconOnClick;

        _notifyIcon.ContextMenuStrip = new Forms.ContextMenuStrip();
        _notifyIcon.ContextMenuStrip.Items.Add("Quit", null, OnClick);
        _notifyIcon.Visible = false;
        
        mainW = new MainWindow();
    }

    private async void OnClick(object sender, EventArgs e)
    {
        mainW.closing = true;
        mainW.Close();
    }

    private void NotifyIconOnClick(object sender, EventArgs e)
    {
        MainWindow.Show();
        _notifyIcon.Visible = false;
    }

    private void Application_DispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
    {
        MessageBox.Show(
            e.Exception.Message + @" A log file has been created. Check '%localappdata%\GadzzaaTB\logs'",
            "Exception Occured!", MessageBoxButton.OK, MessageBoxImage.Error);
        e.Handled = false; //TO:DO ADD TRUE 
        Current.Shutdown();
    }
}