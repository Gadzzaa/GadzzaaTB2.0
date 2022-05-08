using System.Windows;
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
    }
}