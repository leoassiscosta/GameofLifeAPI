using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace GameOfLife.Data.Models
{
    public class Board
    {
        [BsonId]
        public ObjectId Id { get; set; } = ObjectId.GenerateNewId();

        [BsonElement("rows")]
        public int Rows { get; set; }

        [BsonElement("cols")]
        public int Cols { get; set; }

        [BsonElement("state")]
        public List<List<int>> State { get; set; }  // Nested list to represent the 2D board

        [BsonElement("createdAt")]
        public DateTime CreatedAt { get; set; }

        [BsonElement("lastUpdatedAt")]
        public DateTime LastUpdatedAt { get; set; }
    }
}