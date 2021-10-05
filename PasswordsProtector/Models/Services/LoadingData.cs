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
            return new ContentWindowModel
            {
                UrlSite = node.SelectSingleNode("UrlSite").Value,
                Login = node.SelectSingleNode("Login").Value,
                Password = node.SelectSingleNode("Password").Value
            };
        }
    }
}
