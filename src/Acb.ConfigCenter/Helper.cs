using Acb.ConfigCenter.ViewModels;
using Microsoft.AspNetCore.Http;
using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace Acb.ConfigCenter
{
    public class Helper
    {
        private const string Key = "kd#2d^cw";
        private const string Iv = "l$d-lo%2";

        private static string Md5(string str)
        {
            var algorithm = MD5.Create();
            algorithm.ComputeHash(Encoding.UTF8.GetBytes(str));
            return BitConverter.ToString(algorithm.Hash).Replace("-", "").ToUpper();
        }

        private static string Encrypt(string inputString)
        {
            var algorithm = DES.Create();

            var desString = Encoding.UTF8.GetBytes(inputString);

            var desKey = Encoding.ASCII.GetBytes(Key);

            var desIv = Encoding.ASCII.GetBytes(Iv);

            var mStream = new MemoryStream();

            var cStream = new CryptoStream(mStream, algorithm.CreateEncryptor(desKey, desIv), CryptoStreamMode.Write);

            cStream.Write(desString, 0, desString.Length);

            cStream.FlushFinalBlock();

            cStream.Close();

            return Convert.ToBase64String(mStream.ToArray());
        }

        /// <summary> 对字符串进行对称解密 </summary>
        private static string Decrypt(string inputString)
        {
            var algorithm = DES.Create();

            var desString = Convert.FromBase64String(inputString);

            var desKey = Encoding.ASCII.GetBytes(Key);

            var desIv = Encoding.ASCII.GetBytes(Iv);

            var mStream = new MemoryStream();

            var cStream = new CryptoStream(mStream, algorithm.CreateDecryptor(desKey, desIv), CryptoStreamMode.Write);

            cStream.Write(desString, 0, desString.Length);

            cStream.FlushFinalBlock();

            cStream.Close();

            return Encoding.UTF8.GetString(mStream.ToArray());
        }

        /// <summary> 验证令牌 </summary>
        /// <param name="request"></param>
        /// <param name="dto"></param>
        /// <param name="scheme"></param>
        /// <returns></returns>
        public static bool VerifyTicket(HttpRequest request, SecurityDto dto, string scheme = "acb")
        {
            if (!request.Headers.TryGetValue("Authorization", out var authorize) ||
                string.IsNullOrWhiteSpace(authorize))
                return false;
            var arr = authorize.ToString()?.Split(' ');
            if (arr == null || arr.Length != 2 || arr[0] != scheme)
                return false;
            var ticket = arr[1];
            var author = Decrypt(ticket);
            return string.Equals(author, $"{dto.Account}__{Md5(dto.Password)}");
        }

        public static string GetTicket(SecurityDto dto)
        {
            return Encrypt($"{dto.Account}__{Md5(dto.Password)}");
        }
    }
}
