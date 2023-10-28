using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataStructure;
using Board;
using ExperimentData;

namespace InformariveSearch
{
    public class AStar
    {
        public class Node : DataStructure.Node
        {
            public Node Parent;
            public int G;
            public int H;
            public int F;

            public Node(int[] state, Node parent = null) : base(state)
            {
                this.Parent = parent;
                this.G = parent == null ? 0 : parent.G + 1;
                this.H = this.CalculateHeuristic();
                this.F = this.G + this.H;
            }

            public int CalculateHeuristic()
            {
                int count = 0;
                for (int i = 0; i < State.Length; i++)
                {
                    for (int j = i + 1; j < State.Length; j++)
                    {
                        if (State[i] == State[j] || State[i] - i == State[j] - j || State[i] + i == State[j] + j)
                        {
                            count++;
                        }
                    }
                }
                return count;
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
                            children[index] = new Node(newState, this);
                            index++;
                        }
                    }
                }
                return children;
            }
        }

        public static Results Search(int[] board)
        {
            Console.WriteLine("___.:A* algorithm:.___");
            Results results = new Results();
            Node start = new Node(board);
            Console.WriteLine("Start state:");
            Chessboard.PrintResult(start.State);
            List<Node> openList = new List<Node>();
            List<Node> closedList = new List<Node>();

            openList.Add(start);

            while (openList.Count > 0)
            {
                results.Steps++;
                Node current = openList.OrderBy(node => node.F).First();

                if (current.H == 0)
                {
                    results.Board = current.State;
                    results.StatesInMemory = openList.Count + closedList.Count;
                    return results;
                }

                openList.Remove(current);
                closedList.Add(current);

                foreach (Node child in current.GenerateChildren())
                {
                    if (closedList.Any(x => x.State.SequenceEqual(child.State)))
                    {
                        continue;
                    }

                    if (!openList.Any(x => x.State.SequenceEqual(child.State)))
                    {
                        results.States++;
                        openList.Add(child);
                    }
                    else
                    {
                        Node existing = openList.First(x => x.State.SequenceEqual(child.State));
                        if (child.G < existing.G)
                        {
                            results.States++;
                            openList.Remove(existing);
                            openList.Add(child);
                        }
                    }
                }

            }

            return null;
        }
    }
}
