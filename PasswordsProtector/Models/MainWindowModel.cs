using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;

namespace PasswordsProtector.Models
{
    public class MainWindowModel : INotifyPropertyChanged
    {
        private string _password;

        public string Password
        {
            get => _password;
            set
            {
                _password = value;
                OnPropertyChanged();
            }
        }

        #region EVENT AND METHOD FOR NOTIFYING THE CLIENT ABOUT CHANGES
        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion
    }
}
