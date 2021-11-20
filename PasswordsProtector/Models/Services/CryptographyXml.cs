using System;
using System.Security.Cryptography;
using System.Security.Cryptography.Xml;
using System.Windows;
using System.Xml;

namespace PasswordsProtector.Models.Services
{
    public class CryptographyXml
    {
        #region FIELDS
        private static XmlDocument xmlDoc = new XmlDocument();
        private static CspParameters cspParams = new CspParameters(); // Create a new CspParameters object to specify a key container.
        private static RSACryptoServiceProvider rsaKey; // Create a new RSA key and save it in the container.  This key will encrypt a symmetric key, which will then be encrypted in the XML document.
        private static string _keyName = "rsaKeyDenis";
        private static readonly string _path = $"{Environment.CurrentDirectory}\\";
        #endregion

        #region METHODS
        public static void EcryptXml(string encryptElement, string fileName)
        {
            try
            {
                xmlDoc.PreserveWhitespace = true;
                xmlDoc.Load(_path + fileName);
            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString());
            }

            cspParams.KeyContainerName = "XML_ENC_RSA_KEY";
            rsaKey = new RSACryptoServiceProvider(cspParams);
            try
            {
                // Encrypt the element.
                Encrypt(xmlDoc, encryptElement, "EncryptedElement1", rsaKey, _keyName);
                xmlDoc.Save(_path + fileName);
            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString());
            }
            finally
            {
                // Clear the RSA key.
                rsaKey.Clear();
            }
        }

        public static void DecryptXml(string fileName)
        {
            try
            {
                xmlDoc.PreserveWhitespace = true;
                xmlDoc.Load(_path + fileName);
            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString());
            }

            cspParams.KeyContainerName = "XML_ENC_RSA_KEY";
            rsaKey = new RSACryptoServiceProvider(cspParams);

            try
            {
                Decrypt(xmlDoc, rsaKey, _keyName);
                xmlDoc.Save(_path + fileName);
            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString());
                //Application.Current.Shutdown();
            }
            finally
            {
                // Clear the RSA key.
                rsaKey.Clear();
            }
        }
        private static void Encrypt(XmlDocument Doc, string ElementToEncrypt, string EncryptionElementID, RSA Alg, string KeyName)
        {
            // Check the arguments.
            if (Doc == null)
                throw new ArgumentNullException("Doc");
            if (ElementToEncrypt == null)
                throw new ArgumentNullException("ElementToEncrypt");
            if (EncryptionElementID == null)
                throw new ArgumentNullException("EncryptionElementID");
            if (Alg == null)
                throw new ArgumentNullException("Alg");
            if (KeyName == null)
                throw new ArgumentNullException("KeyName");

            ////////////////////////////////////////////////
            // Find the specified element in the XmlDocument
            // object and create a new XmlElement object.
            ////////////////////////////////////////////////
            XmlElement elementToEncrypt = Doc.GetElementsByTagName(ElementToEncrypt)[0] as XmlElement;

            // Throw an XmlException if the element was not found.
            if (elementToEncrypt == null)
            {
                throw new XmlException("The specified element was not found");
            }
            Aes sessionKey = null;

            try
            {
                //////////////////////////////////////////////////
                // Create a new instance of the EncryptedXml class
                // and use it to encrypt the XmlElement with the
                // a new random symmetric key.
                //////////////////////////////////////////////////

                // Create an AES key.
                sessionKey = Aes.Create();

                EncryptedXml eXml = new EncryptedXml();

                byte[] encryptedElement = eXml.EncryptData(elementToEncrypt, sessionKey, false);
                ////////////////////////////////////////////////
                // Construct an EncryptedData object and populate
                // it with the desired encryption information.
                ////////////////////////////////////////////////

                EncryptedData edElement = new EncryptedData();
                edElement.Type = EncryptedXml.XmlEncElementUrl;
                edElement.Id = EncryptionElementID;
                // Create an EncryptionMethod element so that the
                // receiver knows which algorithm to use for decryption.

                edElement.EncryptionMethod = new EncryptionMethod(EncryptedXml.XmlEncAES256Url);
                // Encrypt the session key and add it to an EncryptedKey element.
                EncryptedKey ek = new EncryptedKey();

                byte[] encryptedKey = EncryptedXml.EncryptKey(sessionKey.Key, Alg, false);

                ek.CipherData = new CipherData(encryptedKey);

                ek.EncryptionMethod = new EncryptionMethod(EncryptedXml.XmlEncRSA15Url);

                // Create a new DataReference element
                // for the KeyInfo element.  This optional
                // element specifies which EncryptedData
                // uses this key.  An XML document can have
                // multiple EncryptedData elements that use
                // different keys.
                DataReference dRef = new DataReference();

                // Specify the EncryptedData URI.
                dRef.Uri = "#" + EncryptionElementID;

                // Add the DataReference to the EncryptedKey.
                ek.AddReference(dRef);
                // Add the encrypted key to the
                // EncryptedData object.

                edElement.KeyInfo.AddClause(new KeyInfoEncryptedKey(ek));
                // Set the KeyInfo element to specify the
                // name of the RSA key.


                // Create a new KeyInfoName element.
                KeyInfoName kin = new KeyInfoName();

                // Specify a name for the key.
                kin.Value = KeyName;

                // Add the KeyInfoName element to the
                // EncryptedKey object.
                ek.KeyInfo.AddClause(kin);
                // Add the encrypted element data to the
                // EncryptedData object.
                edElement.CipherData.CipherValue = encryptedElement;
                ////////////////////////////////////////////////////
                // Replace the element from the original XmlDocument
                // object with the EncryptedData element.
                ////////////////////////////////////////////////////
                EncryptedXml.ReplaceElement(elementToEncrypt, edElement, false);
            }
            catch (Exception e)
            {
                // re-throw the exception.
                throw e;
            }
            finally
            {
                if (sessionKey != null)
                {
                    sessionKey.Clear();
                }
            }
        }

        private static void Decrypt(XmlDocument Doc, RSA Alg, string KeyName)
        {
            // Check the arguments.
            if (Doc == null)
                throw new ArgumentNullException("Doc");
            if (Alg == null)
                throw new ArgumentNullException("Alg");
            if (KeyName == null)
                throw new ArgumentNullException("KeyName");

            // Create a new EncryptedXml object.
            EncryptedXml exml = new EncryptedXml(Doc);

            // Add a key-name mapping.
            // This method can only decrypt documents
            // that present the specified key name.
            exml.AddKeyNameMapping(KeyName, Alg);

            // Decrypt the element.
            exml.DecryptDocument();
        }
        #endregion
    }
}
