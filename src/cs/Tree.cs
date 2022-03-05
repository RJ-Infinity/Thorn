using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Thorn
{
    public class Tree<T> : IEnumerable<Tree<T>>
    {
        private List<Tree<T>> childNodes;
        public T Data;
        public Tree(T data, List<Tree<T>> initalChildren)
        {
            Data = data;
            childNodes = initalChildren;
        }
        public Tree(T data)
        {
            Data = data;
            childNodes = new List<Tree<T>>();
        }
        public Tree<T> this[int i]
        {
            get => childNodes[i];
        }

        public IEnumerator<Tree<T>> GetEnumerator() => childNodes.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => childNodes.GetEnumerator();
        public void Add(Tree<T> subTree) => childNodes.Add(subTree);
    }
}
