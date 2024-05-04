using System;
using System.ComponentModel;
using System.Diagnostics;
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
    public bool IsClosing;
   // private readonly MainWindow _mainWindow = Application.Current.MainWindow as MainWindow;
    private string reportNametxt;
    private bool firsttime = true;

    public BugReport()
    {
        InitializeComponent();
        DataContext = this;
        IsClosing = false;
        Loaded += OnContentRendered;
    }

    private void OnContentRendered(object sender, EventArgs e)
    {
        ReportName.Text = "A";
        VerifyReportName();
    }

    public String ReportNameTxt
    {
        get { return reportNametxt; }
        set { reportNametxt = value; OnPropertyChanged(); }
    }
    
    private void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
    public event PropertyChangedEventHandler PropertyChanged;

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
        if (string.IsNullOrWhiteSpace(ReportName.Text))
        {
            return;
        }
        //  _description = await LogFile.LoadLogFile(_description);
        await Classes.Octokit.Main(_name, _description);
        string executableLocation = System.IO.Path.GetDirectoryName(Assembly.GetEntryAssembly()!.Location);
        Process.Start(executableLocation + "\\GadzzaaTB.exe");
        Application.Current.Shutdown();
    }
    
    private void VerifyReportName()
    {            
        var myBindingExpression = ReportName.GetBindingExpression(TextBox.TextProperty);
        var myBinding = myBindingExpression.ParentBinding;
        myBinding.UpdateSourceExceptionFilter = ReturnExceptionHandler;
        myBindingExpression.UpdateSource();
        if(firsttime) ReportName.Text = "";
        firsttime = false;
    }
    
    private object ReturnExceptionHandler(object bindingExpression, Exception exception) => "This is from the UpdateSourceExceptionFilterCallBack.";


    private void Grid_OnMouseDown(object sender, MouseButtonEventArgs e)
    {
        Grid.Focus();
    }
}