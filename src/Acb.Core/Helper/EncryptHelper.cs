using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace Acb.Core.Helper
{
    /// <summary> 加密和解密 </summary>
    public static class EncryptHelper
    {
        #region 加密类型
        /// <summary>
        /// Hash 加密采用的算法
        /// </summary>
        public enum HashFormat
        {
            MD516,
            MD532,
            //RIPEMD160,
            SHA1,
            SHA256,
            SHA384,
            SHA512
        }

        /// <summary>
        /// 基于密钥的 Hash 加密采用的算法
        /// </summary>
        public enum HmacFormat
        {
            HMACMD5,
            //HMACRIPEMD160,
            HMACSHA1,
            HMACSHA256,
            HMACSHA384,
            HMACSHA512
        }

        /// <summary>
        /// 对称加密采用的算法
        /// </summary>
        public enum SymmetricFormat
        {
            /// <summary>
            /// 有效的 KEY 与 IV 长度，以英文字符为单位： KEY（Min:8 Max:8 Skip:0），IV（8）
            /// </summary>
            DES,
            /// <summary>
            /// 有效的 KEY 与 IV 长度，以英文字符为单位： KEY（Min:16 Max:24 Skip:8），IV（8）
            /// </summary>
            TripleDES,
            /// <summary>
            /// 有效的 KEY 与 IV 长度，以英文字符为单位： KEY（Min:5 Max:16 Skip:1），IV（8）
            /// </summary>
            RC2,
            /// <summary>
            /// 有效的 KEY 与 IV 长度，以英文字符为单位： KEY（Min:16 Max:32 Skip:8），IV（16）
            /// </summary>
            Rijndael,
            /// <summary>
            /// 有效的 KEY 与 IV 长度，以英文字符为单位： KEY（Min:16 Max:32 Skip:8），IV（16）
            /// </summary>
            AES
        }
        #endregion

        /// <summary> 对字符串进行 Hash 加密 </summary>
        public static string Hash(string inputString, HashFormat hashFormat = HashFormat.SHA1)
        {
            var algorithm = GetHashAlgorithm(hashFormat);

            algorithm.ComputeHash(Encoding.UTF8.GetBytes(inputString));

            if (hashFormat == HashFormat.MD516)
                return BitConverter.ToString(algorithm.Hash).Replace("-", "").Substring(8, 16).ToUpper();

            return BitConverter.ToString(algorithm.Hash).Replace("-", "").ToUpper();
        }

        /// <summary> 对字符串进行基于密钥的 Hash 加密 </summary>
        /// <param name="inputString"></param>
        /// <param name="key">密钥的长度不限，建议的密钥长度为 64 个英文字符。</param>
        /// <param name="hashFormat"></param>
        /// <returns></returns>
        public static string Hmac(string inputString, string key, HmacFormat hashFormat = HmacFormat.HMACSHA1)
        {
            var algorithm = GetHmac(hashFormat, Encoding.ASCII.GetBytes(key));

            algorithm.ComputeHash(Encoding.UTF8.GetBytes(inputString));

            return BitConverter.ToString(algorithm.Hash).Replace("-", "").ToUpper();
        }

        /// <summary> 对字符串进行对称加密 </summary>
        public static string SymmetricEncrypt(string inputString, SymmetricFormat symmetricFormat, string key,
            string iv)
        {
            var algorithm = GetSymmetricAlgorithm(symmetricFormat);

            var desString = Encoding.UTF8.GetBytes(inputString);

            var desKey = Encoding.ASCII.GetBytes(key);

            var desIv = Encoding.ASCII.GetBytes(iv);

            if (!algorithm.ValidKeySize(desKey.Length * 8))
                throw new ArgumentOutOfRangeException("key");

            if (algorithm.IV.Length != desIv.Length)
                throw new ArgumentOutOfRangeException("iv");

            var mStream = new MemoryStream();

            var cStream = new CryptoStream(mStream, algorithm.CreateEncryptor(desKey, desIv), CryptoStreamMode.Write);

            cStream.Write(desString, 0, desString.Length);

            cStream.FlushFinalBlock();

            cStream.Close();

            return Convert.ToBase64String(mStream.ToArray());
        }

        /// <summary> 对字符串进行对称解密 </summary>
        public static string SymmetricDecrypt(string inputString, SymmetricFormat symmetricFormat, string key,
            string iv)
        {
            var algorithm = GetSymmetricAlgorithm(symmetricFormat);

            var desString = Convert.FromBase64String(inputString);

            var desKey = Encoding.ASCII.GetBytes(key);

            var desIv = Encoding.ASCII.GetBytes(iv);

            var mStream = new MemoryStream();

            var cStream = new CryptoStream(mStream, algorithm.CreateDecryptor(desKey, desIv), CryptoStreamMode.Write);

            cStream.Write(desString, 0, desString.Length);

            cStream.FlushFinalBlock();

            cStream.Close();

            return Encoding.UTF8.GetString(mStream.ToArray());
        }

        /// <summary> 使用 RSA 公钥加密 </summary>
        public static string RsaEncrypt(string message, string publicKey)
        {
            using (var rsa = new RSACryptoServiceProvider())
            {
                rsa.FromXmlString(publicKey);

                var messageBytes = Encoding.UTF8.GetBytes(message);

                var resultBytes = rsa.Encrypt(messageBytes, false);

                return Convert.ToBase64String(resultBytes);
            }
        }

        /// <summary> 使用 RSA 私钥解密 </summary>
        public static string RsaDecrypt(string message, string privateKey)
        {
            using (var rsa = new RSACryptoServiceProvider())
            {
                rsa.FromXmlString(privateKey);

                var messageBytes = Convert.FromBase64String(message);

                var resultBytes = rsa.Decrypt(messageBytes, false);

                return Encoding.UTF8.GetString(resultBytes);
            }
        }

        #region RSA私有方法
        private static int GetIntegerSize(BinaryReader binr)
        {
            byte bt = 0;
            byte lowbyte = 0x00;
            byte highbyte = 0x00;
            int count = 0;
            bt = binr.ReadByte();
            if (bt != 0x02)
            {
                return 0;
            }

            bt = binr.ReadByte();

            if (bt == 0x81)
            {
                count = binr.ReadByte();
            }
            else if (bt == 0x82)
            {
                highbyte = binr.ReadByte();
                lowbyte = binr.ReadByte();
                byte[] modint = { lowbyte, highbyte, 0x00, 0x00 };
                count = BitConverter.ToInt32(modint, 0);
            }
            else
            {
                count = bt;
            }

            while (binr.ReadByte() == 0x00)
            {
                count -= 1;
            }
            binr.BaseStream.Seek(-1, SeekOrigin.Current);
            return count;
        }
        private static RSA DecodeRSAPrivateKey(byte[] privkey, string signType)
        {
            byte[] MODULUS, E, D, P, Q, DP, DQ, IQ;
            MemoryStream mem = new MemoryStream(privkey);
            BinaryReader binr = new BinaryReader(mem);
            byte bt = 0;
            ushort twobytes = 0;
            int elems = 0;
            try
            {
                twobytes = binr.ReadUInt16();
                if (twobytes == 0x8130)
                {
                    binr.ReadByte();
                }
                else if (twobytes == 0x8230)
                {
                    binr.ReadInt16();
                }
                else
                {
                    return null;
                }

                twobytes = binr.ReadUInt16();
                if (twobytes != 0x0102)
                {
                    return null;
                }

                bt = binr.ReadByte();
                if (bt != 0x00)
                {
                    return null;
                }

                elems = GetIntegerSize(binr);
                MODULUS = binr.ReadBytes(elems);

                elems = GetIntegerSize(binr);
                E = binr.ReadBytes(elems);

                elems = GetIntegerSize(binr);
                D = binr.ReadBytes(elems);

                elems = GetIntegerSize(binr);
                P = binr.ReadBytes(elems);

                elems = GetIntegerSize(binr);
                Q = binr.ReadBytes(elems);

                elems = GetIntegerSize(binr);
                DP = binr.ReadBytes(elems);

                elems = GetIntegerSize(binr);
                DQ = binr.ReadBytes(elems);

                elems = GetIntegerSize(binr);
                IQ = binr.ReadBytes(elems);

                int bitLen = 1024;
                if ("RSA2".Equals(signType))
                {
                    bitLen = 2048;
                }

                RSA RSA = RSA.Create();
                RSA.KeySize = bitLen;
                RSAParameters RSAparams = new RSAParameters
                {
                    Modulus = MODULUS,
                    Exponent = E,
                    D = D,
                    P = P,
                    Q = Q,
                    DP = DP,
                    DQ = DQ,
                    InverseQ = IQ
                };
                RSA.ImportParameters(RSAparams);
                return RSA;
            }
            catch (Exception)
            {
                return null;
            }
            finally
            {
                binr.Close();
            }
        }

        private static bool CompareBytearrays(byte[] a, byte[] b)
        {
            if (a.Length != b.Length)
            {
                return false;
            }
            int i = 0;
            foreach (byte c in a)
            {
                if (c != b[i])
                {
                    return false;
                }
                i++;
            }
            return true;
        }

        private static RSA CreateRsaProviderFromPublicKey(string publicKeyString, string signType)
        {
            byte[] seqOid = { 0x30, 0x0D, 0x06, 0x09, 0x2A, 0x86, 0x48, 0x86, 0xF7, 0x0D, 0x01, 0x01, 0x01, 0x05, 0x00 };
            byte[] seq = new byte[15];

            var x509Key = Convert.FromBase64String(publicKeyString);
            using (MemoryStream mem = new MemoryStream(x509Key))
            {
                using (BinaryReader binr = new BinaryReader(mem))
                {
                    byte bt = 0;
                    ushort twobytes = 0;

                    twobytes = binr.ReadUInt16();
                    if (twobytes == 0x8130)
                    {
                        binr.ReadByte();
                    }
                    else if (twobytes == 0x8230)
                    {
                        binr.ReadInt16();
                    }
                    else
                    {
                        return null;
                    }

                    seq = binr.ReadBytes(15);
                    if (!CompareBytearrays(seq, seqOid))
                    {
                        return null;
                    }

                    twobytes = binr.ReadUInt16();
                    if (twobytes == 0x8103)
                    {
                        binr.ReadByte();
                    }
                    else if (twobytes == 0x8203)
                    {
                        binr.ReadInt16();
                    }
                    else
                    {
                        return null;
                    }

                    bt = binr.ReadByte();
                    if (bt != 0x00)
                    {
                        return null;
                    }

                    twobytes = binr.ReadUInt16();
                    if (twobytes == 0x8130)
                    {
                        binr.ReadByte();
                    }
                    else if (twobytes == 0x8230)
                    {
                        binr.ReadInt16();
                    }
                    else
                    {
                        return null;
                    }

                    twobytes = binr.ReadUInt16();
                    byte lowbyte = 0x00;
                    byte highbyte = 0x00;

                    if (twobytes == 0x8102)
                    {
                        lowbyte = binr.ReadByte();
                    }
                    else if (twobytes == 0x8202)
                    {
                        highbyte = binr.ReadByte();
                        lowbyte = binr.ReadByte();
                    }
                    else
                    {
                        return null;
                    }
                    byte[] modint = { lowbyte, highbyte, 0x00, 0x00 };
                    int modsize = BitConverter.ToInt32(modint, 0);

                    int firstbyte = binr.PeekChar();
                    if (firstbyte == 0x00)
                    {
                        binr.ReadByte();
                        modsize -= 1;
                    }

                    byte[] modulus = binr.ReadBytes(modsize);

                    if (binr.ReadByte() != 0x02)
                    {
                        return null;
                    }
                    int expbytes = binr.ReadByte();
                    byte[] exponent = binr.ReadBytes(expbytes);

                    RSA rsa = RSA.Create();
                    rsa.KeySize = signType == "RSA" ? 1024 : 2048;
                    RSAParameters rsaKeyInfo = new RSAParameters
                    {
                        Modulus = modulus,
                        Exponent = exponent
                    };
                    rsa.ImportParameters(rsaKeyInfo);

                    return rsa;
                }
            }
        }
        #endregion

        /// <summary> 使用 RSA 私钥签名 </summary>
        public static string RsaSignature(string message, string privateKey, string charset = "utf-8", string signType = "RSA")
        {
            byte[] signatureBytes = null;
            try
            {
                byte[] data = Convert.FromBase64String(privateKey);
                var rsa = DecodeRSAPrivateKey(data, signType);
                using (rsa)
                {
                    var messageBytes = Encoding.GetEncoding(charset).GetBytes(message);
                    if ("RSA2".Equals(signType))
                        signatureBytes = rsa.SignData(messageBytes, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);
                    else
                        signatureBytes = rsa.SignData(messageBytes, HashAlgorithmName.SHA1, RSASignaturePadding.Pkcs1);
                }
            }
            catch (Exception)
            {
                throw new Exception($"您使用的私钥格式错误，请检查RSA私钥配置,charset = {charset}");
            }
            return Convert.ToBase64String(signatureBytes);
        }

        /// <summary>
        /// 使用 RSA 公钥验证签名
        /// </summary>
        public static bool RsaVerifySign(string message, string signature, string publicKey, string charset = "utf-8", string signType = "RSA")
        {
            try
            {
                string sPublicKeyPEM = publicKey;
                var rsa = CreateRsaProviderFromPublicKey(sPublicKeyPEM, signType);
                bool bVerifyResultOriginal = false;

                if ("RSA2".Equals(signType))
                {
                    bVerifyResultOriginal = rsa.VerifyData(Encoding.GetEncoding(charset).GetBytes(message),
                       Convert.FromBase64String(signature), HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);
                }
                else
                {
                    bVerifyResultOriginal = rsa.VerifyData(Encoding.GetEncoding(charset).GetBytes(message),
                       Convert.FromBase64String(signature), HashAlgorithmName.SHA1, RSASignaturePadding.Pkcs1);
                }
                return bVerifyResultOriginal;
            }
            catch
            {
                return false;
            }
        }

        /// <summary> 为 RSA 非对称加密生成密钥对，并存储到文件 </summary>
        public static void CreateKeyFileForAsymmetricAlgorithm(string publicFileName, string privateFileName)
        {
            if (string.IsNullOrEmpty(publicFileName)) throw new ArgumentNullException("publicFileName");

            if (string.IsNullOrEmpty(privateFileName)) throw new ArgumentNullException("privateFileName");

            using (var rsa = new RSACryptoServiceProvider())
            {
                File.WriteAllText(publicFileName, rsa.ToXmlString(false));
                File.WriteAllText(privateFileName, rsa.ToXmlString(true));
            }
        }

        /// <summary> 为非对称加密从文件读取密钥 </summary>
        public static string GetKeyFromFileForAsymmetricAlgorithm(string fileName)
        {
            if (string.IsNullOrEmpty(fileName)) throw new ArgumentNullException("fileName");

            return File.ReadAllText(fileName);
        }

        /// <summary> 获取 Hash 加密方法 </summary>
        private static HashAlgorithm GetHashAlgorithm(HashFormat hashFormat)
        {
            HashAlgorithm algorithm = null;

            switch (hashFormat)
            {
                case HashFormat.MD516:
                case HashFormat.MD532:
                    algorithm = System.Security.Cryptography.MD5.Create();
                    break;
                //case HashFormat.RIPEMD160:
                //    algorithm = RIPEMD160.Create();
                //    break;
                case HashFormat.SHA1:
                    algorithm = SHA1.Create();
                    break;
                case HashFormat.SHA256:
                    algorithm = SHA256.Create();
                    break;
                case HashFormat.SHA384:
                    algorithm = SHA384.Create();
                    break;
                case HashFormat.SHA512:
                    algorithm = SHA512.Create();
                    break;
            }

            return algorithm;
        }

        /// <summary> 获取基于密钥的 Hash 加密方法 </summary>
        private static HMAC GetHmac(HmacFormat hmacFormat, byte[] key)
        {
            HMAC hmac = null;

            switch (hmacFormat)
            {
                case HmacFormat.HMACMD5:
                    hmac = new HMACMD5(key);
                    break;
                //case HmacFormat.HMACRIPEMD160:
                //    hmac = new HMACRIPEMD160(key);
                //    break;
                case HmacFormat.HMACSHA1:
                    hmac = new HMACSHA1(key);
                    break;
                case HmacFormat.HMACSHA256:
                    hmac = new HMACSHA256(key);
                    break;
                case HmacFormat.HMACSHA384:
                    hmac = new HMACSHA384(key);
                    break;
                case HmacFormat.HMACSHA512:
                    hmac = new HMACSHA512(key);
                    break;
            }

            return hmac;
        }

        /// <summary> 获取对称加密方法 </summary>
        private static SymmetricAlgorithm GetSymmetricAlgorithm(SymmetricFormat symmetricFormat)
        {
            SymmetricAlgorithm algorithm = null;

            switch (symmetricFormat)
            {
                case SymmetricFormat.DES:
                    algorithm = DES.Create();
                    break;
                case SymmetricFormat.TripleDES:
                    algorithm = TripleDES.Create();
                    break;
                case SymmetricFormat.RC2:
                    algorithm = RC2.Create();
                    break;
                case SymmetricFormat.Rijndael:
                    algorithm = Rijndael.Create();
                    break;
                case SymmetricFormat.AES:
                    algorithm = Aes.Create();
                    break;
            }

            return algorithm;
        }

        /// <summary>
        /// MD5加密
        /// </summary>
        /// <param name="data">数据</param>
        public static string MD5(string data)
        {
            return MD5(data, Encoding.UTF8);
        }

        /// <summary>
        /// MD5加密
        /// </summary>
        /// <param name="data">数据</param>
        /// <param name="encoding">编码</param>
        /// <returns></returns>
        public static string MD5(string data, Encoding encoding)
        {
            var md5 = System.Security.Cryptography.MD5.Create();
            byte[] dataByte = md5.ComputeHash(encoding.GetBytes(data));
            var sb = new StringBuilder();
            for (int i = 0; i < dataByte.Length; i++)
            {
                sb.Append(dataByte[i].ToString("X2"));
            }

            return sb.ToString();
        }
    }
}