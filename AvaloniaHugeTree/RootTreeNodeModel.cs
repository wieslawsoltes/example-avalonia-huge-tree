using Avalonia.Collections;
using Avalonia.Threading;

namespace AvaloniaHugeTree;

public class RootTreeNodeModel : TreeNodeModel
{
    public IAvaloniaReadOnlyList<TreeNodeModel> VisibleChildren => _visibleChildren;
    private AvaloniaList<TreeNodeModel> _visibleChildren = new AvaloniaList<TreeNodeModel>();

    public RootTreeNodeModel()
    {
        _root = new Root(this);
    }

    class Root : ITreeNodeRoot
    {
        private readonly RootTreeNodeModel _root;
        private bool _updateEnqueued;

        public Root(RootTreeNodeModel root)
        {
            _root = root;
        }

        public void EnqueueUpdate()
        {
            if (!_updateEnqueued)
            {
                _updateEnqueued = true;
                Dispatcher.UIThread.Post(Update, DispatcherPriority.Background);
            }
        }

        private static void AppendItems(AvaloniaList<TreeNodeModel> list, TreeNodeModel node)
        {
            list.Add(node);
            if(node.IsExpanded)
                foreach(var ch in node.Children)
                    AppendItems(list, ch);
        }

        public void Update()
        {
            _updateEnqueued = false;
            var list = new AvaloniaList<TreeNodeModel>();
            AppendItems(list, _root);

            _root._visibleChildren = new AvaloniaList<TreeNodeModel>(list);
            _root.RaisePropertyChanged(nameof(_root.VisibleChildren));
        }
    }

    public void ForceResync() => _root.Update();
}