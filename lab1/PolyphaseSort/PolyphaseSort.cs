using CommandSettings;

namespace ExternalSort
{
    public class PolyphaseSort
    {
        public int tapesCount;

        public int lastTapeIndex;

        public const string TEMP_TAPE = "tape";

        public void Sort(Settings settings)
        {
            string inputFile = settings.InputPath;
            string outputFile = settings.OutputPath;
            tapesCount = settings.tapesCount;
            lastTapeIndex = tapesCount - 1;

            Tape[] tapes = new Tape[tapesCount];
            for (int i = 0; i < tapesCount; i++)
            {
                tapes[i] = new Tape();
            }
            DistributeSeries(inputFile, tapes);
            MergeSeries(outputFile, tapes);
        }

        public void DistributeSeries(string inputFile, Tape[] tapes)
        {
            int fileLength = File.ReadAllLines(inputFile).Length;

            StreamReader sr = new StreamReader(inputFile);

            for (int i = 0; i < tapesCount; i++)
            {
                tapes[i].Writer = File.CreateText($"{TEMP_TAPE}{i}");
            }

            int chunkLength = Convert.ToInt32(Math.Ceiling(fileLength / Convert.ToDouble(tapesCount - 1)));

            for (int i = 0; i < tapesCount - 1; i++)
            {
                for (int j = 0; j < chunkLength && !sr.EndOfStream; j++)
                {
                    tapes[i].Writer.WriteLine(sr.ReadLine());
                }
            }

            for (int i = 0; i < tapesCount; i++)
            {
                tapes[i].Writer.Close();
            }

            sr.Close();
        }

        public void MergeSeries(string outputFile, Tape[] tapes)
        {
            CountSeries(tapes);
            AddEmptySeries(tapes);

            while (SumOfSeries(tapes) != 1)
            {
                int minSeries = GetMinSeries(tapes);

                int[] numbers = new int[tapesCount - 1];

                for (int i = 0, index = 0; i < tapesCount; i++)
                {

                    if (i == lastTapeIndex) continue;
                    tapes[index].Reader = new StreamReader($"{TEMP_TAPE}{i}");
                    if (tapes[i].RealSeries > 0)
                    {
                        string line = tapes[index].Reader.ReadLine();
                        numbers[index] = line != null ? int.Parse(line) : int.MaxValue;
                    }
                    else
                    {
                        numbers[index] = int.MaxValue;
                    }
                    index++;
                }

                tapes[lastTapeIndex].Writer = new StreamWriter($"{TEMP_TAPE}{lastTapeIndex}");

                int[] last = new int[tapesCount - 1];
                for (int i = 0; i < minSeries; i++)
                {
                    while (!MergeFinished(numbers))
                    {
                        int minIndex = FindMinIndex(numbers);
                        tapes[lastTapeIndex].Writer.WriteLine(numbers[minIndex]);

                        string line = tapes[minIndex].Reader.ReadLine();
                        int next = line != null ? int.Parse(line) : int.MinValue;
                        if (next < numbers[minIndex])
                        {
                            numbers[minIndex] = int.MaxValue;
                            last[minIndex] = line != null ? next : int.MaxValue;
                        }
                        else
                        {
                            numbers[minIndex] = next;
                        }
                    }
                    for (int j = 0; j < tapesCount - 1; j++)
                    {
                        numbers[j] = last[j] == 0 ? int.MaxValue : last[j];
                    }
                }

                tapes[lastTapeIndex].Writer.Close();

                for (int i = 0, index = 0; i < tapesCount; i++)
                {
                    if (i == lastTapeIndex) continue;
                    StreamWriter temp = new StreamWriter($"temp{i}");
                    if (numbers[index] == int.MaxValue)
                    {
                        temp.Close();
                        index++;
                        continue;
                    }
                    temp.WriteLine(numbers[index]);
                    while (!tapes[index].Reader.EndOfStream)
                    {
                        temp.WriteLine(tapes[index].Reader.ReadLine());
                    }
                    temp.Close();
                    index++;
                }

                for (int i = 0; i < tapesCount - 1; i++)
                {
                    tapes[i].Reader.Close();
                }

                for (int i = 0; i < tapesCount; i++)
                {
                    if (i == lastTapeIndex) continue;
                    File.Delete($"{TEMP_TAPE}{i}");
                    File.Move($"temp{i}", $"{TEMP_TAPE}{i}");
                }

                bool emptySeriesMerge = true;
                for (int i = 0; i < tapesCount; i++)
                {
                    if (tapes[i].TotalSeries == 0) continue;
                    if (tapes[i].RealSeries >= minSeries)
                    {
                        emptySeriesMerge = false;
                        break;
                    }
                }

                for (int i = 0; i < tapesCount; i++)
                {
                    if (tapes[i].TotalSeries == 0)
                    {
                        tapes[i].TotalSeries += minSeries;
                        if (!emptySeriesMerge)
                        {
                            tapes[i].RealSeries += minSeries;
                        }
                        else
                        {
                            tapes[i].EmptySeries += EmptyMergeCount(tapes, minSeries);
                            tapes[i].RealSeries = tapes[i].TotalSeries - tapes[i].EmptySeries;
                        }
                        break;
                    }
                }

                for (int i = 0; i < tapesCount; i++)
                {
                    if (i != lastTapeIndex)
                    {
                        tapes[i].TotalSeries -= minSeries;
                        if (tapes[i].RealSeries < minSeries)
                        {
                            tapes[i].EmptySeries -= minSeries - tapes[i].RealSeries;
                            tapes[i].RealSeries = 0;
                        }
                        else
                        {
                            tapes[i].RealSeries -= minSeries;
                        }
                    }
                }

                for (int i = 0; i < tapesCount; i++)
                {
                    if (tapes[i].TotalSeries == 0)
                    {
                        lastTapeIndex = i;
                        break;
                    }
                }
            }

            int outputIndex = -1;
            for (int i = 0; i < tapesCount; i++)
            {
                if (tapes[i].TotalSeries == 1)
                {
                    outputIndex = i;
                    break;
                }
            }

            if (File.Exists(outputFile))
                File.Delete(outputFile);
            File.Move($"{TEMP_TAPE}{outputIndex}", outputFile);
            for (int i = 0; i < tapesCount; i++)
            {
                File.Delete($"{TEMP_TAPE}{i}");
            }
        }

