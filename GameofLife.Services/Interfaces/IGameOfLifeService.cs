using GameOfLife.Data.Models;
using MongoDB.Bson;

namespace GameOfLife.Services.Interfaces
{
    public interface IGameOfLifeService
    {
        Task<ObjectId> CreateNewBoard(int rows, int cols);
        Task<Board> GetNextState(string boardId);
        Task<Board> AdvanceStates(string boardId, int steps);
        Task<Board> GetFinalState(string boardId, int maxAttempts);
    }
}
