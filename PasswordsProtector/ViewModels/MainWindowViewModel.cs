using PasswordsProtector.Models;
using PasswordsProtector.Models.Commands;
using PasswordsProtector.Models.Interfaces;
using PasswordsProtector.Models.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Xml.Linq;
using WindowsDesktop;

namespace PasswordsProtector.ViewModels
{
    public class MainWindowViewModel : INotifyPropertyChanged, IRequireViewIdentification
    {
        #region FIELDS
        private string _checkPassword;
        private string _setNewPassword = "";
        private string _checkNewPasswordSet;
        private string _oldPassword;
        private ObservableCollection<MainWindowModel> _passwordApplication = new ObservableCollection<MainWindowModel>();
        private const string _filaName = "PasswordsProtector.config";
        private const string _nameXRoot = "ArrayOfMainWindowModel";
        private Guid _viewId;
        #endregion

        #region COMMANDS
        private RelayCommand _savNewePasswordCommand { get; set; }
        public RelayCommand SaveNewPasswordCommand
        {
            get
            {
                return _savNewePasswordCommand ?? new RelayCommand(parameter =>
                {
                    SaveNewPassword();
                });
            }
        }

        private RelayCommand _passwordValidationCommand { get; set; }
        public RelayCommand PasswordValidationCommand
        {
            get
            {
                return _passwordValidationCommand ?? new RelayCommand(parameter =>
                {
                    PasswordValidation();
                });
            }
        }

        private RelayCommand _checkPasswordWhenClosingWindow { get; set; }
        public RelayCommand CheckPasswordWhenClosingWindowCommand
        {
            get
            {
                return _checkPasswordWhenClosingWindow ?? new RelayCommand(parameter =>
                {
                    CheckPasswordWhenClosingWindow();
                });
            }
        }

        private RelayCommand _closeApplicationCommand { get; set; }
        public RelayCommand CloseApplicationCommand
        {
            get
            {
                return _closeApplicationCommand ?? new RelayCommand(parameter =>
                {
                    CloseApplication();
                });
            }
        }
        #endregion

        #region CONSTRUCTORS
        public MainWindowViewModel()
        {
            DesktopManager.InitializeComObjects();
            DesktopManager.SwitchDesktop();
            if (!IsInDesignMode)
            {
                ViewId = Guid.NewGuid();
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

        public string SetMewPassword
        {
            get => _setNewPassword;
            set
            {
                _setNewPassword = value;
                OnPropertyChanged();
            }
        }

        public string CheckNewPasswordSet
        {
            get => _checkNewPasswordSet;
            set
            {
                _checkNewPasswordSet = value;
                OnPropertyChanged();
            }
        }

        public string CheckPassword
        {
            get => _checkPassword;
            set
            {
                _checkPassword = value;
                OnPropertyChanged();
            }
        }

        public string OldPassword
        {
            get => _oldPassword;
            set
            {
                _oldPassword = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<MainWindowModel> PasswordApplication
        {
            get => _passwordApplication;
            set => _passwordApplication = value;
        }

        public Guid ViewId
        {
            get => _viewId;
            set => _viewId = value;
        }
        #endregion

        #region METHODS
        /// <summary>
        /// Checks the existence of a password file and the correctness of its structure.
        /// </summary>
        private void CheckFileExists()
        {
            var fileEx = File.Exists(_filaName);
            if (!fileEx)
            {
                MessageBox.Show("Отсуствует файл");
                Application.Current.Shutdown();
            }
            else
            {
                CryptographyXml.DecryptXml(_filaName);
                XDocument xDoc = XDocument.Load(_filaName);
                var el = xDoc.Root.Name;
                if (el != _nameXRoot)
                {
                    MessageBox.Show("Файл поврежден");
                    Application.Current.Shutdown();
                }
            }
        }

        /// <summary>
        /// Gets the xml value of the element containing the password
        /// </summary>
        /// <returns></returns>
        private string GetValueXElement()
        {
            CryptographyXml.DecryptXml(_filaName);
            XDocument xDocs = XDocument.Load(_filaName);
            string root = xDocs.Root.Element("MainWindowModel").Value;
            return root;
        }

        /// <summary>
        /// Closes the window "MainWindow.xaml" if the password is entered correctly.
        /// At the same time, the application continues to work and open the window "ContentWindow.xaml". 
        /// </summary>
        private void PasswordValidation()
        {
            var root = GetValueXElement();
            if (CheckPassword != root)
                CheckPassword = "Неверный пароль";
            else
            {
                CallEncryptMethod();
                App.CloseMainWindow(ViewId);
            }
        }

        /// <summary>
        /// If the password is not correct and the window "MainWindow.xaml" is closed, then the application terminates.
        /// </summary>
        private void CheckPasswordWhenClosingWindow()
        {
            var root = GetValueXElement();
            if (CheckPassword == "" || CheckPassword != root)
            {
                Application.Current.Shutdown();
                CallEncryptMethod();
                DesktopManager.RemoveDesktop();
            }
            else
            {
                App.ShowContentWindow();
                CallEncryptMethod();
            }
        }

        /// <summary>
        /// Saves the newly set application password.
        /// </summary>
        private void SaveNewPassword()
        {
            var root = GetValueXElement();
            if (OldPassword != root)
                OldPassword = "Неверный пароль";
            else if (SetMewPassword != CheckNewPasswordSet)
                CheckNewPasswordSet = "Пароли не совпадают";
            else
            {
                PasswordApplication.Add(new MainWindowModel { Password = SetMewPassword });
                SaveData.SaveCollectionData(PasswordApplication, _filaName);
                CallEncryptMethod();
            }
        }

        /// <summary>
        /// Calls the encryption method from the "CryptographyXml.cs" class
        /// </summary>
        private void CallEncryptMethod()
        {
            CryptographyXml.EcryptXml(_nameXRoot, _filaName);
        }

        /// <summary>
        /// Shuts down the application.
        /// </summary>
        private void CloseApplication()
        {
            Application.Current.Shutdown();
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
