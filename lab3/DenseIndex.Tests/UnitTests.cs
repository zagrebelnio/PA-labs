using System.Data;
using Xunit.Sdk;

namespace DenseIndex.Tests
{
    public class DatabaseTests
    {
        private readonly string testDbFilePath = "test_files/db/db.txt";
        private readonly string testDbSaveToPath = "test_files/db/length.txt";
        
        [Fact]
        public void AddRecord_AddsRecordToDatabase()
        {
            // Arrange
            File.WriteAllText(testDbFilePath,
                "1 some\n" +
                "2 data\n" +
                "3 for\n" +
                "4 unit\n" +
                "5 tests\n");

            File.WriteAllText(testDbSaveToPath, "5");

            int initialSize = 5;
            int initialValuesNum = 5;

            Database db = new Database(testDbFilePath, testDbSaveToPath, initialSize, initialValuesNum);

            // Act
            db.AddRecord(6, "new_record");

            // Assert
            Assert.Equal(initialSize + 1, db.size);
            Assert.Equal(initialValuesNum + 1, db.valuesNum);
            Assert.Equal("6 new_record", File.ReadAllLines(testDbFilePath)[^1]);
        }

        [Fact]
        public void Edit_EditsRecordInDatabase()
        {
            // Arrange

            File.WriteAllText(testDbFilePath,
                "1 some\n" +
                "2 data\n" +
                "3 for\n" +
                "4 unit\n" +
                "5 tests\n");

            File.WriteAllText(testDbSaveToPath, "5");

            int initialSize = 5;
            int initialValuesNum = 5;

            Database db = new Database(testDbFilePath, testDbSaveToPath, initialSize, initialValuesNum);

            // Act

            db.Edit(4, "program");

            // Assert
            Assert.Equal("4 program", File.ReadAllLines(testDbFilePath)[3]);
        }

        [Fact]
        public void Delete_DeletesRecordInDatabase()
        {
            // Arrange

            File.WriteAllText(testDbFilePath,
                 "1 some\n" +
                 "2 data\n" +
                 "3 for\n" +
                 "4 unit\n" +
                 "5 tests\n");

            File.WriteAllText(testDbSaveToPath, "5");

            int initialSize = 5;
            int initialValuesNum = 5;

            Database db = new Database(testDbFilePath, testDbSaveToPath, initialSize, initialValuesNum);

            // Act

            db.Delete(3);

            // Assert

            Assert.Equal(initialSize, db.size);
            Assert.Equal(initialValuesNum - 1, db.valuesNum);
            Assert.Equal("", File.ReadAllLines(testDbFilePath)[2]);
        }
    }

    public class IndexTests
    {
        private readonly string testIndexFilePath = "test_files/index/index.txt";
        private readonly string testIndexSaveToFilePath = "test_files/index/max_size.txt";
        private readonly string deletedIndexesSaveTo = "test_files/deleted_indexes/indexes.txt";

        [Fact]
        public void Search_FindsValue()
        {
            // Arrange

            int[] rows = { 4, 7, 1, 10, 5, 2, 3, 9, 6, 8 };

            using (StreamWriter sw = new StreamWriter(testIndexFilePath))
            {
                for (int i = 1; i <= rows.Length; i++)
                {
                    sw.WriteLine($"{i} {rows[i - 1]}");
                }
            }

            using (StreamWriter sw = new StreamWriter(testIndexSaveToFilePath)) { }

            int maxSize = 10;
            int size = 10;

            IndexFile index = new IndexFile(testIndexFilePath, testIndexSaveToFilePath, maxSize, size, deletedIndexesSaveTo);

            // Act

            int[] results = new int[rows.Length];

            for (int i = 1; i <= rows.Length; i++)
            {
                results[i - 1] = index.Search(i);
            }

            // Assert

            for (int i = 0; i < results.Length; i++)
            {
                Assert.Equal(results[i], rows[i]);
            }

        }

