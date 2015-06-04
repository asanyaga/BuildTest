using System.Collections.Generic;
using System.Linq;
using Android.Content;
using Android.Widget;
using Mobile.Common.Core.Data;

namespace Mobile.Common.Core.Views
{
    // A ListAdapter that displays a maximum of 100 items. It also pre-loads pages in both directions
    // and adds or removes them automatically depending on the lists scroll poistion ie Infinite Scrolling
    // Users of this class should call Initialise, passing in their base IDataSource. This class will
    // ask the IDataSource for the correct page, depending on where the list scoll position currently is. 
    // You can call Initialise multiple times which is useful for when the User changes the filter parameters. 
    public class FixedSizeListAdapter<T> : ArrayAdapter<T>, AbsListView.IOnScrollListener where T : new()
    {
        private List<T> currentItems = new List<T>();
        private List<T> previousPage;
        private List<T> nextPage;

        private const int PageSize = 50;
        //The offset into the underlying database table.
        private int offset;
        private int _firstVisibleItem;
       
        private IDataSource<T> _dataSource;

      
        public FixedSizeListAdapter(Context context)
            : base(context, Android.Resource.Layout.SimpleListItem1, new List<T>())
        {
        }

        public void Initialise(IDataSource<T> dataSource)
        {
            _dataSource = dataSource;
            previousPage = new List<T>();
            nextPage = new List<T>();
            offset = 0;
            _firstVisibleItem = 0;

            currentItems = _dataSource.Fetch(offset, PageSize*2);
            //Move the offset so that we will advance through the IDataSource records
            offset = PageSize * 2;

            FetchNextPage();

            UpdateItems();
        }

        private async void FetchNextPage()
        {
            nextPage = await _dataSource.FetchAsync(offset, PageSize);
        }

        private async void FetchPreviousPage()
        {
            var previousPageOffset = offset - PageSize * 3;
            previousPage = await _dataSource.FetchAsync(previousPageOffset, PageSize);
        }

        public void OnScroll(AbsListView view, int firstVisibleItem, int visibleItemCount, int totalItemCount)
        {
            _firstVisibleItem = firstVisibleItem;
        }

        public void OnScrollStateChanged(AbsListView view, ScrollState scrollState)
        {
            //Are we scrolling forwards, beyond three-quarters of the way throught the list?
            var threshold = (PageSize * 2) * 0.75;

            if (_firstVisibleItem >= threshold && nextPage.Any())
            {
                //Get the precise position of the item currently showing at the top of the ListView
                var currentPosition = RecordCurrentPosition(view);
                RemoveFromStart();
                AddToEnd();
                //Restore the item to the top of the ListView
                RestorePosition(view, currentPosition, PageSize);
            }
            //Are we scrolling backwards, into the last quarter of items and have previous pages to reshow?
            else if (_firstVisibleItem < PageSize * 0.5 && previousPage.Any())
            {
                var currentPosition = RecordCurrentPosition(view);
                RemoveFromEnd();
                AddToStart();
                RestorePosition(view, currentPosition, -PageSize);
            }
        }

        private void UpdateItems()
        {
            Clear();
            AddAll(currentItems);
        }

        private ViewItemPosition RecordCurrentPosition(AbsListView view)
        {
            var pos = view.FirstVisiblePosition;
            var topItem = view.GetChildAt(0);
            var top = topItem == null ? 0 : (topItem.Top - view.ListPaddingTop);
            return new ViewItemPosition(pos, top);
        }

        private void RestorePosition(AbsListView view, ViewItemPosition position, int pageOffset)
        {
            view.SetSelectionFromTop(position.Position - pageOffset, position.Offset);
        }

        private void AddToStart()
        {
            currentItems.InsertRange(0, previousPage);
            if (offset > PageSize * 2)
            {
                FetchPreviousPage();
            }
            else
            {
                previousPage = new List<T>();
            }
            UpdateItems();
        }

        private void RemoveFromEnd()
        {
            var count = currentItems.Count - PageSize;
            nextPage = currentItems.GetRange(PageSize, count);
            currentItems.RemoveRange(PageSize, count);
            offset -= PageSize;
        }

        private void AddToEnd()
        {
            currentItems.AddRange(nextPage);
            offset += PageSize;
            FetchNextPage();
            UpdateItems();
        }

        private void RemoveFromStart()
        {
            previousPage = currentItems.GetRange(0, PageSize);
            currentItems.RemoveRange(0, PageSize);
        }
    }

    public class ViewItemPosition
    {
        public ViewItemPosition(int position, int offset)
        {
            Position = position;
            Offset = offset;
        }

        public int Position { get; private set; }
        public int Offset { get; private set; }
    }
}