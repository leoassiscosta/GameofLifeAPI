using GameOfLife.Data.Models;

namespace GameofLife.Data.Interfaces
{
    public interface IBoardRepository
    {
        Task<Board> CreateBoard(Board board);
        Task<Board> GetBoardById(string id);
        Task UpdateBoardState(string id, List<List<int>> newState);
    }
}
