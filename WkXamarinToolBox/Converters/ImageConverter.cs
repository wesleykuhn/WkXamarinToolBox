using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using WkXamarinToolBox.Extensions;
using WkXamarinToolBox.Models;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace WkXamarinToolBox.Converters
{
    public class ImageConverter
    {
        public static string GlyphToXamlStaticGlyph(string staticGlyph)
        {
            if (staticGlyph.IsNullEmptyOrWhiteSpace()) return null;
            else return staticGlyph.Replace(@"\u", "&#x");
        }

        public static ImageSource BytesArrayToImageSource(in byte[] bytesArray)
        {
            try
            {
                MemoryStream ms = new(bytesArray);
                return ImageSource.FromStream(() => ms);
            }
            catch (Exception)
            {
                return null;
            }
        }

        public static byte[] UriToBytesArray(in string uri)
        {
            byte[] downloaded;

            using (WebClient wc = new())
            {
                downloaded = wc.DownloadData(uri);
            }

            return downloaded;
        }

        public static async Task<DeviceFileModel> FileResultToDeviceFile(FileResult photo)
        {
            if (photo is null)
                return null;

            var newFilePath = Path.Combine(FileSystem.CacheDirectory, photo.FileName);
            DeviceFileModel dcp = new(newFilePath, photo.FileName);

            try
            {
                using (var stream = await photo.OpenReadAsync())
                {
                    using (MemoryStream ms = new MemoryStream())
                    {
                        stream.CopyTo(ms);
                        dcp.ByteArray = ms.ToArray();
                        ms.Dispose();
                    }
                }
            }
            catch (Exception)
            {
                return null;
            }

            return dcp;
        }
    }
}
