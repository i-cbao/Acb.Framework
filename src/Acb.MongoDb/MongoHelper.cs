using Acb.Core.Domain.Entities;
using Acb.Core.Extensions;
using Acb.Core.Helper;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.GridFS;
using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace Acb.MongoDb
{
    public class MongoHelper
    {
        private readonly string _prefix;
        private readonly MongoConfig _config;

        public MongoHelper(MongoConfig config = null, string prefix = "acb")
        {
            _prefix = prefix;
            _config = config ?? MongoConfig.Config();
        }

        /// <summary> Mongo服务器 </summary>
        /// <returns></returns>
        private IMongoClient Client()
        {
            var settings = new MongoClientSettings
            {
                Servers = _config.Servers.Select(t => new MongoServerAddress(t.Host, t.Port)),
                SocketTimeout = TimeSpan.FromSeconds(_config.Timeout),
                MaxConnectionPoolSize = _config.PoolSize, //default 100
                ConnectTimeout = TimeSpan.FromSeconds(_config.Timeout)
            };
            if (_config.WaitSize > _config.PoolSize)
                settings.WaitQueueSize = _config.WaitSize;
            var cred = _config.Credentials.FirstOrDefault();
            if (cred != null)
            {
                settings.Credential = MongoCredential.CreateCredential(cred.Database, cred.User, cred.Pwd);
            }
            var client = new MongoClient(settings);
            return client;
        }

        protected virtual string GetTypeName(MemberInfo type)
        {
            var name = type.PropName();
            return string.IsNullOrWhiteSpace(_prefix) ? name : $"{_prefix}_{name}";
        }

        public IMongoDatabase Database => GetDatabase(_config.Database);

        public IMongoDatabase GetDatabase(string database, MongoDatabaseSettings settings = null)
        {
            if (string.IsNullOrWhiteSpace(database))
                throw new ArgumentException("数据库名称不能为空", nameof(database));
            return Client().GetDatabase(database, settings);
        }

        private IGridFSBucket _gridFs;
        public IGridFSBucket GridFs
        {
            get => _gridFs ?? (_gridFs = new GridFSBucket(Database));
            set => _gridFs = value;
        }

        public IGridFSBucket GetGridFs(string database)
        {
            return new GridFSBucket(GetDatabase(database));
        }

        /// <summary> 获取Mongo集合 </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public IMongoCollection<T> Collection<T>(string collection = null) where T : IEntity
        {
            if (string.IsNullOrWhiteSpace(collection))
            {
                collection = GetTypeName(typeof(T));
            }
            return Database.GetCollection<T>(collection);
        }

        /// <summary> 获取Mongo集合 </summary>
        /// <returns></returns>
        public IMongoCollection<BsonDocument> BsonCollection<T>()
        {
            var collection = GetTypeName(typeof(T));
            return BsonCollection(collection);
        }

        /// <summary> 获取Mongo集合 </summary>
        /// <returns></returns>
        public IMongoCollection<BsonDocument> BsonCollection(string collection)
        {
            if (string.IsNullOrWhiteSpace(collection))
                throw new ArgumentException("集合名称不能为空", nameof(collection));
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
