using Microsoft.AspNetCore.DataProtection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Diary.WebUI.Extensions
{
    public static class StringExtension
    {
        public static T Base64crypter<T>(this string str, bool mode)
        {
            try
            {
                if (mode)
                {
                   str = Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(str));
                }
                str = System.Text.Encoding.UTF8.GetString(Convert.FromBase64String(str));
                return (T)Convert.ChangeType(str, typeof(T));
            }
            catch
            {
                return default(T);
            }
        }

        public static string Base64crypter(this string str, bool mode) => mode ? Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(str)) : System.Text.Encoding.UTF8.GetString(Convert.FromBase64String(str));
        public static string Encrypter(this string str, string key, bool mode, IDataProtectionProvider provider) => mode ? provider.CreateProtector(key).Protect(str) : provider.CreateProtector(key).Unprotect(str);
    }
}
