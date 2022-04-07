using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace WkXamarinToolBox.Extensions
{
    public static class StringExtension
    {
        public static bool IsNullEmptyOrWhiteSpace(this string str) =>
            string.IsNullOrEmpty(str) || string.IsNullOrWhiteSpace(str);

        public static string ToMd5Hash(this string value)
        {
            if (string.IsNullOrEmpty(value))
                return "";

            var source = value;

            using (var md5Hash = MD5.Create())
            {
                var uiid = value;

                var i = uiid.ToCharArray().Sum(f => (int)f);

                var hash = GetMd5Hash(md5Hash, source);

                for (var l = 0; l < i; l++)
                    hash = GetMd5Hash(md5Hash, hash);

                return hash;
            }
        }

        private static string GetMd5Hash(MD5 md5Hash, string input)
        {
            byte[] data = md5Hash.ComputeHash(Encoding.UTF8.GetBytes(input));

            StringBuilder sBuilder = new();

            for (int i = 0; i < data.Length; i++)
            {
                sBuilder.Append(data[i].ToString("x2"));
            }

            return sBuilder.ToString();
        }
    }
}
