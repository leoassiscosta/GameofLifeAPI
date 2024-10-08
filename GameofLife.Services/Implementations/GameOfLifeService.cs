
using GameofLife.Data.Interfaces;
using GameOfLife.Data.Models;
using GameOfLife.Services.Interfaces;
using MongoDB.Bson;

namespace GameofLife.Services.Implementations
{
    public class GameOfLifeService : IGameOfLifeService
    {
        private readonly IBoardRepository _boardRepository;

        public GameOfLifeService(IBoardRepository boardRepository)
        {
            _boardRepository = boardRepository;
        }

        public async Task<ObjectId> CreateNewBoard(int rows, int cols)
        {
            if (rows <= 0 || cols <= 0)
            {
                throw new ArgumentException("Rows and columns must be greater than 0.");
            }

            var board = new Board
            {
                Rows = rows,
                Cols = cols,
                State = InitializeEmptyBoard(rows, cols),
                CreatedAt = DateTime.UtcNow,
                LastUpdatedAt = DateTime.UtcNow
            };

            await _boardRepository.CreateBoard(board);

            // Return borad Id
            return board.Id;
        }

        public async Task<Board> GetNextState(string boardId)
        {
            var board = await _boardRepository.GetBoardById(boardId);
            if (board == null)
            {
                throw new ArgumentException($"Board with ID {boardId} not found.");
            }

            var nextState = ApplyGameOfLifeRules(board.State);
            await _boardRepository.UpdateBoardState(boardId, nextState);

            board.State = nextState;
            board.LastUpdatedAt = DateTime.UtcNow;
            return board;
        }

        public async Task<Board> AdvanceStates(string boardId, int steps)
        {
            var board = await _boardRepository.GetBoardById(boardId);
            if (board == null)
            {
                throw new ArgumentException($"Board with ID {boardId} not found.");
            }

            var currentState = board.State;
            for (int i = 0; i < steps; i++)
            {
                currentState = ApplyGameOfLifeRules(currentState);
            }

            await _boardRepository.UpdateBoardState(boardId, currentState);
            board.State = currentState;
            board.LastUpdatedAt = DateTime.UtcNow;
            return board;
        }

        public async Task<Board> GetFinalState(string boardId, int maxAttempts)
        {
            // Fetch the board by ID
            var board = await _boardRepository.GetBoardById(boardId);
            if (board == null)
            {
                throw new ArgumentException($"Board with ID {boardId} not found.");
            }

            if (maxAttempts <= 0)
            {
                throw new ArgumentException($"The number of maximum attempts must be greater than zero.");
            }

            var currentState = board.State;
            List<List<int>> nextState = null;

            // Iterate up to the max number of attempts to find a stable state
            for (int attempt = 0; attempt < maxAttempts; attempt++)
            {
                nextState = ApplyGameOfLifeRules(currentState);

                // If the state is stable, break out of the loop
                if (IsStableState(currentState, nextState))
                {
                    currentState = nextState;
                    break;
                }

                // Update the current state for the next iteration
                currentState = nextState;
            }

            // If the state did not stabilize after maxAttempts, throw an error
            if (!IsStableState(board.State, currentState))
            {
                throw new InvalidOperationException($"The board did not stabilize after {maxAttempts} attempts.");
            }

            // Update the board state and timestamp
            board.State = currentState;
            board.LastUpdatedAt = DateTime.UtcNow;

            // Save the updated board state in the repository
            await _boardRepository.UpdateBoardState(boardId, board.State);

            return board;
        }

        private static List<List<int>> InitializeEmptyBoard(int rows, int cols)
        {
            // Create a random or empty starting board
            var initialState = new List<List<int>>();
            Random rand = new Random();
            for (int i = 0; i < rows; i++)
            {
                var row = new List<int>();
                for (int j = 0; j < cols; j++)
                {
                    row.Add(rand.Next(2)); // Ou random 0/1
                }
                initialState.Add(row);
            }
            return initialState;
        }

        private static List<List<int>> ApplyGameOfLifeRules(List<List<int>> state)
        {
            int rows = state.Count;
            int cols = state[0].Count;

            // Create a new state for the next generation
            List<List<int>> newState = new List<List<int>>();

            // Initialize the new state with the same list structure
            for (int i = 0; i < rows; i++)
            {
                newState.Add(new List<int>(new int[cols]));
            }

            // Apply the rules of the Game of Life
            for (int x = 0; x < rows; x++)
            {
                for (int y = 0; y < cols; y++)
                {
                    int aliveNeighbors = CountAliveNeighbors(x, y, state, rows, cols);
                    int currentState = state[x][y];

                    // Rule 1: Living cell with less than two living neighbors dies (subpopulation)
                    // Rule 3: Living cell with more than three living neighbors dies (overpopulation)
                    if (currentState == 1 && (aliveNeighbors < 2 || aliveNeighbors > 3))
                    {
                        newState[x][y] = 0;  // Célula morre
                    }
                    // Rule 2: A living cell with two or three living neighbors remains alive
                    else if (currentState == 1 && (aliveNeighbors == 2 || aliveNeighbors == 3))
                    {
                        newState[x][y] = 1;  // Célula continua viva
                    }
                    // Rule 4: Dead cell with exactly three living neighbors becomes alive (reproduction)
                    else if (currentState == 0 && aliveNeighbors == 3)
                    {
                        newState[x][y] = 1;  // Cell is born
                    }
                    else
                    {
                        newState[x][y] = currentState;  // Maintains current state
                    }
                }
            }

            return newState;
        }

        private static int CountAliveNeighbors(int x, int y, List<List<int>> state, int rows, int cols)
        {
            int aliveNeighbors = 0;
            for (int i = -1; i <= 1; i++)
            {
                for (int j = -1; j <= 1; j++)
                {
                    if (i == 0 && j == 0)
                        continue;  // Don't count the current cell

                    int neighborX = x + i;
                    int neighborY = y + j;

                    // Checks if the neighbor is within limits
                    if (neighborX >= 0 && neighborX < rows && neighborY >= 0 && neighborY < cols)
                    {
                        aliveNeighbors += state[neighborX][neighborY];
                    }
                }
            }
            return aliveNeighbors;
        }

        private static bool IsStableState(List<List<int>> currentState, List<List<int>> nextState)
        {
            // Check if the dimensions of the states are different
            if (currentState.Count != nextState.Count)
                return false;

            for (int row = 0; row < currentState.Count; row++)
            {
                // Ensure the number of columns in each row matches
                if (currentState[row].Count != nextState[row].Count)
                    return false;

                // Compare each cell in the row
                for (int col = 0; col < currentState[row].Count; col++)
                {
                    if (currentState[row][col] != nextState[row][col])
                    {
                        // If any cell is different, the state is not stable
                        return false;
                    }
                }
            }

            // If all cells are the same, the state is stable
            return true;
        }
    }
}
