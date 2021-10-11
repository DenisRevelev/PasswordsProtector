using PasswordsProtector.Models;
using PasswordsProtector.Models.Commands;
using PasswordsProtector.Models.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;
using System.Windows;

namespace PasswordsProtector.ViewModels
{
    public class ContentWindowViewModel : INotifyPropertyChanged
    {
        #region FIELDS
        private string _urlSite = "";
        private string _login = "";
        private string _password = "";
        private const string _fileName = "Keeper-of-Passwords.xml";
        private ObservableCollection<ContentWindowModel> _enteredData = new ObservableCollection<ContentWindowModel>();
        private const string _encryptElement = "ArrayOfContentWindowModel";
        #endregion

        #region COMMANDS
        private RelayCommand _addData { get; set; }
        public RelayCommand AddData
        {
            get
            {
                return _addData ?? new RelayCommand(parameter =>
                {
                    AddEnteredData();
                });
            }
        }

        private RelayCommand _deleteData { get; set; }
        public RelayCommand DeleteData
        {
            get
            {
                return _deleteData ?? new RelayCommand(parameter =>
                {
                    var data = (ContentWindowModel)parameter;
                    DeleteSelectedData(data);
                });
            }
        }

        private RelayCommand _removeDesktopCommand { get; set; }

        public RelayCommand RemoveDesktopCommand
        {
            get
            {
                return _removeDesktopCommand ?? new RelayCommand(parameter =>
                {
                    DesktopManager.RemoveDesktop();
                });
            }
        }
        #endregion

        #region CONSTRUCTORS
        public ContentWindowViewModel()
        {
            if (!IsInDesignMode)
            {
                CheckFileExists();
            }
        }
        #endregion

        #region PROPERTIES
        /// <summary>
        /// For the XAML constructor to work correctly.
        /// </summary>
        private bool IsInDesignMode
        {
            get
            {
                var prop = DesignerProperties.IsInDesignModeProperty;
                return (bool)DependencyPropertyDescriptor
                    .FromProperty(prop, typeof(FrameworkElement))
                    .Metadata.DefaultValue;
            }
        }

        public ObservableCollection<ContentWindowModel> EnteredData
        {
            get => _enteredData;
            set
            {
                _enteredData = value;
            }
        }

        public string UrlSiteVM
        {
            get => _urlSite;
            set
            {
                _urlSite = value;
                OnPropertyChanged();
            }
        }

        public string LoginVM
        {
            get => _login;
            set
            {
                _login = value;
                OnPropertyChanged();
            }
        }

        public string PasswordVM
        {
            get => _password;
            set
            {
                _password = value;
                OnPropertyChanged();
            }
        }
        #endregion

        #region METHODS
        private void AddEnteredData()
        {
            if (CheckFieldsEmptiness() == true)
            {
                MessageBox.Show("Не все поля заполнены");
            }
            else
            {
                EnteredData.Add(new ContentWindowModel { UrlSite = UrlSiteVM, Login = LoginVM, Password = PasswordVM });
                SaveEndEcryptFile();
            }
        }

        private void DeleteSelectedData(ContentWindowModel data)
        {
            EnteredData.Remove(data);
            SaveEndEcryptFile();
        }

        private void CheckFileExists()
        {
            var fileEx = File.Exists(_fileName);
            if (!fileEx)
            {
                SaveEndEcryptFile();
            }
            else
            {
                CryptographyXml.DecryptXml(_fileName);
                EnteredData = LoadingData.LoadData(_fileName);
                CryptographyXml.EcryptXml(_encryptElement, _fileName);
            }
        }

        private void SaveEndEcryptFile()
        {
            SaveData.SaveCollectionData(EnteredData, _fileName);
            CryptographyXml.EcryptXml(_encryptElement, _fileName);
        }

        private bool CheckFieldsEmptiness()
        {
            var boolCheck = false;
            if (UrlSiteVM == string.Empty || LoginVM == string.Empty || PasswordVM == string.Empty)
            {
                boolCheck = true;
            }
            return boolCheck;
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
