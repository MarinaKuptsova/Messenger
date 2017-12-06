using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net.Mime;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using Messenger.Model;

namespace Messenger.Client.DataAccess
{
    public class Converter
    {
        public BitmapImage ConverterByteToImage(byte[] bytes)
        {
            BitmapImage image = new BitmapImage();
            using (var memoryStream = new MemoryStream(bytes))
            {
                image.BeginInit();
                image.CacheOption = BitmapCacheOption.OnLoad;
                image.StreamSource = memoryStream;
                image.EndInit();
            }
            return image;
        }

        public byte[] ConverterImageToArray(string path)
        {
            Image image = Image.FromFile(path);

            ImageConverter converter = new ImageConverter();
            byte[] array = (byte[])converter.ConvertTo(image, typeof(byte[]));
            return array;
        }

        public byte[] ConverterTextToArray(string path)
        {
            var array = File.ReadAllBytes(path);
            return array;
        }

        public void FromBytesToText(Files files)
        {
            string s = Encoding.UTF7.GetString(files.UserFile);
            var filename = String.Concat(files.Name, files.Type);
            var path = String.Concat(Directory.GetCurrentDirectory(), "\\" + filename);
            File.WriteAllText(path, s);
            Process.Start(filename);
        }

        public void FromBytesToImage(Files files)
        {
            MemoryStream ms = new MemoryStream(files.UserFile);
            Image img = Image.FromStream(ms);
            var filename = String.Concat(files.Name, files.Type);
            var path = String.Concat(Directory.GetCurrentDirectory(), "\\" + filename);
            img.Save(path);
            Process.Start(filename);
        }

        public byte[] FromSmthToArray(string filename, string path)
        {
            var type = "." + filename.Split('.')[1];
            switch (type)
            {
                case ".jpg":
                    return ConverterImageToArray(path);
                case ".png":
                    return ConverterImageToArray(path);
                case ".jpeg":
                    return ConverterImageToArray(path);
                case ".txt":
                    return ConverterTextToArray(path);
                default:
                    return null;
            }
        }

        public void FromBytesToSmth(Files file)
        {
            switch (file.Type)
            {
                case ".jpg":
                    FromBytesToImage(file);
                    break;
                case ".png":
                    FromBytesToImage(file);
                    break;
                case ".jpeg":
                    FromBytesToImage(file);
                    break;
                case ".txt":
                    FromBytesToText(file);
                    break;
                default:
                    break;
            }
        }
    }
}
