using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Thorn
{
    public class Graph<T> : IEnumerable<Graph<T>>
    {
        private List<Graph<T>> childNodes;
        public T Data;
        public Graph(T data, List<Graph<T>> initalChildren)
        {
            Data = data;
            childNodes = initalChildren;
        }
        public Graph(T data)
        {
            Data = data;
            childNodes = new List<Graph<T>>();
        }
        public Graph<T> this[int i]
        {
            get => childNodes[i];
        }

        public IEnumerator<Graph<T>> GetEnumerator() => childNodes.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => childNodes.GetEnumerator();
        public void Add(Graph<T> subTree) => childNodes.Add(subTree);
    }
}
