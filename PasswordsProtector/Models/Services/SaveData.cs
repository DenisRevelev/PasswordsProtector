using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Text;
using System.Windows;
using System.Xml.Serialization;

namespace PasswordsProtector.Models.Services
{
    public class SaveData
    {
        private static readonly string _pafh = $"{Environment.CurrentDirectory}\\";
        public static void SaveCollectionData<T>(ObservableCollection<T> collection, string fileName)
        {
            try
            {
                XmlSerializer xmlSerializer = new XmlSerializer(typeof(ObservableCollection<T>));
                using (StreamWriter sw = new StreamWriter(_pafh + fileName))
                {
                    xmlSerializer.Serialize(sw, collection);
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString());
            }
        }
    }
}
