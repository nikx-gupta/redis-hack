using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Hack.Service.MongoDb.Models {
    public class Theater {
        [BsonRepresentation(BsonType.ObjectId)]
        public string _id { get; set; }

        public int theaterId { get; set; }
        public location location { get; set; }
    }
}