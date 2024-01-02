using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace DenseIndex
{
    public class OverflowArea
    {
        public string filePath = "database_data/overflow.txt";
        public int size;
        public DeletedIndexes deletedIndexes;
        public int comparisons = 0;

        public OverflowArea(DeletedIndexes deletedIndexes)
        {
            this.deletedIndexes = deletedIndexes;
            size = File.ReadAllLines(filePath).Length;
        }

        public OverflowArea(string filePath, int size, string deletedIndexesSaveTo)
        {
            this.filePath = filePath;
            this.size = size;
            deletedIndexes = new DeletedIndexes(deletedIndexesSaveTo);
        }

        public int Search(int key)
        {
            comparisons = 0;
            StreamReader reader = new StreamReader(filePath);

            while (!reader.EndOfStream)
            {
                string[] data = reader.ReadLine().Split(' ');
                comparisons++;
                if (int.Parse(data[0]) == key)
                    return int.Parse(data[1]);
            }

            reader.Close();

            return -1;
        }

        public int Add(int key)
        {
            int db_size = key;
            if (deletedIndexes.set.Count != 0)
            {
                key = deletedIndexes.set.First();
                deletedIndexes.set.Remove(key);
            }
            File.AppendAllText(filePath, $"{key} {db_size}\n");
            size++;
            return key;
        }

        public int Delete(int key)
        {
            size--;
            StreamReader reader = new StreamReader(filePath);
            StreamWriter writer = new StreamWriter("temp_overflow.txt");
            string line;
            int row = -1;
            while (!reader.EndOfStream)
            {
                line = reader.ReadLine();
                string[] data = line.Split(' ');
                if (int.Parse(data[0]) == key)
                {
                    row = int.Parse(data[1]);
                    deletedIndexes.set.Add(key);
                }
                else
                {
                    writer.WriteLine(line);
                }
            }

            writer.Close();
            reader.Close();

            File.Delete(filePath);
            File.Move("temp_overflow.txt", filePath);

            return row;
        }
    }
}
