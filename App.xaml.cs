using System.Windows;
using System.Windows.Threading;

namespace GadzzaaTB
{
    /// <summary>
    ///     Interaction logic for App.xaml
    /// </summary>
    public partial class App
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
}