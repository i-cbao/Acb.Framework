using Acb.Core;
using Acb.Core.Modules;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using System;

namespace Acb.MongoDb
{
    [DependsOn(typeof(CoreModule))]
    public class MongoModule : DModule
    {
        public override void Initialize()
        {
            // 本地时间
            var serializer = new DateTimeSerializer(DateTimeKind.Local, BsonType.DateTime);
            BsonSerializer.RegisterSerializer(typeof(DateTime), serializer);
            // Guid数据存储
            BsonDefaults.GuidRepresentation = GuidRepresentation.Standard;
            base.Initialize();
        }
    }
}
