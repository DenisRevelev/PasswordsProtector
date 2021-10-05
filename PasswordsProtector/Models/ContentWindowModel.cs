using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;

namespace PasswordsProtector.Models
{
    public class ContentWindowModel : INotifyPropertyChanged
    {
        #region FIELDS
        private string _urlSite;
        private string _login;
        private string _password;
        #endregion

        #region PROPERTIES
        public string UrlSite
        {
            get => _urlSite;
            set
            {
                _urlSite = value;
                OnPropertyChanged();
            }
        }

        public string Login
        {
            get => _login;
            set
            {
                _login = value;
                OnPropertyChanged();
            }
        }

        public string Password
        {
            get => _password;
            set
            {
                _password = value;
                OnPropertyChanged();
            }
        }
        #endregion

        #region EVENT AND METHOD FOR NOTIFYING THE CLIENT ABOUT CHANGES
        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion
    }
}