        public void CountSeries(Tape[] tapes)
        {
            for (int i = 0; i < tapesCount; i++)
            {
                using (tapes[i].Reader = new StreamReader($"{TEMP_TAPE}{i}"))
                {
                    string line;
                    int subsequenceCount = 0;
                    int previousNum = int.MinValue;  // Initialize with smallest possible value

                    while ((line = tapes[i].Reader.ReadLine()) != null)
                    {
                        if (int.TryParse(line, out int currentNum))
                        {
                            // Check if the current number is in non-decreasing order
                            if (currentNum >= previousNum)
                            {
                                previousNum = currentNum;
                            }
                            else
                            {
                                // Start of a new sorted subsequence
                                subsequenceCount++;
                                previousNum = currentNum;
                            }
                        }
                    }

                    // Increment subsequence count if the last subsequence is not empty
                    if (previousNum != int.MinValue)
                    {
                        subsequenceCount++;
                    }

                    tapes[i].RealSeries = subsequenceCount;
                    tapes[i].TotalSeries = tapes[i].RealSeries;
                }
            }
        }

        public void AddEmptySeries(Tape[] tapes)
        {
            int maxSeries = GetMaxSeries(tapes);
            int[] fibonacciSequence = new int[tapesCount - 1];
            for (int i = 0; i < tapesCount - 2; i++)
            {
                fibonacciSequence[i] = 0;
            }
            fibonacciSequence[tapesCount - 2] = 1;
            int[] totalSeries = new int[tapesCount - 1];
            do
            {
                for (int i = 0, index = 0; i < tapesCount; i++)
                {
                    if (tapes[i].TotalSeries == 0) continue;
                    totalSeries[index] = tapes[i].TotalSeries;
                    index++;
                }
                if (maxSeries == fibonacciSequence[tapesCount - 2])
                {
                    if (new HashSet<int>(totalSeries).SetEquals(fibonacciSequence))
                    {
                        return;
                    }
                }
                UpdateFibonacci(fibonacciSequence);
            } while (!FibonacciEnded(totalSeries, fibonacciSequence));

            int[] realSeries = new int[tapesCount];
            for (int i = 0; i < tapesCount; i++)
            {
                realSeries[i] = tapes[i].RealSeries;
            }

            for (int i = tapesCount - 2; i >= 0; i--)
            {
                int maxIndex = -1;
                int max = 0;
                for (int j = 0; j < tapesCount; j++)
                {
                    if (max < realSeries[j])
                    {
                        maxIndex = j;
                        max = realSeries[j];
                    }
                }
                realSeries[maxIndex] = 0;
                tapes[maxIndex].EmptySeries = fibonacciSequence[i] - tapes[maxIndex].RealSeries;
                tapes[maxIndex].TotalSeries = tapes[maxIndex].RealSeries + tapes[maxIndex].EmptySeries;
            }
        }

