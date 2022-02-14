using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Thorn
{
    class Tree<T>
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
    }
}
