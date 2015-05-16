using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Markup;
using System.Windows.Media;
using Point = System.Windows.Point;

namespace Distributr.WPF.Lib.ViewModels.Printerutilis
{
    public class CustomDataGridDocumentPaginator : DocumentPaginator
    {
        #region Private Members

        private DataGrid _documentSource;

        private Collection<ColumnDefinition> _tableColumnDefinitions;

        private double _avgRowHeight;

        private double _availableHeight;

        private int _rowsPerPage;

        private int _pageCount;

        #endregion

        #region Constructor

        public CustomDataGridDocumentPaginator(DataGrid documentSource, string documentTitle, Size pageSize,
                                               Thickness pageMargin)
        {
            _tableColumnDefinitions = new Collection<ColumnDefinition>();
            _documentSource = documentSource;

            _tableColumnDefinitions = new Collection<ColumnDefinition>();
            _documentSource = documentSource;

            this.DocumentTitle = documentTitle;
            this.PageSize = pageSize;
            this.PageMargin = pageMargin;

            if (_documentSource != null)
                MeasureElements();
        }

        #endregion

        #region Public Properties

        #region Styling

        public Style AlternatingRowBorderStyle { get; set; }

        public Style DocumentHeaderTextStyle { get; set; }

        public Style DocumentFooterTextStyle { get; set; }

        public Style TableCellTextStyle { get; set; }

        public Style TableHeaderTextStyle { get; set; }

        public Style TableHeaderBorderStyle { get; set; }

        public Style GridContainerStyle { get; set; }

        #endregion

        public string DocumentTitle { get; set; }

        public Thickness PageMargin { get; set; }

        public override Size PageSize { get; set; }

        public override bool IsPageCountValid
        {
            get { return true; }
        }

        public override int PageCount
        {
            get { return _pageCount; }
        }

        public override IDocumentPaginatorSource Source
        {
            get { return null; }
        }

        #endregion

        #region Public Methods

