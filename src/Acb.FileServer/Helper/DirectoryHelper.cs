using Acb.FileServer.Enums;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Acb.FileServer.Helper
{
    /// <summary> 文件结构辅助类 </summary>
    public class DirectoryHelper
    {
        /// <summary> 文件夹集合 </summary>
        private static IDictionary<FileType, string> _directoryCache;

        /// <summary> 文件集合 </summary>
        private static IDictionary<FileType, int> _fileCountCache;

        public DirectoryHelper()
        {
            _directoryCache = new Dictionary<FileType, string>(10);
            _fileCountCache = new Dictionary<FileType, int>(10);
            foreach (FileType type in Enum.GetValues(typeof(FileType)))
            {
                InitDirectory(type);
            }
        }

        /// <summary> 初始化类型文件夹 </summary>
        /// <param name="type"></param>
        private void InitDirectory(FileType type)
        {
            var path = Path.Combine(Constants.BaseDirectory, type.ToString().ToLower());
            if (!Directory.Exists(path))
            {
                //初始化类型文件
                path = Path.Combine(path, GenerateFirstDirectory(), Util.GenerateFileName());
                _directoryCache[type] = path;
                _fileCountCache[type] = 0;
                return;
            }
            var parent = new DirectoryInfo(path);
            var dir = parent.GetDirectories().OrderByDescending(t => t.LastWriteTime).FirstOrDefault();
            //判断二级目录
            if (dir == null || dir.GetDirectories().Length >= Constants.MaxFileCount)
            {
                var count = 1;
                if (dir != null)
                {
                    var search = $"d{Convert.ToString(DateTime.Now.Year, 16)}*";
                    count = parent.GetDirectories(search).Length + 1;
                }
                _directoryCache[type] = Path.Combine(path, GenerateFirstDirectory(count), Util.GenerateFileName());
                _fileCountCache[type] = 0;
                return;
            }
            path = dir.FullName;
            dir = dir.GetDirectories().OrderByDescending(t => t.LastWriteTime).FirstOrDefault();
            if (dir == null || dir.GetFiles().Length >= Constants.MaxFileCount)
            {
                _directoryCache[type] = Path.Combine(path, Util.GenerateFileName());
                _fileCountCache[type] = 0;
                return;
            }
            _directoryCache[type] = dir.FullName;
            _fileCountCache[type] = dir.GetFiles().Length;
        }

        /// <summary> 生成一级目录 </summary>
        /// <param name="count"></param>
        /// <returns></returns>
        private string GenerateFirstDirectory(int count = 1)
        {
            if (count < 1)
                count = 1;
            return string.Format(Constants.FirstDirectory, Convert.ToString(DateTime.Now.Year, 16),
                Convert.ToString(count, 16).PadLeft(3, 'v'));
        }

        public string GetCurrentDirectory(FileType fileType, bool create = false)
        {
            var count = _fileCountCache[fileType];
            if (count >= Constants.MaxFileCount)
                InitDirectory(fileType);
            else
            {
                _fileCountCache[fileType] += 1;
            }
            var path = _directoryCache[fileType];
            if (create && !Directory.Exists(path))
                Directory.CreateDirectory(path);
            return path;
        }
    }
}