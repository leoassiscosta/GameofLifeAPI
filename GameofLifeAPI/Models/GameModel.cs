using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace GameofLifeAPI.Models
{
    public class GameModel
    {
        [BsonId]
        public ObjectId Id { get; set; } = ObjectId.GenerateNewId();  // Automatically generates a new ObjectId

        [BsonElement("rows")]
        public int Rows { get; set; }

        [BsonElement("cols")]
        public int Cols { get; set; }

        [BsonElement("boardState")]
        public List<List<int>> Board { get; set; }

        [BsonElement("createdAt")]
        public DateTime CreatedAt { get; set; }

        [BsonElement("lastUpdatedAt")]
        public DateTime LastUpdatedAt { get; set; }
    }
}
