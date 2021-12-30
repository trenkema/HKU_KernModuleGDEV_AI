using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BehaviourTree
{
    public enum NodeState
    {
        RUNNING,
        SUCCESS,
        FAILURE
    }

    public class Node
    {
        protected NodeState state;

        public Node parent;
        protected List<Node> children = new List<Node>();

        private Dictionary<string, object> dataContext = new Dictionary<string, object>();

        public Node()
        {
            parent = null;
        }

        public Node(List<Node> _children)
        {
            foreach (Node child in _children)
            {
                Attach(child);
            }
        }

        private void Attach(Node _node)
        {
            _node.parent = this;
            children.Add(_node);
        }

        public virtual NodeState Evaluate() => NodeState.FAILURE;

        public void SetData(string _key, object _value)
        {
            dataContext[_key] = _value;
        }

        public object GetData(string _key)
        {
            object value = null;

            if (dataContext.TryGetValue(_key, out value))
                return value;

            Node node = parent;

            while (node != null)
            {
                value = node.GetData(_key);

                if (value != null)
                    return value;

                node = node.parent;
            }

            return null;
        }

        public bool ClearData(string _key)
        {
            if (dataContext.ContainsKey(_key))
            {
                dataContext.Remove(_key);
                return true;
            }

            Node node = parent;

            while (node != null)
            {
                bool cleared = node.ClearData(_key);

                if (cleared)
                    return true;

                node = node.parent;
            }

            return false;
        }
    }
}
