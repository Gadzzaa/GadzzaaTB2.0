using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;

namespace GadzzaaTB.Windows;

public partial class BugReport
{
    private string? _description;
    private string? _name;
    public bool IsClosing;

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

    private void ReportName_OnGotFocus(object sender, RoutedEventArgs e)
    {
        if (ReportName.Text == "Report Name") ReportName.Clear();
    }

    private void ReportName_OnLostFocus(object sender, RoutedEventArgs e)
    {
        if (ReportName.Text == "") ReportName.Undo();
    }

    private void ReportName_OnTextChanged(object sender, TextChangedEventArgs e)
    {
        _name = ReportName.Text;
    }

    private void ReportDescription_OnGotFocus(object sender, RoutedEventArgs e)
    {
        if (ReportDescription.Text == "Report Description") ReportDescription.Clear();
    }

    private void ReportDescription_OnLostFocus(object sender, RoutedEventArgs e)
    {
        if (ReportDescription.Text == "") ReportDescription.Undo();
    }

    private void ReportDescription_OnTextChanged(object sender, TextChangedEventArgs e)
    {
        _description = ReportDescription.Text;
    }

    private async void SubmitButton_OnClick(object sender, RoutedEventArgs e)
    {
        //  _description = await LogFile.LoadLogFile(_description);
        await Classes.Octokit.Main(_name, _description);
        Close();
        Process.Start(Application.ResourceAssembly.Location);
        Application.Current.Shutdown();
    }
}