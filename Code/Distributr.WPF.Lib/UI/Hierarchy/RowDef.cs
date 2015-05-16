using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Distributr.WPF.Lib.UI.Hierarchy
{
    public class RowDef
    {
        public RowDef()
        {
            _cells = new ObservableCollection<string>();
            _children = new List<RowDef>();
        }
        public RowDef(RowDef parent)
            : this()
        {
            Parent = parent;
        }
        public static event Action<RowDef> RowExpanding;
        public static event Action<RowDef> RowCollapsing;

        //TODO: Probably should have another class defining Cell, in case you want something more sophisticated than just a string
        ObservableCollection<string> _cells;

        public ObservableCollection<string> Cells
        {
            get { return _cells; }
            internal set { _cells = value; }
        }
        bool? _isExpanded;
        public bool? IsExpanded
        {
            get { return _isExpanded; }
            set 
            {
                if (_isExpanded != value)
                {
                    _isExpanded = value;
                    if (_isExpanded.Value)
                    {
                        if (RowDef.RowExpanding != null)
                            RowDef.RowExpanding(this);
                    }
                    else
                    {
                        if (RowDef.RowCollapsing != null)
                            RowDef.RowCollapsing(this);
                    }
                }
            }
        }
        List<RowDef> _children;
        public List<RowDef> Children
        {
            get { return _children; }
            set { _children = value; }
        }
        RowDef _parent;
        public RowDef Parent
        {
            get { return _parent; }
            private set
            {
                _parent = value;
                if (_parent != null)
                    _level = _parent.Level + 1;
            }
        }
        int _level;
        public int Level
        {
            get { return _level; }
            set { _level = value; }
        }
        bool _isVisible;
        public bool IsVisible
        {
            get { return _isVisible; }
            set { _isVisible = value; }
        }
    }
}

