using System;
using System.Threading;
using System.Windows;

namespace RemoteDesktopEmulator
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private static readonly Mutex ApplicationMutex = new Mutex(true, "{A3233BDD-035C-40D2-879B-CEF0F99773AD}");

        public App()
        {
            InitializeComponent();
        }

        [STAThread]
        private static void Main()
        {
            if (ApplicationMutex.WaitOne(TimeSpan.Zero, true))
            {
                try
                {
                    App app = new App();
                    app.Run(new MainWindow());
                }
                finally
                {
                    ApplicationMutex.ReleaseMutex();
                }
            }
        }
    }
}
