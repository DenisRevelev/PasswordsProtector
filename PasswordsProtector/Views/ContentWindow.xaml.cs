using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;

namespace PasswordsProtector.Views
{
    /// <summary>
    /// Логика взаимодействия для ContentWindow.xaml
    /// </summary>
    public partial class ContentWindow : Window
    {
        public ContentWindow()
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
    }
}
