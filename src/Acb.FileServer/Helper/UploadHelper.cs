using Acb.Core.Exceptions;
using Acb.FileServer.Enums;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace Acb.FileServer.Helper
{
    /// <summary> 上传文件辅助 </summary>
    public class UploadHelper
    {
        private readonly DirectoryHelper _directoryHelper;
        private readonly FileType _fileType;

        public UploadHelper(DirectoryHelper directoryHelper, FileType fileType = FileType.Image)
        {
            _directoryHelper = directoryHelper;
            _fileType = fileType;
        }

        /// <summary> form表单文件检测 </summary>
        /// <returns></returns>
        public void CheckFormFile(IFormFileCollection files)
        {
            //不是Form表单提交
            if (files.Count == 0)
                throw new BusiException("请上传文件");
            foreach (var formFile in files)
            {
                if (formFile == null)
                {
                    throw new BusiException("文件不存在！");
                }

                _fileType.CheckExtAndSize(formFile.Length, formFile.FileName);
            }
        }

        /// <summary> 文件系统保存 </summary>
        public async Task<Dictionary<string, string>> FileSave(IFormFileCollection files)
        {
            var list = new Dictionary<string, string>();
            foreach (var file in files)
            {
                if (file == null) continue;
                string fileName;
                if (_fileType == FileType.ZipFile)
                {
                    fileName = Path.Combine(Constants.BaseDirectory, "upload/paper/marking",
                        DateTime.Now.ToString("yyyyMM"),
                        Guid.NewGuid().ToString("N"));
                    var stream = file.OpenReadStream();
                    await Util.UnZipFile(stream, fileName);
                }
                else
                {
                    var path = _directoryHelper.GetCurrentDirectory(_fileType, true);
                    fileName = Util.GenerateFileName(Path.GetExtension(file.FileName));
                    fileName = Path.Combine(path, fileName);
                    using (var fs = File.Create(fileName))
                    {
                        await file.CopyToAsync(fs);
                        fs.Flush();
                    }
                }

                list.Add(file.Name,
                    fileName.Replace(Constants.BaseDirectory, Constants.BaseUrl.TrimStart('/')).Replace("\\", "/"));
            }
            return list;
        }

        /// <summary> GridFS保存 </summary>
        /// <returns></returns>
        public async Task<Dictionary<string, string>> GridFsSave(IFormFileCollection files)
        {
            var database = _fileType == FileType.Image ? "picture" : "attach";
            var manager = new GridFsManager(database);
            var list = new Dictionary<string, string>();
            foreach (var formFile in files)
            {
                if (formFile == null) continue;
                var meta = new Dictionary<string, object>
                {
                    {"ContentType", formFile.ContentType}
                };
                var fileName = await manager.SaveAsync(formFile.OpenReadStream(), meta);
                var ext = Path.GetExtension(formFile.FileName);
                var uri = new Uri(new Uri(Constants.BaseUrl), $"/gfs/{database}/{fileName}{ext}");
                list.Add(formFile.Name, uri.AbsoluteUri);
            }
            return list;
        }
    }

}