        public bool FibonacciEnded(int[] totalSeries, int[] fibonacci)
        {
            int[] temp = new int[totalSeries.Length];
            Array.Copy(totalSeries, temp, totalSeries.Length);
            Array.Sort(temp);
            for (int i = 0; i < totalSeries.Length; i++)
            {
                if (temp[i] > fibonacci[i])
                {
                    return false;
                }
            }
            return true;
        }

        public void UpdateFibonacci(int[] fibonacci)
        {
            int[] nextFibonacci = new int[fibonacci.Length];

            nextFibonacci[0] = fibonacci[^1];

            for (int i = 1; i < fibonacci.Length; i++)
            {
                nextFibonacci[i] = fibonacci[^1] + fibonacci[i - 1];
            }

            for (int i = 0; i < fibonacci.Length; i++)
            {
                fibonacci[i] = nextFibonacci[i];
            }
        }

        public int GetMinSeries(Tape[] tapes)
        {
            int minSeries = int.MaxValue;

            for (int i = 0; i < tapesCount; i++)
            {
                if (tapes[i].TotalSeries == 0)
                {
                    continue;
                }
                if (tapes[i].TotalSeries < minSeries)
                {
                    minSeries = tapes[i].TotalSeries;
                }
            }

            return minSeries;
        }

        public int GetMaxSeries(Tape[] tapes)
        {
            int maxSeries = int.MinValue;

            for (int i = 0; i < tapesCount; i++)
            {
                if (tapes[i].TotalSeries > maxSeries)
                {
                    maxSeries = tapes[i].TotalSeries;
                }
            }

            return maxSeries;
        }

        public int FindMinIndex(int[] numbers)
        {
            int minIndex = -1;
            int minValue = int.MaxValue;
            for (int i = 0; i < numbers.Length; i++)
            {
                if (numbers[i] < minValue)
                {
                    minValue = numbers[i];
                    minIndex = i;
                }
            }
            return minIndex;
        }

        public bool MergeFinished(int[] numbers)
        {
            foreach (int num in numbers)
            {
                if (num != int.MaxValue)
                {
                    return false;
                }
            }
            return true;
        }

        public int SumOfSeries(Tape[] tapes)
        {
            int sum = 0;
            for (int i = 0; i < tapesCount; i++)
            {
                sum += tapes[i].TotalSeries;
            }

            return sum;
        }

        public int EmptyMergeCount(Tape[] tapes, int minSeries)
        {
            int maxRealSeries = 0;
            for (int i = 0; i < tapes.Length; i++)
            {
                if (tapes[i].RealSeries > maxRealSeries)
                {
                    maxRealSeries = tapes[i].RealSeries;
                }
            }
            return minSeries - maxRealSeries;
        }
    }
    public class Tape
    {
        public int TotalSeries = 0;
        public int EmptySeries = 0;
        public int RealSeries = 0;
        public StreamReader Reader = null!;
        public StreamWriter Writer = null!;

        public Tape() { }
    }
}