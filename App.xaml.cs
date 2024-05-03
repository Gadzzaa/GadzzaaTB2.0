using System.Windows;
using System.Windows.Threading;
using Castle.Windsor;
using GadzzaaTB.Classes;
using GadzzaaTB.Windows;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Serilog;
using ILogger = Serilog.ILogger;


namespace GadzzaaTB;

/// <summary>
///     Interaction logic for App.xaml
/// </summary>
public partial class App : Application
{
    private void Application_DispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
    {
        MessageBox.Show(
            e.Exception.Message + @" A log file has been created. Check '%localappdata%\GadzzaaTB\logs'",
            "Exception Occured!", MessageBoxButton.OK, MessageBoxImage.Error);
        e.Handled = false; //TO:DO ADD TRUE 
        Current.Shutdown();
    }
}