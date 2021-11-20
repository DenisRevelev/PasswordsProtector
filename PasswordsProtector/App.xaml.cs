using PasswordsProtector.Models.Interfaces;
using PasswordsProtector.Views;
using System;
using System.Windows;

namespace PasswordsProtector
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public static void ShowContentWindow()
        {
            ContentWindow contentWindow = new ContentWindow();
            contentWindow.Show();
        }

        public static void CloseMainWindow(Guid id)
        {
            Window winCerrent = Current.MainWindow;
            var winId = winCerrent.DataContext as IRequireViewIdentification;
            if (winId != null && winId.ViewId.Equals(id))
                winCerrent.Close();
        }
    }
}
