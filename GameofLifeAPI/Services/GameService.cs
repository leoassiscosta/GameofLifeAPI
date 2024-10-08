//using MongoDB.Driver;
//using GameofLifeAPI.Models;
//using System.Collections.Generic;
//using System.Threading.Tasks;
//using MongoDB.Bson;

//namespace GameofLifeAPI.Services
//{
//    public class GameService
//    {
//        private readonly IMongoCollection<GameModel> _collection;

//        public GameService(IMongoClient client)
//        {
//            var database = client.GetDatabase("GameofLife");
//            _collection = database.GetCollection<GameModel>("Board");
//        }

//        public async Task<List<GameModel>> GetAsync()
//        {
//            return await _collection.Find(_ => true).ToListAsync();
//        }

//        public async Task<GameModel> GetByIdAsync(ObjectId id) => await _collection.Find(model => model.Id == id).FirstOrDefaultAsync();

//        public Task CreateAsync(GameModel gameModel)
//        {

//            int rows = 20;
//            int cols = 40;

//            int[,] newBoard = new int[rows, cols];
//            Random rand = new Random();
//            for (int i = 0; i < rows; i++)
//            {
//                for (int j = 0; j < cols; j++)
//                {
//                    newBoard[i, j] = rand.Next(2); // 0 or 1 random
//                }
//            }

//            gameModel.CreatedAt = DateTime.Now;
//            gameModel.Board = Convert2DArrayToNestedList(newBoard);
//            gameModel.Cols = cols;
//            gameModel.Rows = rows;

//            return _collection.InsertOneAsync(gameModel);
//        }

//        public static List<List<int>> Convert2DArrayToNestedList(int[,] array)
//        {
//            var result = new List<List<int>>();
//            for (int i = 0; i < array.GetLength(0); i++)
//            {
//                var row = new List<int>();
//                for (int j = 0; j < array.GetLength(1); j++)
//                {
//                    row.Add(array[i, j]);
//                }
//                result.Add(row);
//            }
//            return result;
//        }

//        public async Task UpdateAsync(ObjectId id, GameModel model) => await _collection.ReplaceOneAsync(m => m.Id == id, model);

//        public async Task DeleteAsync(ObjectId id) => await _collection.DeleteOneAsync(m => m.Id == id);
//    }


//}
