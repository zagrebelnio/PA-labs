using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataStructure;
using Board;
using ExperimentData;

namespace NonInformativeSearch
{
    public class BFS
    {
        public class Node : DataStructure.Node
        {
            public Node(int[] state) : base(state) { }

            public bool IsGoalState()
            {
                for (int i = 0; i < this.State.Length; i++)
                {
                    for (int j = i + 1; j < this.State.Length; j++)
                    {
                        if (this.State[i] == this.State[j] ||
                            this.State[i] - i == this.State[j] - j ||
                            this.State[i] + i == this.State[j] + j)
                        {
                            return false;
                        }

                    }
                }
                return true;
            }

            public new Node[] GenerateChildren()
            {
                int length = this.State.Length * (this.State.Length - 1);
                Node[] children = new Node[length];
                for (int i = 0, index = 0; i < this.State.Length; i++)
                {
                    for (int j = 0; j < this.State.Length; j++)
                    {
                        if (this.State[i] != j)
                        {
                            int[] newState = new int[this.State.Length];
                            Array.Copy(this.State, newState, newState.Length);
                            newState[i] = j;
                            children[index] = new Node(newState);
                            index++;
                        }
                    }
                }
                return children;
            }
        }

        public static Results Search(int[] board)
        {
            Console.WriteLine("___.:BFS algorithm:.___");
            Results results = new Results();
            Node start = new Node(board);
            Console.WriteLine("Start state:");
            Chessboard.PrintResult(start.State);
            Queue<Node> openQueue = new Queue<Node>();
            HashSet<string> closedSet = new HashSet<string>();

            openQueue.Enqueue(start);

            while (openQueue.Count > 0)
            {
                results.Steps++;
                Node current = openQueue.Dequeue();
                if (current.IsGoalState())
                {
                    results.Board = current.State;
                    results.StatesInMemory = openQueue.Count + closedSet.Count;
                    return results;
                }

                closedSet.Add(string.Join(",", current.State));

                foreach (Node child in current.GenerateChildren())
                {
                    if (!closedSet.Contains(string.Join(",", child.State)))
                    {
                        results.States++;
                        openQueue.Enqueue(child);
                    }
                }
            }

            return null;
        }
    }
}