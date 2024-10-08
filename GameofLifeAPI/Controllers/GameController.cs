using GameOfLife.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace GameofLife.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class GameOfLifeController : ControllerBase
    {
        private readonly IGameOfLifeService _gameOfLifeService;

        public GameOfLifeController(IGameOfLifeService gameOfLifeService)
        {
            _gameOfLifeService = gameOfLifeService;
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreateBoard(int rows, int cols)
        {
            try
            {
                var boardId = await _gameOfLifeService.CreateNewBoard(rows, cols);
                return Ok(new { BoardId = boardId.ToString() });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { Error = ex.Message });
            }
            catch (Exception)
            {
                return StatusCode(500, new { Error = "An unexpected error occurred." });
            }
        }

        [HttpGet("{id}/next")]
        public async Task<IActionResult> GetNextState(string id)
        {
            try
            {
                var board = await _gameOfLifeService.GetNextState(id);
                if (board == null) return NotFound();
                return Ok(board);
            }
            catch (ArgumentException ex)
            {
                return NotFound(new { Message = ex.Message });
            }
            catch (Exception)
            {
                return StatusCode(500, new { Error = "An unexpected error occurred." });
            }
        }

        [HttpGet("{id}/advance/{steps}")]
        public async Task<IActionResult> AdvanceStates(string id, int steps)
        {
            try
            {
                var board = await _gameOfLifeService.AdvanceStates(id, steps);
                if (board == null) return NotFound();
                return Ok(board);
            }
            catch (ArgumentException ex)
            {
                return NotFound(new { Message = ex.Message });
            }
            catch (Exception)
            {
                return StatusCode(500, new { Error = "An unexpected error occurred." });
            }
        }

        [HttpGet("{id}/final/{maxAttempts}")]
        public async Task<IActionResult> GetFinalState(string id, int maxAttempts)
        {
            try
            {
                var board = await _gameOfLifeService.GetFinalState(id, maxAttempts);
                if (board == null) return NotFound();
                return Ok(board);
            }
            catch (ArgumentException ex)
            {
                return NotFound(new { Message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "An unexpected error occurred.", Details = ex.Message });
            }
        }
    }

}


