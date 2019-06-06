using Acb.Core.Exceptions;
using Acb.Core.Helper;
using Acb.Core.Logging;
using Acb.MongoDb;
using MongoDB.Bson;
using MongoDB.Driver.GridFS;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace Acb.FileServer.Helper
{
    public class GridFsManager
    {
        private readonly MongoHelper _helper;
        private readonly ILogger _logger;

        public GridFsManager(string database)
        {
            _helper = MongoManager.Instance.GetHelper($"gfs_{database}");
            _logger = LogManager.Logger<GridFsManager>();
        }

        /// <summary> 保存图片 </summary>
        /// <param name="fileStream"></param>
        /// <param name="meta"></param>
        /// <returns></returns>
        public async Task<string> SaveAsync(Stream fileStream, IDictionary<string, object> meta = null)
        {
            var name = IdentityHelper.Guid32;
            var options = new GridFSUploadOptions();
            if (meta != null)
                options.Metadata = new BsonDocument(meta);
            await _helper.GridFs.UploadFromStreamAsync(name, fileStream, options);
            return name;
        }

        /// <summary> 读取文件 </summary>
        /// <param name="fileName"></param>
        public async Task<GridFSDownloadStream<ObjectId>> Read(string fileName)
        {
            try
            {
                return await _helper.GridFs.OpenDownloadStreamByNameAsync(fileName);
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message, ex);
                throw new BusiException("文件不存在");
            }
        }

        /// <summary> 删除文件 </summary>
        /// <param name="fileName"></param>
        public Task Delete(string fileName)
        {
            return _helper.GridFs.DeleteAsync(new ObjectId(fileName));
        }
    }
}