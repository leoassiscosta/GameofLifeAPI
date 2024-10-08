using MongoDB.Driver;
using MongoDB.Bson;
using GameOfLife.Data.Models;
using GameofLife.Data.Interfaces;

namespace GameOfLife.Data.Repositories
{
    public class BoardRepository : IBoardRepository
    {
        private readonly IMongoCollection<Board> _boards;

        public BoardRepository(IMongoDatabase database)
        {
            _boards = database.GetCollection<Board>("Boards");
        }

        public async Task<Board> CreateBoard(Board board)
        {
            await _boards.InsertOneAsync(board);
            return board;
        }

        public async Task<Board> GetBoardById(string id)
        {
            return await _boards.Find(x => x.Id == ObjectId.Parse(id)).FirstOrDefaultAsync();
        }

        public async Task UpdateBoardState(string id, List<List<int>> newState)
        {
            var update = Builders<Board>.Update.Set(b => b.State, newState);
            await _boards.UpdateOneAsync(b => b.Id == ObjectId.Parse(id), update);
        }
    }
}
