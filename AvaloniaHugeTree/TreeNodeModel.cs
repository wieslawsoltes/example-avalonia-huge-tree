using System;
using System.Collections.Generic;

namespace AvaloniaHugeTree
{
    public class TreeNodeModel : PropertyChangedBase
    {
        private bool _isExpanded;
        private string _name;
        private List<TreeNodeModel> _children = new List<TreeNodeModel>();
        protected ITreeNodeRoot _root;
        public int Level { get; private set; }
        
        protected interface ITreeNodeRoot
        {
            void EnqueueUpdate();
            void Update();
        }
        
        public bool IsExpanded
        {
            get => _isExpanded;
            set
            {
                SetAndRaise(ref _isExpanded, value);
                _root?.EnqueueUpdate();
            }
        }

        public string Name
        {
            get => _name;
            set => SetAndRaise(ref _name, value);
        }

        public IReadOnlyList<TreeNodeModel> Children => _children;

        public void InsertChild(int index, TreeNodeModel child)
        {
            if (child._root != null)
                throw new InvalidOperationException();
            _children.Insert(index, child);
            child.SetRoot(_root, Level + 1);
            _root?.EnqueueUpdate();
        }

        public void RemoveChildAt(int index)
        {
            var child = _children[index];
            _children.RemoveAt(index);
            child.SetRoot(null, 0);
            _root?.EnqueueUpdate();
        }

        protected void SetRoot(ITreeNodeRoot root, int level)
        {
            _root = root;
            Level = root == null ? -1 : level;
            foreach (var child in _children) 
                child.SetRoot(root, level + 1);
        }
        
        public void RemoveChild(TreeNodeModel child)
        {
            var idx = _children.IndexOf(child);
            if (idx != -1)
                RemoveChildAt(idx);
        }

        public void AddChild(TreeNodeModel child) => InsertChild(_children.Count, child);
    }
}
