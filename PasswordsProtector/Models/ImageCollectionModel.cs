using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Runtime.CompilerServices;
using System.Text;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace PasswordsProtector.Models
{
    public class ImageCollectionModel : INotifyPropertyChanged
    {
        private string _icon;

        public string Icon
        {
            get => _icon;
            set
            {
                _icon = value;
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
