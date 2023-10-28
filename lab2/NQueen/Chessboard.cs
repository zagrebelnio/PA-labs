using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ExperimentData;

namespace Board
{
    public class Chessboard
    {
        private const string BoardFilePath = "board.txt";

        public static int[] GenerateBoard(int N)
        {
            int[] board = new int[N];
            for (int i = 0; i < N; i++)
            {
                board[i] = new Random().Next(0, N - 1);
            }
            return board;
        }

        public static int[] ReadBoard()
        {
            int[] numbers = null;
            try
            {
                string text = File.ReadAllText(BoardFilePath);
                string[] numberStrings = text.Split(',');
                numbers = new int[numberStrings.Length];

                for (int i = 0; i < numberStrings.Length; i++)
                {
                    if (int.TryParse(numberStrings[i], out int number))
                        numbers[i] = number;
                    else
                        throw new Exception($"The {BoardFilePath} file has invalid input. It contains non integer values.");
                }

                for (int i = 0; i < numbers.Length; i++)
                {
                    if (numbers[i] < 0 || numbers[i] > numbers.Length - 1)
                    {
                        throw new Exception($"The {BoardFilePath} file has invalid input. The values in the file are out of range of the board.");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                Environment.Exit(1);
            }

            return numbers;
        }

        public static void PrintResult(int[] result)
        {
            int n = result.Length;
            char[,] board = new char[n, n];

            // Initialize the board with '-'
            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < n; j++)
                {
                    board[i, j] = '-';
                }
            }

            // Place queens on the board
            for (int i = 0; i < n; i++)
            {
                int j = result[i];
                board[j, i] = 'Q';
            }

            // Print the board
            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < n; j++)
                {
                    Console.Write(board[i, j] + " ");
                }
                Console.WriteLine();
            }

            Console.WriteLine(new string('=', n * 2 - 1) + "\n");
        }

        public static void PrintResult(Results result)
        {
            int n = result.Board.Length;
            char[,] board = new char[n, n];

            // Initialize the board with '-'
            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < n; j++)
                {
                    board[i, j] = '-';
                }
            }

            // Place queens on the board
            for (int i = 0; i < n; i++)
            {
                int j = result.Board[i];
                board[j, i] = 'Q';
            }

            // Print the board
            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < n; j++)
                {
                    Console.Write(board[i, j] + " ");
                }
                Console.WriteLine();
            }

            Console.WriteLine(new string('=', n * 2 - 1) + "\n");

            Console.WriteLine($"Number of Steps (Iterations): {result.Steps}");
            Console.WriteLine($"Number of States generated: {result.States}");
            Console.WriteLine($"Number of States allocated in memory: {result.StatesInMemory}");
        }
    }
}
