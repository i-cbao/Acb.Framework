using Acb.Core.Extensions;
using Aliyun.OSS;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;

namespace Acb.Framework.Tests
{
    [TestClass]
    public class OssTest : DTest
    {
        public class OssConfig
        {
            public string Endpoint { get; set; }
            public string KeyId { get; set; }
            public string KeySecret { get; set; }
            public Dictionary<string, OssBucket> Buckets { get; set; }
        }
        public class OssBucket
        {
            public string Name { get; set; }
            public string Host { get; set; }
        }

        internal const string DefaultBucket = "default";
        internal const string PrivateBucket = "private";

        private readonly OssConfig _ossConfig;
        public OssTest()
        {
            _ossConfig = "oss".Config<OssConfig>();
        }

        [TestMethod]
        public void Test()
        {
            Print(_ossConfig);
            var bucket = _ossConfig.Buckets[DefaultBucket];
            const string key = "icb.png";
            var client = new OssClient(_ossConfig.Endpoint, _ossConfig.KeyId, _ossConfig.KeySecret);
            //获取所有存储空间
            var list = client.ListBuckets();
            Print(list);
            var alc = client.GetBucketAcl(bucket.Name);
            Print(alc);
            var obj = client.GetObjectMetadata(bucket.Name, key);
            Print(obj);
            //var bucket = _ossConfig.Buckets[DefaultBucket];
            //var baseUri = new Uri(bucket.Host);
            //using (var ms = new FileStream("d://20170227.png", FileMode.Open))
            //{
            //    
            //    var result = client.PutObject(new PutObjectRequest(bucket.Name, key, ms));
            //    Print(result);
            //    Print(new Uri(baseUri, key).AbsoluteUri);
            //}
        }
    }
}
