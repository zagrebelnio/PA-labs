using Xunit;
using InformariveSearch;
using NonInformativeSearch;
using ExperimentData;

namespace NQueen.Tests
{
    public class AStarTests
    {
        [Fact]
        public void AStar_Search_ReturnsValidResult()
        {
            // Arrange
            int[] board = new int[] { 0, 0, 0, 0 };

            // Act
            Results result = AStar.Search(board);

            // Assert
            Assert.NotNull(result);
        }

        [Fact]
        public void AStar_Search_ReturnsNull()
        {
            // Arrange
            int[] board = new int[] { 0, 0, 0 };

            // Act
            Results result = AStar.Search(board);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public void AStar_CalculateHeuristic_ReturnsZero()
        {
            // Arrange
            int[] board = new int[] { 1, 3, 0, 2 };
            AStar.Node node = new AStar.Node(board);

            // Act
            int result = node.CalculateHeuristic();

            // Assert
            Assert.Equal(0, result);
        }

        [Fact]
        public void AStar_CalculateHeuristic_ReturnsFive()
        {
            // Arrange
            int[] board = new int[] { 0, 1, 0, 1 };
            AStar.Node node = new AStar.Node(board);

            // Act
            int result = node.CalculateHeuristic();

            // Assert
            Assert.Equal(5, result);
        }
    }

    public class BFSTests
    {
        [Fact]
        public void BFS_Search_ReturnsValidResult()
        {
            // Arrange
            int[] board = new int[] { 0, 0, 0, 0 };

            // Act
            Results result = BFS.Search(board);

            // Assert
            Assert.NotNull(result);
        }

        [Fact]
        public void BFS_Search_ReturnsNull()
        {
            // Arrange
            int[] board = new int[] { 0, 0, 0 };

            // Act
            Results result = BFS.Search(board);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public void BFS_IsGoalState_ReturnsTrue()
        {
            // Arrange
            int[] board = new int[] { 1, 3, 0, 2 };
            BFS.Node node = new BFS.Node(board);

            // Act
            bool result = node.IsGoalState();

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void BFS_IsGoalState_ReturnsFalse()
        {
            // Arrange
            int[] board = new int[] { 0, 1, 0, 1 };
            BFS.Node node = new BFS.Node(board);

            // Act
            bool result = node.IsGoalState();

            // Assert
            Assert.False(result);
        }
    }
}