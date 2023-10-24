using System;
using System.Diagnostics;
using ExternalSort;
using CommandLine;
using System.Runtime.InteropServices;
using CommandSettings;

namespace Program
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                Parser.Default.ParseArguments<Settings>(args).WithParsed<Settings>(settings =>
                {
                    ParseSettings(settings);
                    Console.WriteLine($"Input Path: {settings.InputPath}");
                    Console.WriteLine($"Output Path: {settings.OutputPath}");

                    // if needed to generate a new file
                    if (settings.FileSizeMB > 0)
                    {
                        GenerateInputFile(settings.InputPath, settings.FileSizeMB);
                        Console.WriteLine("Input file generated");
                    }

                    ExternalSort(settings);
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }

        static void ParseSettings(Settings settings)
        {
            // input file path
            if (!File.Exists(settings.InputPath) && settings.FileSizeMB == 0)
            {
                throw new Exception("The input file doesn't exist");
            }

            if (settings.FileSizeMB == 0 && new FileInfo(settings.InputPath).Length == 0 )
            {
                throw new Exception("The input file is empty");
            }

            if (settings.FileSizeMB == 0)
            {
                using (StreamReader sr = new StreamReader(settings.InputPath))
                {
                    while (!sr.EndOfStream)
                    {
                        string line = sr.ReadLine();

                        if (!int.TryParse(line, out int number))
                        {
                            throw new Exception("Invalid input. Not all lines of input file contain valid integers.");
                        }

                        // Process the integer 'number' if needed
                    }
                }
            }
            // output file path

            // tapes count

            if (settings.tapesCount < 3 || settings.tapesCount > 10)
            {
                throw new Exception("Tapes Count should be the value in range (3-10)");
            }

            // file size in MB
            if (settings.FileSizeMB < 0 || settings.FileSizeMB > 1024)
            {
                throw new Exception("The file size should be in range (0-1024) MB");
            }
        }


        // Generate input file with given size in MB filled with int numbers in specific range
        static void GenerateInputFile(string filePath, int fileSizeInMB)
        {
            long fileSizeInBytes = fileSizeInMB * 1024L * 1024L;

            using (StreamWriter writer = new StreamWriter(filePath))
            {
                Random random = new Random();

                while (writer.BaseStream.Length < fileSizeInBytes)
                {
                    int randomNumber = random.Next(0, 1000000001);
                    string numberString = randomNumber.ToString();
                    writer.WriteLine(numberString);
                }
            }
        }

        // calculate file size in MB
        static double FileSizeMB(string inputFile)
        {
            FileInfo fileInfo = new FileInfo(inputFile);

            return fileInfo.Length / (1024.0 * 1024.0);
        }

        static void PreSort(string inputFile, int maxMB)
        {
            const string tempFile = "temp_preSort";
            long bytes = 1024L * 1024L * maxMB;  // Use long for accurate calculations
            int[] array = new int[bytes / sizeof(int)];
            StreamReader reader = new StreamReader(inputFile);
            StreamWriter writer = new StreamWriter(tempFile);
            while (!reader.EndOfStream)
            {
                int numElements = 0;
                // Read and fill the array with data up to the specified limit
                while (numElements * sizeof(int) < bytes && !reader.EndOfStream)
                {
                    int num;
                    if (int.TryParse(reader.ReadLine(), out num))
                    {
                        array[numElements] = num;
                        numElements++;
                    }
                }

                // Sort the data in the array
                Array.Sort(array, 0, numElements);

                // Write the sorted data to the temp file

                for (int i = 0; i < numElements; i++)
                {
                    writer.WriteLine(array[i]);
                }
            }
            reader.Close();
            writer.Close();
            // Move the temp file to the input file
            File.Delete(inputFile);
            File.Move(tempFile, inputFile);
        }

        // Sort the file
        static void ExternalSort(Settings settings)
        {

            // File existance validation
            if (!File.Exists(settings.InputPath))
            {
                throw new Exception("There is no file with given path: " + settings.InputPath);
            }

            // Start the timer
            Stopwatch sw = Stopwatch.StartNew();

            // PreSort
            if (settings.preSort)
            {
                PreSort(settings.InputPath, 64);
            }

            // Polyphase sort
            PolyphaseSort polyphaseSort = new PolyphaseSort();
            polyphaseSort.Sort(settings);
            sw.Stop();

            // Results output
            Console.WriteLine($"Execution time: {sw.Elapsed.TotalSeconds:F3} seconds");
            Console.WriteLine($"File Size: {FileSizeMB(settings.InputPath):F2} MB");
        }
    }
}
