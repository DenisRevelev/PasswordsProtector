using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using WindowsDesktop;

namespace PasswordsProtector.Views
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
           
            InitializeComponent();
            InitializeComObjects();
            CreatAndMove();
        }

        private static async void InitializeComObjects()
        {
            try
            {
                await VirtualDesktopProvider.Default.Initialize(TaskScheduler.FromCurrentSynchronizationContext());
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Failed to initialize.");
            }

            VirtualDesktop.CurrentChanged += (sender, args) => System.Diagnostics.Debug.WriteLine($"Desktop changed: {args.NewDesktop.Id}");
        }
        VirtualDesktop desktop = VirtualDesktop.Create();
        private void CreatAndMove()
        {
            desktop.Switch();
        }

        [DllImport("user32.dll")]
        private static extern uint SetWindowDisplayAffinity(IntPtr hwnd, uint dwAffinity);

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            IntPtr handle = (new WindowInteropHelper(this)).Handle;
            const uint WDA_EXCLUDEFROMCAPTURE = 0x00000011;
            SetWindowDisplayAffinity(handle, WDA_EXCLUDEFROMCAPTURE);
        }
    }
}
