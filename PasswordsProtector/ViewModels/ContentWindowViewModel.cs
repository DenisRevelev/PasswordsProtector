using PasswordsProtector.Models;
using PasswordsProtector.Models.Commands;
using PasswordsProtector.Models.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Windows;
using System.Windows.Data;
using System.Xml.Linq;
using System.Xml.XPath;

namespace PasswordsProtector.ViewModels
{
    public class ContentWindowViewModel : INotifyPropertyChanged
    {
        #region FIELDS
        private string _urlSite = "";
        private string _login = "";
        private string _password = "";
        private string _itemMenu;
        private string _itemNameMenu;
        private ItemsMenuModel _selectedItem;
        private const string _fileName = "Microsoft.CodeAnalysis.xml";
        private const string _fileNameForMenu = "Microsoft.Menu.xml";
        private ObservableCollection<ContentWindowModel> _enteredData;
        private ObservableCollection<ItemsMenuModel> _menuItemNames;
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
                    SetFilterForCollection();
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
                    SetFilterForCollection();
                });
            }
        }

        private RelayCommand _addItem { get; set; }
        public RelayCommand AddItem
        {
            get
            {
                return _addItem ?? new RelayCommand(parameter =>
                {
                    AddNewItemInMenu();
                });
            }
        }
        #endregion

        #region CONSTRUCTORS
        public ContentWindowViewModel()
        {
            _enteredData = new ObservableCollection<ContentWindowModel>();
            _menuItemNames = new ObservableCollection<ItemsMenuModel>();
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

        /// <summary>
        /// Stores all entered user data.
        /// </summary>
        public ObservableCollection<ContentWindowModel> EnteredData
        {
            get => _enteredData;
            set
            {
                _enteredData = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Stores the names of menu items.
        /// </summary>
        public ObservableCollection<ItemsMenuModel> MenuItemNames
        {
            get => _menuItemNames;
            set
            {
                _menuItemNames = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Is a collection on top of the main collection. Used for filtration.
        /// </summary>
        public ICollectionView FilteredCollectionView
        {
            get
            {
                return CollectionViewSource.GetDefaultView(EnteredData);
            }
        }
        /// <summary>
        /// Gets the user-entered name for the menu item.
        /// </summary>
        public string ItemMenuVM
        {
            get => _itemMenu;
            set
            {
                _itemMenu = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Gets the user-entered utl of the site.
        /// </summary>
        public string UrlSiteVM
        {
            get => _urlSite;
            set
            {
                _urlSite = CryptographyDataInMemory.EncryptDataInMemory(value);
                if (value != string.Empty) value = string.Empty;

                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Gets the login entered by the user.
        /// </summary>
        public string LoginVM
        {
            get => _login;
            set
            {
                _login = CryptographyDataInMemory.EncryptDataInMemory(value);
                if (value != string.Empty) value = string.Empty;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Gets the password entered by the user.
        /// </summary>
        public string PasswordVM
        {
            get => _password;
            set
            {
                _password = CryptographyDataInMemory.EncryptDataInMemory(value);
                if (value != string.Empty) value = string.Empty;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// The selected item accesses the menu model.
        /// </summary>
        public ItemsMenuModel SelectedItem
        {
            get => _selectedItem;
            set
            {
                _selectedItem = value;
                ItemNameMenu = value.ItemMenu;
                SetFilterForCollection();
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Gets the name of the selected menu item.
        /// </summary>
        public string ItemNameMenu
        {
            get => _itemNameMenu;
            set
            {
                _itemNameMenu = value;
                OnPropertyChanged();
            }
        }
        #endregion

        #region METHODS

        /// <summary>
        /// Check for xml file with passwords. If it is, then it creates a new, empty file.
        /// If so, it loads data from it.
        /// </summary>
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
                MenuItemNames = LoadingData.LoadMenu(_fileNameForMenu);
                CryptographyXml.EcryptXml(_encryptElement, _fileName);
            }
        }

        /// <summary>
        /// Checks if all fields are filled in.
        /// </summary>
        /// <returns></returns>
        private bool CheckFieldsEmptiness()
        {
            var boolCheck = false;
            if (UrlSiteVM == string.Empty || LoginVM == string.Empty || PasswordVM == string.Empty)
            {
                boolCheck = true;
            }
            return boolCheck;
        }

        /// <summary>
        /// Adds the entered data to the data collection.
        /// </summary>
        private void AddEnteredData()
        {
            if (CheckFieldsEmptiness() == true)
            {
                MessageBox.Show("Не все поля заполнены");
            }
            else
            {
                EnteredData.Add(new ContentWindowModel {
                    UrlSite = CryptographyDataInMemory.DecryptDataInMemory(UrlSiteVM),
                    Login = CryptographyDataInMemory.DecryptDataInMemory(LoginVM), 
                    Password = CryptographyDataInMemory.DecryptDataInMemory(PasswordVM),
                    Filter = ItemNameMenu});
                SaveEndEcryptFile();
            }
        }

        /// <summary>
        /// Removes the selected item from the data collection.
        /// </summary>
        /// <param name="data"></param>
        private void DeleteSelectedData(ContentWindowModel data)
        {
            EnteredData.Remove(data);
            SaveEndEcryptFile();
        }

        /// <summary>
        /// Saves the data added to the collection to an xml file.
        /// And then it encrypts the file.
        /// </summary>
        private void SaveEndEcryptFile()
        {
            SaveData.SaveCollectionData(EnteredData, _fileName);
            SaveData.SaveCollectionData(MenuItemNames, _fileNameForMenu);
            CryptographyXml.EcryptXml(_encryptElement, _fileName);
        }

        /// <summary>
        /// Adds a new item to the menu.
        /// </summary>
        private void AddNewItemInMenu()
        {
            MenuItemNames.Add(new ItemsMenuModel
            {
                ItemMenu = ItemMenuVM
            });
            ItemNameMenu = ItemMenuVM;
            SaveEndEcryptFile();
        }

        /// <summary>
        /// Sets filters for a collection of data.
        /// </summary>
        private void SetFilterForCollection()
        {
            FilteredCollectionView.Filter = GetFilter;
        }

        /// <summary>
        /// Gets the filter value depending on which item is selected in the menu.
        /// </summary>
        /// <param name="o"></param>
        /// <returns></returns>
        private bool GetFilter(object o)
        {
            ContentWindowModel model = o as ContentWindowModel;
            if (model.Filter == ItemNameMenu)
                return true;
            else
                return false;
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
