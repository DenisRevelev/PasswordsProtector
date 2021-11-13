﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;

namespace PasswordsProtector.Models
{
    public class ItemsMenuModel : INotifyPropertyChanged 
    {
        private string _itemMenu;
        private string _imageItem;

        public string ItemMenu
        {
            get => _itemMenu;
            set
            {
                _itemMenu = value;
                OnPropertyChanged();
            }
        }

        public string ImageItem
        {
            get => _imageItem;
            set
            {
                _imageItem = value;
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
