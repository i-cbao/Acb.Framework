using FastDFS.Client;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MongoDB.Bson;
using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;

namespace Acb.Framework.Tests
{
    [TestClass]
    public class FastDFSTest
    {
        public FastDFSTest()
        {
            ConnectionManager.Initialize(new[] { new DnsEndPoint("192.168.0.231", 8512) });
        }

        [TestMethod]
        public async Task UploadTest()
        {
            var file = await File.ReadAllBytesAsync("E:\\ocr\\test1.pdf");
            var storageNode = await FastDFSClient.GetStorageNodeAsync();
            Console.WriteLine(storageNode.ToJson());
            storageNode.EndPoint = new IPEndPoint(IPAddress.Parse("192.168.0.231"), 8513);

            var path = await FastDFSClient.UploadFileAsync(storageNode, file, ".pdf");
            var uri = new Uri(new Uri("http://192.168.0.231:8502"), $"{storageNode.GroupName}/{path}");
            Console.WriteLine(uri.AbsoluteUri);
        }
    }
}
