using System;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Win32;

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
            RegistryKey key = Registry.CurrentUser.OpenSubKey(path, true);
            key.SetValue("GadzzaaTB",  System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + "\\GadzzaaTB.exe");
        }
        else
        {
            var path = @"SOFTWARE\Microsoft\Windows\CurrentVersion\Run";
            RegistryKey key = Registry.CurrentUser.OpenSubKey(path, true);
            key.DeleteValue("GadzzaaTB", false);
        }
    }

    private void ThrowException_OnClick(object sender, RoutedEventArgs e)
    {
        throw new InvalidOperationException("This is a test exception.");
        
    }
}