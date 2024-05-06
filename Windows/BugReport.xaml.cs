using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace GadzzaaTB.Windows;

public partial class BugReport : INotifyPropertyChanged
{
    private string _description;
    private string _name;
    private bool _firstTime = true;

    public bool IsClosing;

    // private readonly MainWindow _mainWindow = Application.Current.MainWindow as MainWindow;
    private string _reportNametxt;

    public BugReport()
    {
        InitializeComponent();
        DataContext = this;
    }

    public string ReportNameTxt
    {
        get => _reportNametxt;
        set
        {
            _reportNametxt = value;
            OnPropertyChanged();
        }
    }

    public event PropertyChangedEventHandler PropertyChanged;


    public void RenderBugReport()
    {
        ReportName.Text = "A";
        VerifyReportName();
    }

    private void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
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
        VerifyReportName();
        if (string.IsNullOrWhiteSpace(ReportName.Text)) return;
        //  _description = await LogFile.LoadLogFile(_description);
        await Classes.Octokit.Main(_name, _description);
        var executableLocation = Path.GetDirectoryName(Assembly.GetEntryAssembly()!.Location);
        Process.Start(executableLocation + "\\GadzzaaTB.exe");
        Application.Current.Shutdown();
    }

    private void VerifyReportName()
    {
        var myBindingExpression = ReportName.GetBindingExpression(TextBox.TextProperty);
        var myBinding = myBindingExpression.ParentBinding;
        myBinding.UpdateSourceExceptionFilter = ReturnExceptionHandler;
        myBindingExpression.UpdateSource();
        if (_firstTime) ReportName.Text = "";
        _firstTime = false;
    }

    private object ReturnExceptionHandler(object bindingExpression, Exception exception)
    {
        return "This is from the UpdateSourceExceptionFilterCallBack.";
    }


    private void Grid_OnMouseDown(object sender, MouseButtonEventArgs e)
    {
        Grid.Focus();
    }
}