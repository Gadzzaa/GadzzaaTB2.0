using System.Windows;
using System.Windows.Threading;
using GadzzaaTB.Classes;

namespace GadzzaaTB
{
    /// <summary>
    ///     Interaction logic for App.xaml
    /// </summary>
    public partial class App
    {
        public App()
        {
            Startup += OnStartup;
        }

        private static void OnStartup(object sender, StartupEventArgs e)
        {
            PostSharpAspects.StartNLog();
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
}