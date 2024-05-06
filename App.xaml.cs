using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Threading.Tasks;
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
        SetupUnhandledExceptionHandling();
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

    private void SetupUnhandledExceptionHandling()
    {
        // Catch exceptions from all threads in the AppDomain.
        AppDomain.CurrentDomain.UnhandledException += (sender, args) =>
            ShowUnhandledException(args.ExceptionObject as Exception, "AppDomain.CurrentDomain.UnhandledException", false);

        // Catch exceptions from each AppDomain that uses a task scheduler for async operations.
        TaskScheduler.UnobservedTaskException += (sender, args) =>
            ShowUnhandledException(args.Exception, "TaskScheduler.UnobservedTaskException", false);
        // Catch exceptions from the main UI dispatcher thread.
        // Typically we only need to catch this OR the Dispatcher.UnhandledException.
        // Handling both can result in the exception getting handled twice.
        //Application.Current.DispatcherUnhandledException += (sender, args) =>
        //{
        //	// If we are debugging, let Visual Studio handle the exception and take us to the code that threw it.
        //	if (!Debugger.IsAttached)
        //	{
        //		args.Handled = true;
        //		ShowUnhandledException(args.Exception, "Application.Current.DispatcherUnhandledException", true);
        //	}
        //};
    }
    void ShowUnhandledException(Exception e, string unhandledExceptionType, bool promptUserForShutdown)
    {
        var messageBoxTitle = $"Unexpected Error Occurred: {unhandledExceptionType}";
        var messageBoxMessage = $"The following exception occurred:\n\n{e}";
        var messageBoxButtons = MessageBoxButton.OK;
        // Let the user decide if the app should die or not (if applicable).
        if (MessageBox.Show(messageBoxMessage, messageBoxTitle, messageBoxButtons) == MessageBoxResult.Yes)
        {
            Application.Current.Shutdown();
        }
    }
}