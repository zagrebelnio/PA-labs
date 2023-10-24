using Xunit;
using ExternalSort;
using Microsoft.VisualStudio.TestPlatform.CommunicationUtilities;

namespace ExternalSort.Tests
{
    public static class HelpFunctions
    {
        public static bool FilesAreEqual(string f1, string f2)
        {
            int length1 = File.ReadLines(f1).Count();
            int length2 = File.ReadLines(f2).Count();

            if (length1 != length2)
            {
                return false;
            }

            StreamReader file1 = new StreamReader(f1);
            StreamReader file2 = new StreamReader(f2);

            while (!file1.EndOfStream)
            {
                if (file1.ReadLine() != file2.ReadLine())
                {
                    return false;
                }
            }

            file1.Close();
            file2.Close();

            return true;
        }
    }

    public class PolyphaseSortTests
    {
        [Fact]
        public void DistributeSeries_ValidInput_DistributesCorrectly()
        {
            // Arrange

            string testFilesPath = "../../../test_files/DistributeSeries/ValidInput/";
            int tapesCount = 3;

            // Act

            PolyphaseSort ps = new PolyphaseSort();
            ps.tapesCount = tapesCount;

            Tape[] tapes = new Tape[tapesCount];
            for (int i = 0; i < tapes.Length; i++)
            {
                tapes[i] = new Tape();
            }

            ps.DistributeSeries($"{testFilesPath}input.txt", tapes);

            // Assert
            
            for (int i = 0; i < tapes.Length; i++)
            {
                Assert.True(HelpFunctions.FilesAreEqual($"{PolyphaseSort.TEMP_TAPE}{i}", $"{testFilesPath}tape{i}.txt"));
            }

            for (int i = 0; i < tapes.Length; i++)
            {
                File.Delete($"{PolyphaseSort.TEMP_TAPE}{i}");
            }
        }

        [Fact]
        public void CountSeries_CountsCorrectly()
        {
            // Arrange

            string testFilesPath = "../../../test_files/CountSeries/";
            int tapesCount = 3;

            // Act

            PolyphaseSort ps = new PolyphaseSort();
            ps.tapesCount = tapesCount;

            Tape[] tapes = new Tape[tapesCount];
            for (int i = 0; i < tapes.Length; i++)
            {
                tapes[i] = new Tape();
                File.Copy($"{testFilesPath}tape{i}", $"tape{i}");
            }

            ps.CountSeries(tapes);

            for (int i = 0; i < tapes.Length; i++)
            {
                File.Delete($"tape{i}");
            }

            // Assert

            int[] totalSeries = { 12, 12, 10 };
            
            for (int i = 0; i < totalSeries.Length; i++)
            {
                Assert.Equal(totalSeries[i], tapes[i].TotalSeries);
            }
        }

        [Fact]
        public void AddEmptySeries_AddsCorrectly()
        {
            // Arrange

            string testFilesPath = "../../../test_files/CountSeries/";
            int tapesCount = 3;

            // Act

            PolyphaseSort ps = new PolyphaseSort();
            ps.tapesCount = tapesCount;
            ps.lastTapeIndex = tapesCount - 1;

            Tape[] tapes = new Tape[tapesCount];
            for (int i = 0; i < tapes.Length; i++)
            {
                tapes[i] = new Tape();
                File.Copy($"{testFilesPath}tape{i}", $"tape{i}");
            }

            ps.CountSeries(tapes);

            tapes[^1].TotalSeries = 0;
            tapes[^1].RealSeries = 0;

            ps.AddEmptySeries(tapes);

            for (int i = 0; i < tapes.Length; i++)
            {
                File.Delete($"tape{i}");
            }

            //Assert

            int[] emptySeries = { 9, 1, 0 };

            for (int i = 0; i < emptySeries.Length; i++)
            {
                Assert.Equal(emptySeries[i], tapes[i].EmptySeries);
            }
        }
    }
}