using System;
using System.Linq;
using Hack.Service.MongoDb.Models;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.IdGenerators;
using MongoDB.Bson.Serialization.Serializers;
using MongoDB.Driver;

namespace TestConsole {
    class Program {
        static void Main(string[] args) {
            // BsonClassMap.RegisterClassMap<MovieImdb>(map => {
            //     map.AutoMap();
            //     // map.SetIgnoreExtraElements(true);
            //     // map.UnmapProperty(x => x.id);
            // });
            // // BsonClassMap.RegisterClassMap<Movie>(map => {
            // //     map.MapProperty(x => x.imdb).SetSerializer(BsonDocumentSerializer.Instance);
            // // });
            MongoClient client = new("mongodb://192.168.29.69:27017");
            var db = client.GetDatabase("sample_mflix");
            var coll = db.GetCollection<BsonDocument>("users");
            var filter = Builders<BsonDocument>.Filter.Empty;
            // var filter = Builders<Movie>.Filter.Eq(x => x.id, "573a1390f29313caabcd4135");
            var docs = coll.Find(filter);
            var list = docs.ToList();

            ClassConvertor convertor = new ClassConvertor();
            // convertor.ConvertJsonToClass("data.json", "Movie");
            convertor.ConvertBsonToClass(list, "User");
            
            Console.WriteLine("Completed");
            Console.Read();
        }
    }
}