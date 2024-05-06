using Microsoft.Win32;
using System;
using System.IO;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;

namespace GadzzaaTB.Windows;

public partial class SettingsPage : Page
{
    public SettingsPage()
    {
        InitializeComponent();
        if (!Settings.Default.Verified) AutoC.IsChecked = false;
    }

    private void WindowsStart_OnClick(object sender, RoutedEventArgs e)
    {
        if (WindowsStart.IsChecked.Value)
        {
            var path = @"SOFTWARE\Microsoft\Windows\CurrentVersion\Run";
            var key = Registry.CurrentUser.OpenSubKey(path, true);
            key.SetValue("GadzzaaTB",
                Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\GadzzaaTB.exe");
        }
        else
        {
            var path = @"SOFTWARE\Microsoft\Windows\CurrentVersion\Run";
            var key = Registry.CurrentUser.OpenSubKey(path, true);
            key.DeleteValue("GadzzaaTB", false);
        }
    }

    private void ThrowException_OnClick(object sender, RoutedEventArgs e)
    {
        throw new InvalidOperationException("This is a test exception.");
    }
}