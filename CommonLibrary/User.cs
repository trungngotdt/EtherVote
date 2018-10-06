using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace CommonLibraryUtilities
{
    public class User
    {
        [BsonId]
        public ObjectId Id { get; set; }

        [BsonElement("address")]
        public string Address { get; set; }

        [BsonElement("role")]
        public string Role { get; set; }

        [BsonElement("available")]
        public bool Available { get; set; }
    }
}
