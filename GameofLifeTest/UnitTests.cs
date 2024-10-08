using System;
using Xunit;
using Moq;
using GameofLife.API.Controllers;
using GameOfLife.Data;
using GameOfLife.Services;
using Microsoft.AspNetCore.Mvc;
using GameOfLife.Services.Interfaces;
using MongoDB.Bson;
using GameOfLife.Data.Models;

namespace GameofLifeTest
{
    public class GameOfLifeControllerTests
    {
        [Fact]
        public void CreateNewBoard_ReturnsNotNull()
        {
            // Arrange
            var mockService = new Mock<IGameOfLifeService>();
            var controller = new GameOfLifeController(mockService.Object);
            var rows = 5;
            var cols = 5;
            var mockBoardId = ObjectId.GenerateNewId();

            mockService.Setup(s => s.CreateNewBoard(rows, cols))
                .ReturnsAsync(mockBoardId);

            // Act
            var result = controller.CreateBoard(rows, cols).Result;

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            
            Assert.NotNull(okResult.Value);
        }

        [Fact]
        public void GetNextState_ReturnsNextState()
        {
            // Arrange
            var mockService = new Mock<IGameOfLifeService>();
            var controller = new GameOfLifeController(mockService.Object);
            var boardId = "651f2bf7b7f9f39d2edfc874";
            var nextState = new List<List<int>> {
                new List<int> { 0, 1, 0 },
                new List<int> { 1, 1, 1 },
                new List<int> { 0, 1, 0 }
            };

            Board board = new Board()
            {
                Cols = 3,
                Rows = 3,
                State = nextState
            };

            mockService.Setup(s => s.GetNextState(boardId))
                       .ReturnsAsync(board);

            // Act
            var result = controller.GetNextState(boardId).Result;

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnValue = Assert.IsType<Board>(okResult.Value);
            Assert.Equal(nextState, returnValue.State);
        }

        [Fact]
        public void GetFinalState_ReturnsFinalState()
        {
            // Arrange
            var mockService = new Mock<IGameOfLifeService>();
            var controller = new GameOfLifeController(mockService.Object);
            var boardId = "651f2bf7b7f9f39d2edfc874";
            var finalState = new List<List<int>> {
                new List<int> { 0, 1, 0 },
                new List<int> { 1, 1, 1 },
                new List<int> { 0, 1, 0 }
            };

            Board board = new Board()
            {
                Cols = 3,
                Rows = 3,
                State = finalState
            };

            mockService.Setup(s => s.GetFinalState(boardId, 10)) // Max attempts = 10
                       .ReturnsAsync(board);

            // Act
            var result = controller.GetFinalState(boardId, 10).Result;

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnValue = Assert.IsType<Board>(okResult.Value);
            Assert.Equal(finalState, returnValue.State);
        }

        [Fact]
        public async Task GetFinalState_BoardDoesNotStabilize_ReturnsBadRequest()
        {
            // Arrange
            var mockService = new Mock<IGameOfLifeService>();
            var controller = new GameOfLifeController(mockService.Object);
            var boardId = "651f2bf7b7f9f39d2edfc874";
            var maxAttempts = 10;

            // Mock the service to throw an InvalidOperationException when max attempts are exceeded
            mockService.Setup(s => s.GetFinalState(boardId, maxAttempts))
                       .ThrowsAsync(new InvalidOperationException("The board did not stabilize after 10 attempts."));

            // Act
            var result = controller.GetFinalState(boardId, maxAttempts).Result;

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Contains("The board did not stabilize after 10 attempts.", badRequestResult.Value.ToString());
        }
    }
}

