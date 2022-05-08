using System.ComponentModel;


namespace GadzzaaTB.Windows
{
    public partial class BugReport
    {
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
                this.Hide();
                e.Cancel = true;
            }
        }
    }
}