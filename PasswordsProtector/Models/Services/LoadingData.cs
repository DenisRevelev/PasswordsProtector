using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Linq;
using System.Xml.Linq;
using System.Xml.XPath;

namespace PasswordsProtector.Models.Services
{
    public class LoadingData
    {
        private static readonly string _path = $"{Environment.CurrentDirectory}\\";
        public static ObservableCollection<ContentWindowModel> LoadData(string file)
        {
            var xDoc = XDocument.Load(_path + file);
            var root = xDoc.CreateNavigator();
            var nodes = root.Select("ArrayOfContentWindowModel/ContentWindowModel");
            return new ObservableCollection<ContentWindowModel>(from XPathNavigator node in nodes select LoadCollection(node));
        }

        public static ContentWindowModel LoadCollection(XPathNavigator node)
        {
            var nameSpace = node.GetNamespace("ArrayOfContentWindowModel");
            return new ContentWindowModel
            {
                UrlSite = node.SelectSingleNode("UrlSite").Value,
                Login = node.SelectSingleNode("Login").Value,
                Password = node.SelectSingleNode("Password").Value,
                Filter = node.GetAttribute("Filter", nameSpace)
            };
        }

        public static ObservableCollection<ItemsMenuModel> LoadMenu(string file)
        {
            var xDoc = XDocument.Load(_path + file);
            var root = xDoc.CreateNavigator();
            var nodes = root.Select("ArrayOfItemsMenuModel/ItemsMenuModel");
            return new ObservableCollection<ItemsMenuModel>(from XPathNavigator node in nodes select LoadCollections(node));
        }


        public static ItemsMenuModel LoadCollections(XPathNavigator node)
        {
            return new ItemsMenuModel
            {
                ItemMenu = node.SelectSingleNode("ItemMenu").Value
            };
        }
    }
}
