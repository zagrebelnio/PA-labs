using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DenseIndex
{
    public class Database
    {
        public string filePath = "database_data/db.txt";
        public string saveTo = "database_data/save/db_length.txt";
        public int size;
        public int valuesNum;

        public Database()
        {
            size = File.ReadAllLines(filePath).Length;
            valuesNum = int.Parse(File.ReadAllText(saveTo));
        }

        public Database(string filePath, string saveTo, int size, int valuesNum)
        {
            this.filePath = filePath;
            this.saveTo = saveTo;
            this.size = size;
            this.valuesNum = valuesNum;
        }

        ~Database()
        {
            File.WriteAllText(saveTo, valuesNum.ToString());
        }

        public void AddRecord(int key, string value)
        {
            File.AppendAllText(filePath, $"{key} {value}\n");
            size++;
            valuesNum++;
        }

        public void Edit(int row, string newValue)
        {
            string[] lines = File.ReadAllLines(filePath);
            string[] data = lines[row - 1].Split(' ');
            lines[row - 1] = $"{data[0]} {newValue}";
            File.WriteAllLines(filePath, lines);
        }

        public void Delete(int row)
        {
            valuesNum--;
            using (StreamReader r = new StreamReader(filePath))
            using (StreamWriter w = new StreamWriter("temp_db.txt"))
            {
                string line;
                for (int i = 0; i < row - 1; i++)
                {
                    w.WriteLine(r.ReadLine());
                }
                r.ReadLine();
                w.WriteLine();
                while (!r.EndOfStream)
                {
                    w.WriteLine(r.ReadLine());
                }
            }
            File.Delete(filePath);
            File.Move("temp_db.txt", filePath);
        }
    }
}
