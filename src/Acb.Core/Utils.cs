using Acb.Core.Extensions;
using Acb.Core.Helper;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace Acb.Core
{
    /// <summary> 通用辅助类 </summary>
    public static class Utils
    {
        /// <summary> 类型转换 </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <param name="def"></param>
        /// <returns></returns>
        public static T CastTo<T>(object obj, T def = default(T))
        {
            return obj.CastTo(def);
        }

        /// <summary> MD5 </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string Md5(string str)
        {
            return EncryptHelper.Hash(str, EncryptHelper.HashFormat.MD532);
        }

        [DllImport("wininet.dll")]
        private static extern bool InternetGetConnectedState(out int connectionDescription, int reservedValue);

        /// <summary> 是否有网络链接 </summary>
        public static bool IsNetConnected
        {
            get
            {
                int i;
                return InternetGetConnectedState(out i, 0);
            }
        }

        /// <summary> 获取真是IP </summary>
        /// <returns></returns>
        public static string GetRealIp()
        {
            return AcbHttpContext.RemoteIpAddress;
        }

        ///<summary> 判断List值相等</summary>
        ///<param name="l1"></param>
        ///<param name="l2"></param>
        ///<typeparam name="T"></typeparam>
        ///<returns></returns>
        public static bool ListEquals<T>(IEnumerable<T> l1, IEnumerable<T> l2)
        {
            if (Equals(l1, l2))
                return true;
            if (l1 == null || l2 == null)
                return false;
            List<T> list1 = l1 as List<T> ?? new List<T>(),
                list2 = l2 as List<T> ?? new List<T>();
            if (list1.Count != list2.Count)
                return false;
            return !list1.Where((t, i) => !t.Equals(list2[i])).Any();
        }

        /// <summary> 获取当前运行文件夹目录 </summary>
        /// <returns></returns>
        public static string GetCurrentDir()
        {
            var dir = AppDomain.CurrentDomain.BaseDirectory;
            // 或者 AppDomain.CurrentDomain.SetupInformation.ApplicationBase
            return dir;
        }

        /// <summary> 获得拼音缩写 </summary>
        /// <param name="cnStr"></param>
        /// <returns></returns>
        public static string GetSpellCode(string cnStr)
        {
            var strTemp = new StringBuilder();

            int iLen = cnStr.Length, i;

            for (i = 0; i <= iLen - 1; i++)
            {
                strTemp.Append(GetShortSpell(cnStr.Substring(i, 1)));
            }

            return strTemp.ToString();
        }

        /// <summary> 得到一个汉字的拼音第一个字母，如果是一个英文字母则直接返回大写字母 </summary> 
        /// <param name="cnChar">单个汉字</param> 
        /// <returns>单个大写字母</returns> 
        private static string GetShortSpell(string cnChar)
        {
            var arrCn = Encoding.Default.GetBytes(cnChar);
            if (arrCn.Length <= 1)
                return cnChar;
            int area = arrCn[0];
            int pos = arrCn[1];
            var code = (area << 8) + pos;
            int[] areacode =
            {
                45217, 45253, 45761, 46318, 46826, 47010, 47297, 47614,
                48119, 48119, 49062, 49324, 49896, 50371, 50614, 50622,
                50906, 51387, 51446, 52218, 52698, 52698, 52698, 52980, 53689, 54481
            };
            for (int i = 0; i < 26; i++)
            {
                int max = 55290;
                if (i != 25)
                    max = areacode[i + 1];
                if (areacode[i] <= code && code < max)
                    return Encoding.Default.GetString(new[] { (byte)(65 + i) });
            }
            return "*";
        }

        /// <summary> 获取0-max数组/// </summary>
        public static readonly Func<int, IEnumerable<int>> EachMax = delegate (int max)
        {
            max = Math.Abs(max);
            return Enumerable.Range(0, max);
        };

        /// <summary> 获取min-max的整数组/// </summary>
        public static readonly Func<int, int, IEnumerable<int>> Each = delegate (int min, int max)
        {
            min = Math.Min(min, max);
            return Enumerable.Range(min, Math.Abs(max - min));
        };

        /// <summary> 获取原始Url </summary>
        /// <returns></returns>
        public static string RawUrl()
        {
            if (AcbHttpContext.Current == null)
                return string.Empty;
            try
            {
                var request = AcbHttpContext.Current.Request;
                return $"{request.Scheme}://{request.Host}{request.Path.Value}{request.QueryString.Value}";
            }
            catch
            {
                return string.Empty;
            }
        }

        /// <summary> 执行命令 </summary>
        /// <param name="inputAction"></param>
        /// <param name="outputAction"></param>
        public static void ExecCommand(Action<Action<string>> inputAction, Action<string> outputAction = null)
        {
            Process pro = null;
            StreamWriter sIn = null;

            try
            {
                pro = new Process
                {
                    StartInfo =
                    {
                        FileName = "cmd.exe",
                        UseShellExecute = false,
                        CreateNoWindow = true,
                        RedirectStandardInput = true,
                        RedirectStandardOutput = true,
                        RedirectStandardError = true
                    }
                };
                if (outputAction == null)
                    outputAction = Console.WriteLine;

                pro.OutputDataReceived += (sender, e) => outputAction(e.Data);
                pro.ErrorDataReceived += (sender, e) => outputAction(e.Data);

                pro.Start();
                sIn = pro.StandardInput;
                sIn.AutoFlush = true;

                pro.BeginOutputReadLine();
                inputAction(value => sIn.WriteLine(value));

                pro.WaitForExit();
            }
            finally
            {
                if (pro != null && !pro.HasExited)
                    pro.Kill();

                if (sIn != null)
                    sIn.Close();
                if (pro != null)
                    pro.Close();
            }
        }
    }
}
