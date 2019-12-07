using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

namespace Emprise.Infra.Extensions
{
    public static class StringExtension
    {
        public static bool Eq(this string input, string toCompare, StringComparison comparison = StringComparison.OrdinalIgnoreCase)
        {
            if (input == null)
            {
                return toCompare == null;
            }
            return input.Equals(toCompare, comparison);
        }
        
        public static Guid? ToGuid(this string str)
        {
            if (string.IsNullOrEmpty(str))
            {
                return null;
            }
            Guid id;
            if (Guid.TryParse(str, out id))
            {
                return id;
            }
            return null;
        }

        public static int? ToInt32(this string str)
        {
            int value;
            if (int.TryParse(str, out value))
            {
                return value;
            }
            return null;
        }
        
        public static bool IsEmail(this string value)
        {
            var reg = new Regex(@"\w+([-+.]\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*");
            return string.IsNullOrEmpty(value) == false && reg.IsMatch(value);
        }

        public static string ToMd5(this string str, string salt)
        {
            byte[] b = Encoding.UTF8.GetBytes($"{str}{salt}");
            b = new MD5CryptoServiceProvider().ComputeHash(b);
            string ret = "";
            for (int i = 0; i < b.Length; i++)
            {
                ret = ret + b[i].ToString("x").PadLeft(2, '0');
            }
            return ret;
        }


        /// <summary> 
        /// DES加密 
        /// </summary> 
        /// <param name="text">待加密的字符串</param> 
        /// <param name="sKey">加密密钥</param> 
        /// <returns>加密后的字符串</returns> 
        public static string ToDesEncrypt(this string text, string sKey= "game@emprise.com")
        {
            byte[] inputArray = Encoding.UTF8.GetBytes(text);
            var tripleDES = TripleDES.Create();
            var byteKey = Encoding.UTF8.GetBytes(sKey);
            tripleDES.Key = byteKey;
            tripleDES.Mode = CipherMode.ECB;
            tripleDES.Padding = PaddingMode.PKCS7;
            ICryptoTransform cTransform = tripleDES.CreateEncryptor();
            byte[] resultArray = cTransform.TransformFinalBlock(inputArray, 0, inputArray.Length);
            return Convert.ToBase64String(resultArray, 0, resultArray.Length);
        }


        /// <summary> 
        /// DES解密 
        /// </summary> 
        /// <param name="text">待解密的字符串</param> 
        /// <param name="sKey">解密密钥</param> 
        /// <returns>解密后的字符串</returns> 
        public static string ToDesDecrypt(this string text, string sKey = "game@emprise.com")
        {
            try
            {
                byte[] inputArray = Convert.FromBase64String(text);
                var tripleDES = TripleDES.Create();
                var byteKey = Encoding.UTF8.GetBytes(sKey);
                tripleDES.Key = byteKey;
                tripleDES.Mode = CipherMode.ECB;
                tripleDES.Padding = PaddingMode.PKCS7;
                ICryptoTransform cTransform = tripleDES.CreateDecryptor();
                byte[] resultArray = cTransform.TransformFinalBlock(inputArray, 0, inputArray.Length);
                return Encoding.UTF8.GetString(resultArray);

            }
            catch
            {
                return "";
            }
        }


        public static string JoinString(this IEnumerable<string> values)
        {
            return JoinString(values, ",");
        }

        public static string JoinString(this IEnumerable<string> values, string split)
        {
            var result = values.Aggregate(string.Empty, (current, value) => current + (split + value));
            result = result.TrimStart(split.ToCharArray());
            return result;
        }



        /// <summary>
        /// 字符串分隔转换为List
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="str"></param>
        /// <param name="separator"></param>
        /// <returns></returns>
        public static List<T> StrToList<T>(this string str, char separator = ',')
        {
            var list = new List<T>();
            if (string.IsNullOrEmpty(str))
            {
                return list;
            }
            foreach (var c in str.Split(separator))
            {
                if (c.Length == 0) { continue; }

                try
                {
                    T result = (T)Convert.ChangeType(c, typeof(T));
                    if (result == null) { continue; }
                    list.Add(result);
                }
                catch (Exception ex)
                {

                }

            }
            return list;
        }

        public static bool IsChinese(this string text)
        {
            Regex rx = new Regex("^[\u4e00-\u9fbb]{0,}$");
            return rx.IsMatch(text);
        }

        public static bool IsNullOrEmpty(this string s)
        {
            return string.IsNullOrEmpty(s);
        }

        
        private static char[] constant =
        {
            'a','b','c','d','e','f','g','h','i','j','k','l','m','n','o','p','q','r','s','t','u','v','w','x','y','z'
        };

        public static string GenerateRandom(this String obj, int Length)
        {
            StringBuilder newRandom = new StringBuilder(constant.Length);
            Random rd = new Random();
            for (int i = 0; i < Length; i++)
            {
                newRandom.Append(constant[rd.Next(constant.Length)]);
            }
            return newRandom.ToString();
        }
    }
}
