using System.ComponentModel;
using System.Runtime.CompilerServices;

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
