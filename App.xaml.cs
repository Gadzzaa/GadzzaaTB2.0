using GadzzaaTB.Windows;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows;
using Forms = System.Windows.Forms;


namespace GadzzaaTB;

/// <summary>
///     Interaction logic for App.xaml
/// </summary>
[SuppressMessage("Interoperability", "CA1416:Validate platform compatibility")]
public partial class App
{
    public readonly Forms.NotifyIcon NotifyIcon;
    private MainWindow _mainW;

    public App()
    {
        SetupUnhandledExceptionHandling();
        NotifyIcon = new Forms.NotifyIcon();
        Startup += OnStartup;
    }

    public static App Me => (App)Current;

    private void OnStartup(object sender, StartupEventArgs e)
    {
        NotifyIcon.Icon = new Icon(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)
                                    + "../../../../Images/gadzzaa_Dq1_icon.ico"); // TODO: Find simpler way to do this
        NotifyIcon.Text = @"GadzzaaTB";
        NotifyIcon.DoubleClick += NotifyIconOnClick;

        NotifyIcon.ContextMenuStrip = new Forms.ContextMenuStrip();
        NotifyIcon.ContextMenuStrip.Items.Add("Quit", null, OnClick);
        NotifyIcon.Visible = false;

        _mainW = new MainWindow();
    }

    private void OnClick(object sender, EventArgs e)
    {
        _mainW.ClosingApp = true;
        _mainW.Close();
    }

    private void NotifyIconOnClick(object sender, EventArgs e)
    {
        MainWindow.Show();
        NotifyIcon.Visible = false;
    }

    private void SetupUnhandledExceptionHandling()
    {
        // Catch exceptions from all threads in the AppDomain.
        AppDomain.CurrentDomain.UnhandledException += (_, args) =>
            ShowUnhandledException(args.ExceptionObject as Exception, "AppDomain.CurrentDomain.UnhandledException");

        // Catch exceptions from each AppDomain that uses a task scheduler for async operations.
        TaskScheduler.UnobservedTaskException += (_, args) =>
            ShowUnhandledException(args.Exception, "TaskScheduler.UnobservedTaskException");
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

    private void ShowUnhandledException(Exception e, string unhandledExceptionType)
    {
        var messageBoxTitle = $"Unexpected Error Occurred: {unhandledExceptionType}";
        var messageBoxMessage = $"The following exception occurred:\n\n{e}";
        var messageBoxButtons = MessageBoxButton.OK;
        // Let the user decide if the app should die or not (if applicable).
        if (MessageBox.Show(messageBoxMessage, messageBoxTitle, messageBoxButtons) == MessageBoxResult.Yes)
            Current.Shutdown();
    }
}