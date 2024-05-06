using Microsoft.Win32;
using System;
using System.IO;
using System.Reflection;
using System.Windows;

namespace GadzzaaTB.Windows;

public partial class SettingsPage
{
    public SettingsPage()
    {
        InitializeComponent();
        if (!Settings.Default.Verified) AutoC.IsChecked = false;
    }

    private void WindowsStart_OnClick(object sender, RoutedEventArgs e)
    {
        if (WindowsStart.IsChecked != null && WindowsStart.IsChecked.Value)
        {
            var path = @"SOFTWARE\Microsoft\Windows\CurrentVersion\Run";
            var key = Registry.CurrentUser.OpenSubKey(path, true);
            if (key != null)
                key.SetValue("GadzzaaTB",
                    Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\GadzzaaTB.exe");
        }
        else
        {
            var path = @"SOFTWARE\Microsoft\Windows\CurrentVersion\Run";
            var key = Registry.CurrentUser.OpenSubKey(path, true);
            if (key != null) key.DeleteValue("GadzzaaTB", false);
        }
    }

    private void ThrowException_OnClick(object sender, RoutedEventArgs e)
    {
        throw new InvalidOperationException("This is a test exception.");
    }
}