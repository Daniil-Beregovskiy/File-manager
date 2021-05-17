using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using File_manager.Properties;

namespace File_manager
{
    [Serializable]
    public class Сustomization
    {
        public Color backColor { get; set; }
        public Font font { get; set; }
        public int havePas { get; set; }
        public string login { get; set; }
        public string password { get; set; }
        private static byte[] key = new byte[8] { 1, 2, 3, 4, 5, 6, 7, 8 };
        private static byte[] iv = new byte[8] { 1, 2, 3, 4, 5, 6, 7, 8 };
        public Сustomization() { }
        public Сustomization(string Login, string Password)
        {
            login = Login;
            password = Password;
        }
        public void Save()
        {
            string filename = "custom.dat";
            BinaryFormatter binFormat = new BinaryFormatter();
            Stream fstream = new FileStream(filename,
                FileMode.Create, FileAccess.Write, FileShare.None);
            binFormat.Serialize(fstream, this);
            fstream.Close();
        }
        public static Сustomization GetSettings()
        {
            string filename = "custom.dat";
            Сustomization custom = new Сustomization();
            if (File.Exists(filename))
            {
                Stream fstream = new FileStream(filename, FileMode.Open, FileAccess.Read, FileShare.None);
                BinaryFormatter binFormat = new BinaryFormatter();
                custom = (Сustomization)binFormat.Deserialize(fstream);
                fstream.Close();
            }
            return custom;
        }
        [OnSerializing]
        internal void OnSerializing(StreamingContext context)
        {
            if (password != null)
            {
                password = Crypt(password);
            }
        }
        [OnDeserialized]
        internal void OnDeserializing(StreamingContext context)
        {
            if (password != null)
            {
                password = Decrypt(password);
            }
        }
        private static string Crypt(string text)
        {
            try
            {
                SymmetricAlgorithm algorithm = DES.Create();
                ICryptoTransform transform = algorithm.CreateEncryptor(key, iv);
                byte[] inputbuffer = Encoding.Unicode.GetBytes(text);
                byte[] outputBuffer = transform.TransformFinalBlock(inputbuffer, 0, inputbuffer.Length);
                return Convert.ToBase64String(outputBuffer);
            }
            catch (Exception)
            {
                return null;
            }
        }

        private static string Decrypt(string text)
        {
            try
            {
                SymmetricAlgorithm algorithm = DES.Create();
                ICryptoTransform transform = algorithm.CreateDecryptor(key, iv);
                byte[] inputbuffer = Convert.FromBase64String(text);
                byte[] outputBuffer = transform.TransformFinalBlock(inputbuffer, 0, inputbuffer.Length);
                return Encoding.Unicode.GetString(outputBuffer);
            }
            catch (Exception)
            {
                return null;
            }
        }



    }
}
