using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace Distributr.WPF.Lib.UI.UI_Utillity
{
    [TemplatePart(Name = "btnFirst", Type = typeof(Button)),
    TemplatePart(Name = "btnPrev", Type = typeof(Button)),
    TemplatePart(Name = "txtPaginationDetails", Type = typeof(TextBox)),
    TemplatePart(Name = "btnNext", Type = typeof(Button)),
    TemplatePart(Name = "btnLast", Type = typeof(Button)),
    TemplatePart(Name = "cmbPageSizes", Type = typeof(ComboBox))]
    public class PagerControl : Control
    {
        #region variables
        protected Button btnFirstPage, btnPreviousPage, btnNextPage, btnLastPage;
        protected TextBox txtPage;
        protected ComboBox cmbPageSizes;

        #endregion
        static PagerControl()
        {
        }

        #region Properties
        public static readonly DependencyProperty ItemsSourceProperty;
        public static readonly DependencyProperty PageProperty;
        public static readonly DependencyProperty TotalPagesProperty;
        public static readonly DependencyProperty PageSizesProperty;
        public static readonly DependencyProperty PageContractProperty;
        public static readonly DependencyProperty FilterTagProperty;
        public ObservableCollection<object> ItemsSource
        {
            get
            {
                return GetValue(ItemsSourceProperty) as ObservableCollection<object>;
            }
            protected set
            {
                SetValue(ItemsSourceProperty, value);
            }
        }

        public uint Page
        {
            get
            {
                return (uint)GetValue(PageProperty);
            }
            set
            {
                SetValue(PageProperty, value);
            }
        }

        public uint TotalPages
        {
            get
            {
                return (uint)GetValue(TotalPagesProperty);
            }
            protected set
            {
                SetValue(TotalPagesProperty, value);
            }
        }

        public ObservableCollection<uint> PageSizes
        {
            get
            {
                return GetValue(PageSizesProperty) as ObservableCollection<uint>;
            }
        }

        public IPageControlContract PageContract
        {
            get
            {
                return GetValue(PageContractProperty) as IPageControlContract;
            }
            set
            {
                SetValue(PageContractProperty, value);
            }
        }

        public object FilterTag
        {
            get
            {
                return GetValue(FilterTagProperty);
            }
            set
            {
                SetValue(FilterTagProperty, value);
            }
        }
        #endregion

        #region events
        public delegate void PageChangedEventHandler(object sender, PageChangedEventArgs args);

        public static readonly RoutedEvent PreviewPageChangeEvent;
        public static readonly RoutedEvent PageChangedEvent;
        #endregion
    }

    public class PageChangedEventArgs : RoutedEventArgs
    {
        private readonly uint _oldPage, _newPage, _totalPages;

        public PageChangedEventArgs(RoutedEvent eventToRaise, uint oldPage, uint newPage, uint totalPages)
            :base(eventToRaise)
        {
            _oldPage = oldPage;
            _newPage = newPage;
            _totalPages = totalPages;
        }

        #region Properties
        public uint OldPage
        {
            get
            {
                return _oldPage;
            }
        }

        public uint NewPage
        {
            get
            {
                return _newPage;
            }
        }

        public uint TotalPages
        {
            get
            {
                return _totalPages;
            }
        }
        #endregion
    }
}
