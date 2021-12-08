using PasswordsProtector.Models;
using PasswordsProtector.Models.Commands;
using PasswordsProtector.Models.Interfaces;
using PasswordsProtector.Models.Services;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace PasswordsProtector.ViewModels
{
    public class ContentWindowViewModel : INotifyPropertyChanged, IAsyncInitialization
    {
        #region FIELDS
        private ObservableCollection<ContentWindowModel> _enteredData;
        private ObservableCollection<ItemsMenuModel> _menuItemNames;
        private ObservableCollection<ImageCollectionModel> _iconCollectionView;
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

        private RelayCommand _deleteItem { get; set; }
        public RelayCommand DeleteItem
        {
            get
            {
                return _deleteItem ?? new RelayCommand(parameter =>
               {
                   var data = (ItemsMenuModel)parameter;
                   DeleteSelectMenuItem(data);
                   SetFilterForCollection();
               });
            }
        }

        private RelayCommand _allItemCollectionData { get; set; }

        public RelayCommand AllItemCollectionData
        {
            get
            {
                return _allItemCollectionData ?? new RelayCommand(parameter =>
                {
                    ReturnAllItemCollection();
                });
            }
        }

        private RelayCommand _editData { get; set; }

        public RelayCommand EditData
        {
            get
            {
                return _editData ?? new RelayCommand(parameter =>
                {
                    UrlSiteView = SelectedItemContent.UrlSite;
                    LoginView = SelectedItemContent.Login;
                    PasswordView = SelectedItemContent.Password;
                    ItemNameMenu = SelectedItemContent.Filter;

                    var data = (ContentWindowModel)parameter;
                    DeleteData.Execute(data);
                });
            }
        }
        #endregion

        #region CONSTRUCTORS
        public ContentWindowViewModel()
        {
            _enteredData = new ObservableCollection<ContentWindowModel>();
            _menuItemNames = new ObservableCollection<ItemsMenuModel>();
            _iconCollectionView = new ObservableCollection<ImageCollectionModel>();
            if (!IsInDesignMode)
            {
                Initialization = InitializeAsync();
                CheckFileExistsAsync();
            }
        }


        #endregion

        #region PROPERTIES
        /// <summary>
        /// Contains a set of icons for the menu.
        /// </summary>
        public ObservableCollection<ImageCollectionModel> IconCollectionView
        {
            get => _iconCollectionView;
            set
            {
                _iconCollectionView = value;
                OnPropertyChanged();
            }
        }

        private ContentWindowModel _selectedItemContent;
        /// <summary>
        /// The selected item accesses the model "ContentWindowModel".
        /// </summary>
        public ContentWindowModel SelectedItemContent
        {
            get => _selectedItemContent;
            set
            {
                _selectedItemContent = value;
                if (value == null)
                    IsEnabledButton = false;
                else
                    IsEnabledButton = true;
                OnPropertyChanged();
            }
        }

        private bool _isEnabletButton = false;
        /// <summary>
        /// The button is inactive until an item is selected in the collection.
        /// </summary>
        public bool IsEnabledButton
        {
            get => _isEnabletButton;
            set
            {
                _isEnabletButton = value;
                OnPropertyChanged();
            }
        }
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

        private string _itemMenu;
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

        private string _urlSiteView = "";
        /// <summary>
        /// Gets the user-entered utl of the site.
        /// </summary>
        public string UrlSiteView
        {
            get => _urlSiteView;
            set
            {
                _urlSiteView = value;
                if (value != string.Empty) value = string.Empty;

                OnPropertyChanged();
            }
        }

        private string _loginView = "";
        /// <summary>
        /// Gets the login entered by the user.
        /// </summary>
        public string LoginView
        {
            get => _loginView;
            set
            {
                _loginView = value;
                if (value != string.Empty) value = string.Empty;
                OnPropertyChanged();
            }
        }

        private string _passwordView = "";
        /// <summary>
        /// Gets the password entered by the user.
        /// </summary>
        public string PasswordView
        {
            get => _passwordView;
            set
            {
                _passwordView = value;
                if (value != string.Empty) value = string.Empty;
                OnPropertyChanged();
            }
        }

        private ItemsMenuModel _selectedItem;
        /// <summary>
        /// The selected item accesses the model "ItemsMenuModel".
        /// </summary>
        public ItemsMenuModel SelectedItemMenu
        {
            get => _selectedItem;
            set
            {
                _selectedItem = value;
                ItemNameMenu = value.ItemMenu;
                ImageCategoryTop = value.ImageItem;
                SetFilterForCollection();
                OnPropertyChanged();
            }
        }

        private ImageCollectionModel _selectedItemInComboBox;

        public ImageCollectionModel SelectedItemInComboBox
        {
            get => _selectedItemInComboBox;
            set
            {
                _selectedItemInComboBox = value;
                ImageCategory = value.Icon;
                OnPropertyChanged();
            }
        }

        private string _imageCategory;

        public string ImageCategory
        {
            get => _imageCategory;
            set
            {
                _imageCategory = value;
                OnPropertyChanged();
            }
        }

        private string _imageCategoryTop;

        public string ImageCategoryTop
        {
            get => _imageCategoryTop;
            set
            {
                _imageCategoryTop = value;
                OnPropertyChanged();
            }
        }

        private string _itemNameMenu;
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

        public Task Initialization { get; private set; }
        #endregion

        #region METHODS
        private const string _fileName = "Microsoft.CodeAnalysis.xml";
        private const string _fileNameForMenu = "Microsoft.Menu.xml";
        private const string _encryptElement = "ArrayOfContentWindowModel";
        private string[] imagesList;

        private async Task InitializeAsync()
        {
            await GetIconsAsync();
            //await CheckFileExistsAsync();
        }

        /// <summary>
        /// Check for xml file with passwords. If it is, then it creates a new, empty file.
        /// If so, it loads data from it.
        /// </summary>
        private void CheckFileExistsAsync()
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
                ReturnAllItemCollection();
                CryptographyXml.EcryptXml(_encryptElement, _fileName);
            }
        }

        /// <summary>
        /// Gets icons from a folder.
        /// </summary>
        private async Task GetIconsAsync()
        {
            imagesList = await Task.Run(() => Directory.EnumerateFiles($"{Environment.CurrentDirectory}", "*.png",
               SearchOption.AllDirectories).ToArray());

            foreach (string pathImg in imagesList)
            {
                IconCollectionView.Add(new ImageCollectionModel()
                {
                    Icon = pathImg
                });
            }
        }

        /// <summary>
        /// Checks if all fields are filled in.
        /// </summary>
        /// <returns></returns>
        private bool CheckFieldsEmptiness()
        {
            var boolCheck = false;
            if (UrlSiteView == string.Empty || LoginView == string.Empty || PasswordView == string.Empty)
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
                EnteredData.Add(new ContentWindowModel
                {
                    UrlSite = UrlSiteView,
                    Login = LoginView,
                    Password = PasswordView,
                    Filter = ItemNameMenu
                });
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
        /// Deletes the selected menu item.
        /// </summary>
        /// <param name="item"></param>
        private void DeleteSelectMenuItem(ItemsMenuModel item)
        {
            MenuItemNames.Remove(item);
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
                ItemMenu = ItemMenuVM,
                ImageItem = ImageCategory
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

        /// <summary>
        /// Returns all elements of the content collection.
        /// </summary>
        private void ReturnAllItemCollection()
        {
            FilteredCollectionView.Filter = AllItemCollection;
        }

        private bool AllItemCollection(object o)
        {
            return true;
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
