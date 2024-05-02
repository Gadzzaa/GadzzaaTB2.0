using System;
using System.Windows;
using System.Windows.Threading;
using Microsoft.Extensions.Logging;


namespace GadzzaaTB;

/// <summary>
///     Interaction logic for App.xaml
/// </summary>
public partial class App : Application
{    
    private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

    public App()
    {
        Logger.Trace("Neata");
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