using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace DataStructure
{
    public class Node
    {
        public int[] State;

        public Node(int[] state)
        {
            this.State = state;
        }

        public virtual Node[] GenerateChildren()
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

}