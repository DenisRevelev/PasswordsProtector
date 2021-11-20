using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;

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
        }

        [DllImport("user32.dll")]
        private static extern uint SetWindowDisplayAffinity(IntPtr hwnd, uint dwAffinity);

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            IntPtr handle = (new WindowInteropHelper(this)).Handle;
            const uint WDA_EXCLUDEFROMCAPTURE = 0x00000011;
            SetWindowDisplayAffinity(handle, WDA_EXCLUDEFROMCAPTURE);
        }

        private void showUIElementForSetNewPassword_Click(object sender, RoutedEventArgs e)
        {
            showUIElementForSetNewPassword.Visibility = Visibility.Collapsed;
            showUIElementForEntrance.Visibility = Visibility.Visible;
        }

        private void showUIElementForEntrance_Click(object sender, RoutedEventArgs e)
        {
            showUIElementForSetNewPassword.Visibility = Visibility.Visible;
            showUIElementForEntrance.Visibility = Visibility.Collapsed;
        }
    }
}
