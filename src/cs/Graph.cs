using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Graph
{
    public class NamedGraph<T>
    {
        public NamedGraph(string rootNode, T data, List<string> children)
        {
            root = rootNode;
            Add(rootNode, data, children);
        }
        internal Dictionary<string, Node<T>> nodes = new Dictionary<string, Node<T>>();
        private string root;
        public void Add(string index, T data, List<string> children) => nodes.Add(index, new Node<T>(this, index, data, children));
        public Node<T> this[int i]
        {
            get
            {
                return nodes[root][i];
            }
        }
        public string Name
        {
            get
            {
                return root;
            }
            set
            {
                Node<T> temp = nodes[root];
                nodes[value] = temp;
                nodes[value].Name = value;
                root = value;
            }
        }
        public T Data
        {
            get
            {
                return nodes[root].Data;
            }
            set
            {
                nodes[root].Data = value;
            }
        }
        public void ChangeRoot(string newRoot)
        {
            root = newRoot;
        }
    }
    public class Node<T>
    {
        private NamedGraph<T> Parent;
        public string Name;
        public T Data;
        private List<string> Children;

        internal Node(NamedGraph<T> parent, string name, T data, List<string> children)
        {
            Parent = parent;
            Name = name;
            Data = data;
            Children = children;
        }
        public Node<T> this[int i]
        {
            get => Parent.nodes[Children[i]];
        }
    }
}