        [Fact]
        public void Add_AddsValue()
        {
            // Arrange

            int[] rows = { 4, 7, 1, 5, 2, 3, 9, 6, 8 };

            using (StreamWriter sw = new StreamWriter(testIndexFilePath))
            {
                for (int i = 1; i <= rows.Length; i++)
                {
                    sw.WriteLine($"{i} {rows[i - 1]}");
                }
            }

            using (StreamWriter sw = new StreamWriter(testIndexSaveToFilePath)) { }

            int maxSize = 10;
            int size = 9;

            IndexFile index = new IndexFile(testIndexFilePath, testIndexSaveToFilePath, maxSize, size, deletedIndexesSaveTo);

            int row = 10;
            int db_size = 10;

            // Act

            int key = index.Add(row, db_size);

            // Assert

            Assert.Equal(row, key);
            Assert.Equal("10 10", File.ReadAllLines(testIndexFilePath)[^1]);
        }

        [Fact]
        public void Delete_DeletesValue()
        {
            // Arrange

            int[] rows = { 4, 7, 1, 10, 5, 2, 3, 9, 6, 8 };

            using (StreamWriter sw = new StreamWriter(testIndexFilePath))
            {
                for (int i = 1; i <= rows.Length; i++)
                {
                    sw.WriteLine($"{i} {rows[i - 1]}");
                }
            }

            using (StreamWriter sw = new StreamWriter(testIndexSaveToFilePath)) { }

            int maxSize = 10;
            int size = 10;

            IndexFile index = new IndexFile(testIndexFilePath, testIndexSaveToFilePath, maxSize, size, deletedIndexesSaveTo);

            // Act

            int row = index.Delete(7);

            // Assert

            Assert.Equal(3, row);
            Assert.Equal("6 2", File.ReadAllLines(testIndexFilePath)[5]);
            Assert.Equal("8 9", File.ReadAllLines(testIndexFilePath)[6]);
        }
    }

    public class OverflowAreaTests
    {
        private readonly string testAreaFilePath = "test_files/overflow_area/overflow.txt";
        private readonly string deletedIndexesSaveTo = "test_files/deleted_indexes/indexes.txt";

        [Fact]
        public void Search_FindsValue()
        {
            // Arrange

            int size = 5;

            using (StreamWriter sw = new StreamWriter(testAreaFilePath))
            {
                for (int i = 11; i <= 10 + size; i++)
                {
                    sw.WriteLine($"{i} {i}");
                }
            }

            OverflowArea area = new OverflowArea(testAreaFilePath, size, deletedIndexesSaveTo);

            // Act

            int row = area.Search(15);

            // Assert

            Assert.Equal(15, row);
        }

        [Fact]
        public void Add_AddsValue()
        {
            // Arrange

            int size = 5;

            using (StreamWriter sw = new StreamWriter(testAreaFilePath))
            {
                for (int i = 11; i <= 10 + size; i++)
                {
                    sw.WriteLine($"{i} {i}");
                }
            }

            OverflowArea area = new OverflowArea(testAreaFilePath, size, deletedIndexesSaveTo);

            // Act

            int key = area.Add(16);

            // Assert

            Assert.Equal(size + 1, area.size);
            Assert.Equal("16 16", File.ReadAllLines(testAreaFilePath)[^1]);
        }

        [Fact]
        public void Delete_DeletesValue()
        {
            // Arrange

            int size = 5;

            using (StreamWriter sw = new StreamWriter(testAreaFilePath))
            {
                for (int i = 11; i <= 10 + size; i++)
                {
                    sw.WriteLine($"{i} {i}");
                }
            }

            OverflowArea area = new OverflowArea(testAreaFilePath, size, deletedIndexesSaveTo);

            // Act

            int row = area.Delete(13);

            // Assert

            Assert.Equal(13, row);
            Assert.Equal(size - 1, area.size);
            Assert.Equal("12 12", File.ReadAllLines(testAreaFilePath)[1]);
            Assert.Equal("14 14", File.ReadAllLines(testAreaFilePath)[2]);
        }
    }
}