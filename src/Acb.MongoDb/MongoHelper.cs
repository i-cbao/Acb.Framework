using Acb.Core.Domain.Entities;
using Acb.Core.Extensions;
using Acb.Core.Helper;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.GridFS;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Acb.MongoDb
{
    public class MongoHelper
    {
        private readonly string _database;
        private const string Prefix = "acb";
        private readonly MongoConfig _config;

        internal MongoHelper(MongoConfig config, string database = Prefix)
        {
            _database = database;
            _config = config;
        }

        /// <summary> Mongo服务器 </summary>
        /// <returns></returns>
        private IMongoClient Client()
        {
            var settings = new MongoClientSettings
            {
                Servers = _config.Servers.Select(t => new MongoServerAddress(t.Host, t.Port)),
                SocketTimeout = TimeSpan.FromSeconds(_config.Timeout),
                ConnectTimeout = TimeSpan.FromSeconds(_config.Timeout)
            };
            var cred = _config.Credentials.FirstOrDefault(t => t.Database == _database);
            if (cred != null)
            {
                settings.Credential = MongoCredential.CreateCredential(cred.Database, cred.User, cred.Pwd);
            }
            var client = new MongoClient(settings);
            return client;
        }

        public static string GetName<T>()
        {
            return string.Format("{0}_{1}", Prefix, typeof(T).Name.ToUrlCase());
        }

        public IMongoDatabase Database => Client().GetDatabase(_database);

        public IMongoDatabase GetDatabase(string database) => Client().GetDatabase(database);

        private IGridFSBucket _gridFs;
        public IGridFSBucket GridFs
        {
            get => _gridFs ?? (_gridFs = new GridFSBucket(Database));
            set => _gridFs = value;
        }

        /// <summary> 获取Mongo集合 </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public IMongoCollection<T> Collection<T>(string collection = null) where T : IEntity
        {
            if (string.IsNullOrWhiteSpace(collection))
            {
                collection = GetName<T>();
            }
            return Database.GetCollection<T>(collection);
        }

        /// <summary> 获取Mongo集合 </summary>
        /// <returns></returns>
        public IMongoCollection<BsonDocument> BsonCollection<T>()
        {
            var collection = GetName<T>();
            return BsonCollection(collection);
        }

        /// <summary> 获取Mongo集合 </summary>
        /// <returns></returns>
        public IMongoCollection<BsonDocument> BsonCollection(string collection)
        {
            if (string.IsNullOrWhiteSpace(collection))
            {
                throw new ArgumentException("未指定collection");
            }

            return Database.GetCollection<BsonDocument>(collection);
        }

        #region 文件管理

        /// <summary> 保存文件 </summary>
        /// <param name="fileStream"></param>
        /// <returns></returns>
        public async Task<string> SaveFile(Stream fileStream)
        {
            var name = IdentityHelper.Guid32;
            await GridFs.UploadFromStreamAsync(name, fileStream);
            return name;
        }

        /// <summary> 读取文件 </summary>
        /// <param name="fileName"></param>
        public async Task<Stream> ReadFile(string fileName)
        {
            try
            {
                var ms = new MemoryStream();
                await GridFs.DownloadToStreamByNameAsync(fileName, ms);
                return ms;
            }
            catch
            {
                return Stream.Null;
            }
        }

        public async Task<GridFSFileInfo> FindFile(string fileName)
        {
            var filter = Builders<GridFSFileInfo>.Filter.Eq(t => t.Filename, fileName);
            var result = await GridFs.FindAsync(filter);
            return await result.FirstOrDefaultAsync();
        }

        /// <summary> 删除文件 </summary>
        /// <param name="fileName"></param>
        public async Task DeleteFile(string fileName)
        {
            var info = await FindFile(fileName);
            await GridFs.DeleteAsync(info.Id);
        }

        #endregion
    }
}
