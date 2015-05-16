using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace Distributr.WPF.Lib.UI.Hierarchy
{
    public class GridDef:INotifyPropertyChanged
    {
        public GridDef()
        {
            _columns = new List<ColumnDef>();
            _source = new ObservableCollection<RowDef>();

            LoadData();

            RowDef.RowExpanding += new Action<RowDef>(RowDef_RowExpanding);
            RowDef.RowCollapsing += new Action<RowDef>(RowDef_RowCollapsing);
        }
        void RowDef_RowExpanding(RowDef row)
        {
            foreach (RowDef child in row.Children)
                child.IsVisible = true;
            OnPropertyChanged("Display");
        }
        void RowDef_RowCollapsing(RowDef row)
        {
            foreach (RowDef child in row.Children)
            {
                if (row.IsExpanded.HasValue && row.IsExpanded.Value)
                    RowDef_RowCollapsing(child);
                child.IsVisible = false;
            }
            OnPropertyChanged("Display");
        }
        private void LoadData()
        {
            //_columns = new List<ColumnDef>() {
            //    new ColumnDef() { Title = "Col1" },
            //    new ColumnDef() { Title = "Col2" }
            //};
            //XDocument doc = XDocument.Load("Data.xml");
            //LoadData(_source, doc.Element("Rows").Elements("Row"), null);
            _source[0].IsVisible = true;
        }
        //private int LoadData(IList<RowDef> srce, IEnumerable<XElement> rows, RowDef parent)
        //{
        //    int count = 0;
        //    //foreach (XElement r in rows)
        //    //{
        //    //    RowDef row = new RowDef(parent) {IsVisible = false, Cells = new ObservableCollection<string>() { r.Attribute("Val1").Value, r.Attribute("Val2").Value } };
        //    //    srce.Add(row);
        //    //    ++count;
        //    //    int children = LoadData(row.Children, r.Elements("Row"), row);
        //    //    if (children > 0)
        //    //        row.IsExpanded = false;
        //    //}
        //    return count;
        //}
        ObservableCollection<RowDef> _source;
        public ObservableCollection<RowDef> Source
        {
            get { return _source; }
        }
        List<ColumnDef> _columns;
        public List<ColumnDef> Columns
        {
            get { return _columns; }
            private set { _columns = value; }
        }

        public IEnumerable<RowDef> Display
        {
            get 
            {
                //TODO: How to do this with multiple roots?
                return IterateTree(_source[0]);
            }
        }
        IEnumerable<RowDef> IterateTree(RowDef parent)
        {
            if (!parent.IsVisible)
                yield break;
            yield return parent;
                foreach (RowDef child in parent.Children)
                {
                    foreach (RowDef r in IterateTree(child))
                    {
                        yield return r;
                    }
                }
        }
        protected virtual void OnPropertyChanged(string prop)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }
        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion
    }
}
