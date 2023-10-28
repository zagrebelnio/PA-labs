using System;
using System.Diagnostics;
using System.Xml.XPath;
using InformariveSearch;
using NonInformativeSearch;
using Board;
using CommandLine;
using Settings;
using ExperimentData;

namespace NQueen
{
    public class Program
    {
        public static void ParseOptions(Options options)
        {
            if (options.N < 0 || options.N > 20)
            {
                throw new Exception("The number of Queens should be the value in range (0 - 20).");
            }

            if (!options.BFS && !options.AStar)
            {
                throw new Exception("Choose at least one of the algorithms to solve the problem.");
            }

            if (options.MemoryGB < 1 ||  options.MemoryGB > 10)
            {
                throw new Exception("Memory limit in GB should be in range (1 - 10)");
            }
        }

        public static void Main(string[] args)
        {
            try
            {
                Parser.Default.ParseArguments<Options>(args)
                   .WithParsed<Options>(async o =>
                   {
                       ParseOptions(o);

                       int[] board = null;

                       if (o.N != 0)
                       {
                           board = Chessboard.GenerateBoard(o.N);
                       }
                       else
                       {
                           board = Chessboard.ReadBoard();
                       }

                       Task.Run(async () => await MonitorMemoryUsageAsync((o.MemoryGB * 1024L * 1024L * 1024L)));

                       while (o.AStar || o.BFS)
                       {
                           ExperimentData.Results results = null;

                           Stopwatch sw = new Stopwatch();
                           sw.Start();

                           if (o.AStar)
                           {
                               results = AStar.Search(board);
                               o.AStar = false;
                           }
                           else
                           {
                               results = BFS.Search(board);
                               o.BFS = false;
                           }

                           sw.Stop();

                           if (results != null)
                           {
                               Console.WriteLine("Found solution:");
                               Chessboard.PrintResult(results);
                           }
                           else
                           {
                               Console.WriteLine("No solution found.");
                           }

                           Console.WriteLine($"Execution time: {sw.Elapsed.TotalSeconds:F3} seconds");
                       }
                   });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }

        static async Task MonitorMemoryUsageAsync(long memoryLimitBytes)
        {
            try
            {
                while (true)
                {
                    // Get the current process
                    Process currentProcess = Process.GetCurrentProcess();

                    // Check the working set memory size in bytes
                    long currentMemoryUsage = currentProcess.WorkingSet64;

                    if (currentMemoryUsage > memoryLimitBytes)
                    {
                        // Memory usage exceeded the limit, throw an exception

                        throw new Exception($"Memory usage exceeded {memoryLimitBytes / (1024 * 1024 * 1024):F0} GB");
                    }

                    // Sleep for a while before checking again (adjust as needed)
                    await Task.Delay(TimeSpan.FromSeconds(1));
                }
            }
            catch (Exception ex)
            {
               Console.WriteLine($"Memory Monitoring Error: {ex.Message}");
               Environment.Exit(1);
            }
        }
    }
}