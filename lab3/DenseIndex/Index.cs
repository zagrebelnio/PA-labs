using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Documents;

namespace DenseIndex
{
    public class IndexFile
    {
        public string filePath = "database_data/index.txt";
        public string saveTo = "database_data/save/index_maxsize.txt";
        public int maxSize;
        public int size;
        private DeletedIndexes deletedIndexes;
        public int comparisons = 0;

        public IndexFile(DeletedIndexes deletedIndexes)
        {
            this.deletedIndexes = deletedIndexes;
            size = File.ReadAllLines(filePath).Length;
            maxSize = int.Parse(File.ReadAllText(saveTo));
        }

        public IndexFile(string filePath, string saveTo, int maxSize, int size, string deletedIndexesSaveTo)
        {
            this.filePath = filePath;
            this.saveTo = saveTo;
            this.maxSize = maxSize;
            this.size = size;
            deletedIndexes = new DeletedIndexes(deletedIndexesSaveTo);
        }

        ~IndexFile()
        {
            File.WriteAllText(saveTo, maxSize.ToString());
        }

        public bool isFull()
        {
            return size == maxSize;
        }

        public int Search(int key, int index = int.MinValue, int delta = 1)
        {
            if (index != int.MinValue)
            {
                string[] data = new string[2];

                if (index > size)
                {
                    data[0] = int.MaxValue.ToString();
                }
                else if (index < 1)
                {
                    data[0] = int.MinValue.ToString();
                }
                else
                {
                    string line = File.ReadLines(filePath).Skip(index - 1).Take(1).First();
                    data = line.Split(' ');
                }

                comparisons++;
                if (int.Parse(data[0]) == key)
                {
                    return int.Parse(data[1]);
                }

                if (delta == 0)
                {
                    return -1;
                }

                delta /= 2;

                if (int.Parse(data[0]) < key)
                {
                    index += delta + 1;
                    return Search(key, index, delta);
                }
                else
                {
                    index -= delta + 1;
                    return Search(key, index, delta);
                }
            }
            else
            {
                comparisons = 0;
                delta = size / 2;
                index = delta + 1;
                return Search(key, index, delta);
            }
        }

        public int Add(int key, int db_size, int index = int.MinValue, int delta = 1)
        {
            if (index == int.MinValue)
            {
                delta = size / 2;
                if (deletedIndexes.set.Count != 0)
                {
                    key = deletedIndexes.set.First();
                    deletedIndexes.set.Remove(key);
                }
                index = delta + 1;
                return Add(key, db_size, index, delta);
            }
            else
            {
                int current;
                int next;

                if (index > size)
                {
                    current = int.MaxValue;
                }
                else if (index < 1)
                {
                    current = int.MinValue;
                }
                else
                {
                    string line = File.ReadLines(filePath).Skip(index - 1).Take(1).First();
                    current = int.Parse(line.Split(' ')[0]);
                }

                if (index + 1 > size)
                {
                    next = int.MaxValue;
                }
                else if (index + 1 < 1)
                {
                    next = int.MinValue;
                }
                else
                {
                    string line = File.ReadLines(filePath).Skip(index).Take(1).First();
                    next = int.Parse(line.Split(' ')[0]);
                }

                if (current < key && next > key)
                {
                    using (StreamReader sr = new StreamReader(filePath))
                    using (StreamWriter sw = new StreamWriter("temp_index.txt"))
                    {
                        for (int i = 0; i < index; i++)
                        {
                            sw.WriteLine(sr.ReadLine());
                        }
                        sw.WriteLine($"{key} {db_size}");
                        while (!sr.EndOfStream)
                        {
                            sw.WriteLine(sr.ReadLine());
                        }
                    }
                    size++;
                    File.Delete(filePath);
                    File.Move("temp_index.txt", filePath);
                    return key;
                }

                if (current < key)
                {
                    delta /= 2;
                    index += delta + 1;
                    return Add(key, db_size, index, delta);
                }
                else
                {
                    index -= delta + 1;
                    return Add(key, db_size, index, delta);
                }

            }
        }

        public int Delete(int key, int index = int.MinValue, int delta = 1)
        {
            if (index != int.MinValue)
            {
                string[] data = new string[2];

                if (index > size)
                {
                    data[0] = int.MaxValue.ToString();
                }
                else if (index < 1)
                {
                    data[0] = int.MinValue.ToString();
                }
                else
                {
                    string line = File.ReadLines(filePath).Skip(index - 1).Take(1).First();
                    data = line.Split(' ');
                }

                if (int.Parse(data[0]) == key)
                {
                    deletedIndexes.set.Add(key);
                    size--;
                    using (StreamReader r = new StreamReader(filePath))
                    using (StreamWriter w = new StreamWriter("temp_index.txt"))
                    {
                        string line;
                        for (int i = 0; i < index - 1; i++)
                        {
                            w.WriteLine(r.ReadLine());
                        }
                        r.ReadLine();
                        while (!r.EndOfStream)
                        {
                            w.WriteLine(r.ReadLine());
                        }
                    }
                    File.Delete(filePath);
                    File.Move("temp_index.txt", filePath);
                    return int.Parse(data[1]);
                }

                if (delta == 0)
                {
                    return -1;
                }

                delta /= 2;

                if (int.Parse(data[0]) < key)
                {
                    index += delta + 1;
                    return Delete(key, index, delta);
                }
                else
                {
                    index -= delta + 1;
                    return Delete(key, index, delta);
                }
            }
            else
            {
                delta = size / 2;
                index = delta + 1;
                return Delete(key, index, delta);
            }
        }
    }

    public class DeletedIndexes
    {
        public string saveTo = "database_data/save/indexes.txt";
        public SortedSet<int> set = new SortedSet<int>();

        public DeletedIndexes()
        {
            using (StreamReader sr = new StreamReader(saveTo))
            {
                while (!sr.EndOfStream)
                {
                    set.Add(int.Parse(sr.ReadLine()));
                }
            }
        }

        public DeletedIndexes(string saveTo)
        {
            this.saveTo = saveTo;
        }

        ~DeletedIndexes ()
        {
            using (StreamWriter sw = new StreamWriter(saveTo))
            {
                foreach (var index in set)
                {
                    sw.WriteLine(index);
                }
            }
        }
    }
}
