using MongoDB.Bson;

namespace Acb.FileServer.Domain
{
    public class FileDocument
    {
        public string Id { get; set; }
        public BsonValue File { get; set; }
    }
}