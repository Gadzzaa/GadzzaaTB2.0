﻿using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;

namespace GadzzaaTB.Windows;

public partial class BugReport
{
    private string _description;
    private string _name;
    public bool IsClosing;
    private readonly MainWindow _mainWindow = Application.Current.MainWindow as MainWindow;

    public BugReport()
    {
        InitializeComponent();
        IsClosing = false;
        Closing += OnClosing;
    }

    private void OnClosing(object sender, CancelEventArgs e)
    {
        if (!IsClosing)
        {
            Hide();
            e.Cancel = true;
        }
    }

    private void ReportName_OnTextChanged(object sender, TextChangedEventArgs e)
    {
        _name = ReportName.Text;
    }

    private void ReportDescription_OnTextChanged(object sender, TextChangedEventArgs e)
    {
        _description = ReportDescription.Text;
    }

    private async void SubmitButton_OnClick(object sender, RoutedEventArgs e)
    {
        //  _description = await LogFile.LoadLogFile(_description);
        //  await Classes.Octokit.Main(_name, _description);
        Close();
        string executableLocation = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location);
        Process.Start(executableLocation + "\\GadzzaaTB.exe");
        Application.Current.Shutdown();
    }

}