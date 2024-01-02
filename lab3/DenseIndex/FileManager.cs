using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json;
using System.Windows.Automation.Peers;

namespace DenseIndex
{
    public class FileManager
    {
        public Database db;
        public IndexFile index;
        public OverflowArea area;
        public int comparisons = 0;

        public FileManager()
        {
            DeletedIndexes deletedIndexes = new DeletedIndexes();
            db = new Database();
            index = new IndexFile(deletedIndexes);
            area = new OverflowArea(deletedIndexes);
        }

        public async Task<string> GenerateData(int size)
        {
            Stopwatch sw = new Stopwatch();
            sw.Start();

            Dictionary<int, string> data = new Dictionary<int, string>();
            List<string> file_data = File.ReadAllLines("database_data/data.txt").ToList();
            Random random = new Random();

            for (int i = 1; i <= size; i++)
            {
                int randIndex = random.Next(file_data.Count);
                string username = file_data[randIndex];
                file_data.RemoveAt(randIndex);
                data.Add(i, username);
            }

            data = data.OrderBy(x => random.Next()).ToDictionary(x => x.Key, x => x.Value);

            using (StreamWriter dbWriter = new StreamWriter(db.filePath))
            {
                for (int i = 0; i < size; i++)
                {
                    var row = data.ElementAt(i);
                    dbWriter.WriteLine($"{row.Key} {row.Value}");
                }
            }
            db.size = size;
            db.valuesNum = size;

            using (StreamWriter indexWriter = new StreamWriter(index.filePath))
            {
                for (int i = 1; i <= size; i++)
                {
                    indexWriter.WriteLine($"{i} {new List<int>(data.Keys).IndexOf(i) + 1}");
                }
            }
            index.size = size;
            index.maxSize = size;

            using (StreamWriter w = new StreamWriter (area.filePath)) { }
            area.size = 0;

            using (StreamWriter w = new StreamWriter (new DeletedIndexes().saveTo)) { }
            using (StreamWriter w = new StreamWriter(index.saveTo)) { }

            sw.Stop();

           return $"Generation time: {sw.Elapsed.TotalSeconds:F2} seconds";
        }

        public string Search(int key)
        {
            comparisons = 0;
            int row = index.Search(key);
            comparisons += index.comparisons;

            if (row == -1)
            {
                row = area.Search(key);
                comparisons += area.comparisons;
            }

            return row == -1 ? null : File.ReadLines(db.filePath).Skip(row - 1).Take(1).First().Split(' ')[1];
        }

        public bool Edit(int key, string value)
        {
            int row = index.Search(key);
            if (row == -1)
            {
                row = area.Search(key);
                if (row == -1)
                {
                    return false;
                }
                else
                {
                    db.Edit(row, value);
                    return true;
                }
            }
            else
            {
                db.Edit(row, value);
                return true;
            }
        }

        public void Add(string value)
        {
            int key = db.valuesNum + 1;
            if (index.isFull())
            {
                key = area.Add(key);
            }
            else
            {
                key = index.Add(key, db.valuesNum + 1);
            }
            db.AddRecord(key, value);
        }

        public bool Delete(int key)
        {
            int row = index.Delete(key);
            if (row == -1)
            {
                row = area.Delete(key);
                if (row == -1)
                {
                    return false;
                }
                db.Delete(row);
                return true;
            }
            else
            {
                db.Delete(row);
                return true;
            }
        }
    }
}
