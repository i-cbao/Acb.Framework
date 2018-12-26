using Acb.Core.Helper;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using Acb.Core.Extensions;

namespace Acb.Framework.Tests
{
    [TestClass]
    public class EncryptHelperTest : DTest
    {
        private const string Content = "a=1&b=2&c=3";

        [TestMethod]
        public void Md5Test()
        {
            var str = EncryptHelper.MD5(Content);
            Print(str);
        }

        [TestMethod]
        public void RsaTest()
        {
            const EncryptHelper.RsaFormat format = EncryptHelper.RsaFormat.SHA1;
            const string pub =
                "MIIBIjANBgkqhkiG9w0BAQEFAAOCAQ8AMIIBCgKCAQEAoPCVk8sVFWDH6FwXbpTy1LN36h7v38g/XdaTpJM00e4+SRFzvyD2r+p34nU3ka6PPg9g/NmE3NpOR0HWoMWXf8FxmyObn2/6Js+0N+hDIu7ZKat3Y22LfxXP1BcoOiN3p8F/dyXdDD7RajSz6gsgef+hR+i/QQMf9fROyn6+gAFvzPavKcItAJXD8QbtVD+X4kh7cwz7P7VjpBoOc9ffWTVxenOGQNjs4KEUH0zFAq7IRdaay6E0cA1l7pwVoiUmAGteToEj1vNmzJ4RJFprCA0kJa0vX0RUfwIcJiAx5qjqXU44xByE7UtFBC594Uhnfb2ERGlPHDYllMcWiqgC9QIDAQAB";
            const string priv =
                "MIIEowIBAAKCAQEAoPCVk8sVFWDH6FwXbpTy1LN36h7v38g/XdaTpJM00e4+SRFzvyD2r+p34nU3ka6PPg9g/NmE3NpOR0HWoMWXf8FxmyObn2/6Js+0N+hDIu7ZKat3Y22LfxXP1BcoOiN3p8F/dyXdDD7RajSz6gsgef+hR+i/QQMf9fROyn6+gAFvzPavKcItAJXD8QbtVD+X4kh7cwz7P7VjpBoOc9ffWTVxenOGQNjs4KEUH0zFAq7IRdaay6E0cA1l7pwVoiUmAGteToEj1vNmzJ4RJFprCA0kJa0vX0RUfwIcJiAx5qjqXU44xByE7UtFBC594Uhnfb2ERGlPHDYllMcWiqgC9QIDAQABAoIBAGH82TVY6odPm6dvmpyrd4Xll1cmauoXY+1gXhhPwFMo+SdHxu9RxKCq4z8wGxtJ6tDgUC7iyTAmgo6EGYddhrY0M1U/wtq6NNY4lAOOGIJGZlYmndQduFcyEo2RY96yBYJQH6mNlh6EiMBfQkN3btiYubusi/WrT2RS0T4dGSOZxO1Kd9NWTfPT+MzjX68Jxc98diYgxIoprIt5bhC7e1tbW4oqjE7Y7x+fQFljrlzEyWcznsHkL++z0R7s5vm0qihWu5KIBHS5GoK5ikegYTPDpuoJA+kkuhmzNVAGG5PA22s1Dbj6yblL3ul1WT9/l+/SOLOyAGngFd8fUsVlcukCgYEA1B5TRg1QtT0phg4awI8X6Ma7HtLYBdTEJbJVCHcEzYAq/yvtjN36gsBFnP4jibj6DTfezNzIf9gYJRYEjOUCiqC+a3m+rT44FELcdnRI+/6dYmy+cypHhAvy0SgV66lZgjBNmUgDwHY+M/fEZD72ShbwNNjE1itAl9+xWw9FW0cCgYEAwjvdcKT4CSE/LHf4ro6295wo8gFwB8K/GuGt3ZfbWG2chtPG+aWPA4kVxyr9YBXpMoPruvVb206GeWb7dAMbeTkMGB+8gM5BJXmnCWcatJ7uHco603GWGnvuQiF4sQLB7s2FS0je0O/I3b1o3SDfL/JeVzJlnErqzWun3soa1eMCgYBlohYSRYT7IdAIfC+TPbyd+sJjdXCt8pD84uJdSPGTKSrw0nJigWVrSobQPvB2J5wfwzpMfCjRIJAJDPCnNX8vPu+d/VpAFcS26IZLd1IU850jMKlb8tELUtJIJuXB4YWSnGRB4kBx8fXB3qBJy+UkALOdDpPZbiXn3vVShRGdNwKBgQCoQbu+DOpg3lAfcX4XmMGfFcPSgLRPxMBPxRgcUaRNogZqNaK0OFr/MQ2QjwkW52Qvwl++gTWlcQzEthU1lkuCY4y6iSN+YS3GDPBKEOrtdI/Jdpom+MqS/rCS0PQAQudEuvyxDtsWG+HhVGc2x3cPzeEzoBCtp6hncs7lDCbbCQKBgGjqgW42ymxr9gwdR56yAoXxgg0d9MusZlmywBjK4AmN+qe5ZmK5Z5EaV4qoqkhDCyF0XtkVLzf45t6JPBsloF0TlzSF7HW6gEvfMqzwmR/ycu1hQWGz7+DDUFapk5iUIl/F0ytKLovQEiaYEfBX7NVSckaXgAYggMymKO0HRllV";

            var sign = EncryptHelper.RsaSignature(Content, priv, signType: format);
            Print(sign);
            var verify = EncryptHelper.RsaVerifySign(Content, sign, pub, signType: format);
            Print(verify);
        }

        [TestMethod]
        public void HashTest()
        {
            foreach (EncryptHelper.HashFormat format in Enum.GetValues(typeof(EncryptHelper.HashFormat)))
            {
                var str = EncryptHelper.Hash(Content, format);
                Print($"{format}:{str}");
            }
        }

        [TestMethod]
        public void HmacTest()
        {
            const string key = "shay168";
            foreach (EncryptHelper.HmacFormat format in Enum.GetValues(typeof(EncryptHelper.HmacFormat)))
            {
                var str = EncryptHelper.Hmac(Content, key, format);
                Print($"{format}:{str}");
            }
        }

        [TestMethod]
        public void SymmetricTest()
        {
            foreach (EncryptHelper.SymmetricFormat format in Enum.GetValues(typeof(EncryptHelper.SymmetricFormat)))
            {
                int keyLen, ivLen;
                switch (format)
                {
                    case EncryptHelper.SymmetricFormat.DES:
                        keyLen = ivLen = 8;
                        break;
                    case EncryptHelper.SymmetricFormat.Rijndael:
                    case EncryptHelper.SymmetricFormat.AES:
                        keyLen = 32;
                        ivLen = 16;
                        break;
                    case EncryptHelper.SymmetricFormat.RC2:
                        keyLen = 16;
                        ivLen = 8;
                        break;
                    case EncryptHelper.SymmetricFormat.TripleDES:
                        keyLen = 24;
                        ivLen = 8;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }

                var key = RandomHelper.RandomNumAndLetters(keyLen);
                var iv = RandomHelper.RandomNumAndLetters(ivLen);
                Print(new { key, iv });
                //加密
                var str = EncryptHelper.SymmetricEncrypt(Content, format, key, iv);
                Print($"{format} Encrypt:{str}");
                //解密
                str = EncryptHelper.SymmetricDecrypt(str, format, key, iv);
                Print($"{format} Decrypt:{str}");
            }
        }
    }
}
