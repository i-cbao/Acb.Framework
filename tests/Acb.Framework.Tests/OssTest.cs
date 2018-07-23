using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Aliyun.OSS;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Acb.Framework.Tests
{
    [TestClass]
    public class OssTest : DTest
    {
        [TestMethod]
        public void Test()
        {
            var client = new OssClient("https://oss-cn-shenzhen.aliyuncs.com", "ONYqXWi0Mi5tJEnG", "nE4drWHmU2Jie3TbGHIGuEi6kfLQHJ");
            using (var ms = new FileStream("d://20170227.png", FileMode.Open))
            {
                var result = client.PutObject(new PutObjectRequest("shoy-bucket", "icb.png", ms));
                Print(result);
            }
        }
    }
}