        public override DocumentPage GetPage(int pageNumber)
        {
            DocumentPage page = null;
            List<object> itemsSource = new List<object>();

            ICollectionView viewSource = CollectionViewSource.GetDefaultView(_documentSource.ItemsSource);

            if (viewSource != null)
            {
                foreach (object item in viewSource)
                    itemsSource.Add(item);
            }

            if (itemsSource != null)
            {
                int rowIndex = 1;
                int startPos = pageNumber*_rowsPerPage;
                int endPos = startPos + _rowsPerPage;

                //Create a new grid
                Grid tableGrid = CreateTable(true) as Grid;

                tableGrid.ShowGridLines = true;

                for (int index = startPos; index < endPos && index < itemsSource.Count; index++)
                {
                    Console.WriteLine("Adding: " + index);

                    if (rowIndex > 0)
                    {
                        object item = itemsSource[index];
                        int columnIndex = 0;

                        if (_documentSource.Columns != null)
                        {
                            foreach (DataGridColumn column in _documentSource.Columns)
                            {
                                if (column.Visibility == Visibility.Visible)
                                {
                                    AddTableCell(tableGrid, column, item, columnIndex, rowIndex);
                                    columnIndex++;
                                }
                            }
                        }

                        if (this.AlternatingRowBorderStyle != null && rowIndex%2 == 0)
                        {
                            Border alernatingRowBorder = new Border();

                            alernatingRowBorder.Style = this.AlternatingRowBorderStyle;
                            alernatingRowBorder.SetValue(Grid.RowProperty, rowIndex);
                            alernatingRowBorder.SetValue(Grid.ColumnSpanProperty, columnIndex);
                            alernatingRowBorder.SetValue(Grid.ZIndexProperty, -1);
                            tableGrid.Children.Add(alernatingRowBorder);
                        }
                    }

                    rowIndex++;
                }

                page = ConstructPage(tableGrid, pageNumber);
            }

            return page;
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// This function measures the heights of the page header, page footer and grid header and the first row in the grid
        /// in order to work out how manage pages might be required.
        /// </summary>
        private void MeasureElements()
        {
            double allocatedSpace = 0;

            //Measure the page header
            ContentControl pageHeader = new ContentControl();
            pageHeader.Content = CreateDocumentHeader();
            allocatedSpace = MeasureHeight(pageHeader);

            //Measure the page footer
            ContentControl pageFooter = new ContentControl();
            pageFooter.Content = CreateDocumentFooter(0);
            allocatedSpace += MeasureHeight(pageFooter);

            //Measure the table header
            ContentControl tableHeader = new ContentControl();
            tableHeader.Content = CreateTable(false);
            allocatedSpace += MeasureHeight(tableHeader);

            //Include any margins
            allocatedSpace += this.PageMargin.Bottom + this.PageMargin.Top;

            //Work out how much space we need to display the grid
            _availableHeight = this.PageSize.Height - allocatedSpace;

            //Calculate the height of the first row
            _avgRowHeight = MeasureHeight(CreateTempRow());

            //Calculate how many rows we can fit on each page
            double rowsPerPage = Math.Floor(_availableHeight/_avgRowHeight);

            if (!double.IsInfinity(rowsPerPage))
                _rowsPerPage = Convert.ToInt32(rowsPerPage);

            //Count the rows in the document source
            double rowCount = CountRows(_documentSource.ItemsSource);

            //Calculate the nuber of pages that we will need
            if (rowCount > 0)
                _pageCount = Convert.ToInt32(Math.Ceiling(rowCount/rowsPerPage));
        }

        /// <summary>
        /// This method constructs the document page (visual) to print
        /// </summary>
        private DocumentPage ConstructPage(Grid content, int pageNumber)
        {
            if (content == null)
                return null;

            //Build the page inc header and footer
            Grid pageGrid = new Grid();

            //Header row
            AddGridRow(pageGrid, GridLength.Auto);

            //Content row
            AddGridRow(pageGrid, new GridLength(1.0d, GridUnitType.Star));

            //Footer row
            AddGridRow(pageGrid, GridLength.Auto);

            ContentControl pageHeader = new ContentControl();
            pageHeader.Content = this.CreateDocumentHeader();
            pageGrid.Children.Add(pageHeader);

            if (content != null)
            {
                content.SetValue(Grid.RowProperty, 1);
                pageGrid.Children.Add(content);
            }

            ContentControl pageFooter = new ContentControl();
            pageFooter.Content = CreateDocumentFooter(pageNumber + 1);
            pageFooter.SetValue(Grid.RowProperty, 2);

            pageGrid.Children.Add(pageFooter);

            double width = this.PageSize.Width - (this.PageMargin.Left + this.PageMargin.Right);
            double height = this.PageSize.Height - (this.PageMargin.Top + this.PageMargin.Bottom);
            
            Border border = CreateBorder();
            border.Measure(new Size(this.PageSize.Width, this.PageSize.Width));
            border.Arrange(new Rect(this.PageMargin.Left, this.PageMargin.Top, width, height));
            border.UpdateLayout();

            pageGrid.Measure(new Size(width*0.98 , height*0.98));
            pageGrid.Arrange(new Rect(this.PageMargin.Left, this.PageMargin.Top, width * 0.98, height * 0.98));

            pageGrid.UpdateLayout();
             border.Child = pageGrid;
            border.UpdateLayout();

            return new DocumentPage(border);
        }

        private Border CreateBorder()
        {
            Border border = new Border();
            border.Background = new SolidColorBrush(Colors.White);
            border.BorderThickness = new Thickness(2);
            border.BorderBrush = new SolidColorBrush(Colors.Black);
            border.CornerRadius = new CornerRadius(15);
            border.Width = this.PageSize.Width - (this.PageMargin.Left + this.PageMargin.Right); 
            border.Height = this.PageSize.Height - (this.PageMargin.Top + this.PageMargin.Bottom);

            return border;
        }

        /// <summary>
        /// Creates a default header for the document; containing the doc title
        /// </summary>
        private object CreateDocumentHeader()
        {
            Border headerBorder = new Border();
            TextBlock titleText = new TextBlock();
            titleText.Style = this.DocumentHeaderTextStyle;
            titleText.TextTrimming = TextTrimming.CharacterEllipsis;
            titleText.Text = this.DocumentTitle;
            titleText.HorizontalAlignment = HorizontalAlignment.Center;

            headerBorder.Child = titleText;

            return headerBorder;
        }

        /// <summary>
        /// Creates a default page footer consisting of datetime and page number
        /// </summary>
        private object CreateDocumentFooter(int pageNumber)
        {
            Grid footerGrid = new Grid();
            footerGrid.Margin = new Thickness(0, 10, 0, 0);

            ColumnDefinition colDefinition = new ColumnDefinition();
            colDefinition.Width = new GridLength(0.5d, GridUnitType.Star);

            TextBlock dateTimeText = new TextBlock();
            dateTimeText.Style = this.DocumentFooterTextStyle;
            dateTimeText.Text = DateTime.Now.ToString("dd-MMM-yyy HH:mm");

            footerGrid.Children.Add(dateTimeText);

            TextBlock pageNumberText = new TextBlock();
            pageNumberText.Style = this.DocumentFooterTextStyle;
            pageNumberText.Text = "Page " + pageNumber.ToString() + " of " + this.PageCount.ToString();
            pageNumberText.SetValue(Grid.ColumnProperty, 1);
            pageNumberText.HorizontalAlignment = HorizontalAlignment.Right;

            footerGrid.Children.Add(pageNumberText);

            return footerGrid;
        }

        /// <summary>
        /// Counts the number of rows in the document source
        /// </summary>
        /// <param name="itemsSource"></param>
        /// <returns></returns>
        private double CountRows(IEnumerable itemsSource)
        {
            int count = 0;

            if (itemsSource != null)
            {
                foreach (object item in itemsSource)
                    count++;
            }

            return count;
        }

        /// <summary>
        /// The following function creates a temp table with a single row so that it can be measured and used to 
        /// calculate the totla number of req'd pages
        /// </summary>
        /// <returns></returns>
        private Grid CreateTempRow()
        {
            Grid tableRow = new Grid();

            if (_documentSource != null)
            {
                foreach (ColumnDefinition colDefinition in _tableColumnDefinitions)
                {
                    ColumnDefinition copy = XamlReader.Parse(XamlWriter.Save(colDefinition)) as ColumnDefinition;
                    tableRow.ColumnDefinitions.Add(copy);
                }

                foreach (object item in _documentSource.ItemsSource)
                {
                    int columnIndex = 0;
                    if (_documentSource.Columns != null)
                    {
                        foreach (DataGridColumn column in _documentSource.Columns)
                        {
                            if (column.Visibility == Visibility.Visible)
                            {
                                AddTableCell(tableRow, column, item, columnIndex, 0);
                                columnIndex++;
                            }
                        }
                    }

                    //We only want to measure teh first row
                    break;
                }
            }

            return tableRow;
        }

        /// <summary>
        /// This function counts the number of rows in the document
        /// </summary>
        private object CreateTable(bool createRowDefinitions)
        {
            if (_documentSource == null)
                return null;

            Grid table = new Grid();
            table.Style = this.GridContainerStyle;

            int columnIndex = 0;


            if (_documentSource.Columns != null)
            {
                double totalColumnWidth =
                    _documentSource.Columns.Sum(
                        column => column.Visibility == Visibility.Visible ? column.Width.Value : 0);

                foreach (DataGridColumn column in _documentSource.Columns)
                {
                    if (column.Visibility == Visibility.Visible)
                    {
                        AddTableColumn(table, totalColumnWidth, columnIndex, column);
                        columnIndex++;
                    }
                }
            }

            if (this.TableHeaderBorderStyle != null)
            {
                Border headerBackground = new Border();
                headerBackground.Style = this.TableHeaderBorderStyle;
                headerBackground.SetValue(Grid.ColumnSpanProperty, columnIndex);
                headerBackground.SetValue(Grid.ZIndexProperty, -1);

                table.Children.Add(headerBackground);
            }

            if (createRowDefinitions)
            {
                for (int i = 0; i <= _rowsPerPage; i++)
                    table.RowDefinitions.Add(new RowDefinition());
            }

            return table;

        }

        /// <summary>
        /// Measures the height of an element
        /// </summary>
        /// <param name="element"></param>
        /// <returns></returns>
        private double MeasureHeight(FrameworkElement element)
        {
            if (element == null)
                throw new ArgumentNullException("element");

            element.Measure(this.PageSize);
            return element.DesiredSize.Height;
        }

        /// <summary>
        /// Adds a column to a grid
        /// </summary>
        /// <param name="grid">Grid to add the column to</param>
        /// <param name="columnIndex">Index of the column</param>
        /// <param name="column">Source column defintition which will be used to calculate the width of the column</param>
        private void AddTableColumn(Grid grid, double totalColumnWidth, int columnIndex, DataGridColumn column)
        {
            double proportion = column.Width.Value/
                                (this.PageSize.Width - (this.PageMargin.Left + this.PageMargin.Right));

            ColumnDefinition colDefinition = new ColumnDefinition();
            colDefinition.Width = new GridLength(proportion, GridUnitType.Star);

            grid.ColumnDefinitions.Add(colDefinition);

            TextBlock text = new TextBlock();
            text.Style = this.TableHeaderTextStyle;
            text.TextTrimming = TextTrimming.CharacterEllipsis;
            if (column.Header != null) text.Text = column.Header.ToString();

            text.SetValue(Grid.ColumnProperty, columnIndex);

            grid.Children.Add(text);
            _tableColumnDefinitions.Add(colDefinition);
        }

        /// <summary>
        /// Adds a cell to a grid
        /// </summary>
        /// <param name="grid">Grid to add teh cell to</param>
        /// <param name="column">Source column definition which contains binding info</param>
        /// <param name="item">The binding source</param>
        /// <param name="columnIndex">Column index</param>
        /// <param name="rowIndex">Row index</param>
        private void AddTableCell(Grid grid, DataGridColumn column, object item, int columnIndex, int rowIndex)
        {
            if (column is DataGridTemplateColumn)
            {
                DataGridTemplateColumn templateColumn = column as DataGridTemplateColumn;
                ContentControl contentControl = new ContentControl();

                contentControl.Focusable = true;
                contentControl.ContentTemplate = templateColumn.CellTemplate;
                contentControl.Content = item;

                contentControl.SetValue(Grid.ColumnProperty, columnIndex);
                contentControl.SetValue(Grid.RowProperty, rowIndex);

                grid.Children.Add(contentControl);
            }
            else if (column is DataGridTextColumn)
            {
                DataGridTextColumn textColumn = column as DataGridTextColumn;
                TextBlock text = new TextBlock {Text = "Text"};

                text.Style = this.TableCellTextStyle;
                text.TextTrimming = TextTrimming.CharacterEllipsis;
                text.DataContext = item;

                Binding binding = textColumn.Binding as Binding;

                //if (!string.IsNullOrEmpty(column.DisplayFormat))
                //binding.StringFormat = column.DisplayFormat;

                text.SetBinding(TextBlock.TextProperty, binding);

                text.SetValue(Grid.ColumnProperty, columnIndex);
                text.SetValue(Grid.RowProperty, rowIndex);

                grid.Children.Add(text);
            }
        }

        /// <summary>
        /// Adds a row to a grid
        /// </summary>
        private void AddGridRow(Grid grid, GridLength rowHeight)
        {
            if (grid == null)
                return;

            RowDefinition rowDef = new RowDefinition();

            if (rowHeight != null)
                rowDef.Height = rowHeight;

            grid.RowDefinitions.Add(rowDef);
        }

        #endregion

    }
}
