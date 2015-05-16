using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace Distributr.WPF.Lib.ViewModels.Printerutilis
{
    public  class UiPrintHelper
    {
        private Int32 VerticalOffset;
        private Int32 HorizontalOffset { get; set; }
        private String Title { get; set; }
        private UIElement Content { get; set; }

        public UiPrintHelper(int verticalOffset, int horizontalOffset, string title, UIElement content)
        {
            VerticalOffset = verticalOffset;
            HorizontalOffset = horizontalOffset;
            Title = title;
            Content = content;
        }

        public Int32 Print()
        {
            var dlg = new PrintDialog();

            if (dlg.ShowDialog() == true)
            {
                //---FIRST PAGE---//
                // Size the Grid.
                Content.Measure(new Size(Double.PositiveInfinity,
                                         Double.PositiveInfinity));

                Size sizeGrid = Content.DesiredSize;

                //check the width
                if (sizeGrid.Width > dlg.PrintableAreaWidth)
                {
                    MessageBoxResult result = MessageBox.Show("Some Items are out of printable area", "Print",
                                                              MessageBoxButton.YesNo, MessageBoxImage.Question);
                    if (result == MessageBoxResult.No)
                        throw new PrintDialogException("print aborted");
                }

                // Position of the grid 
                var ptGrid = new Point(HorizontalOffset, VerticalOffset);

                // Layout of the grid
                Content.Arrange(new Rect(ptGrid, sizeGrid));

                //print
                dlg.PrintVisual(Content, Title);

                //---MULTIPLE PAGES---//

                //---MULTIPLE PAGES---//
                double diff;
                int i = 1;
                while ((diff = sizeGrid.Height - (dlg.PrintableAreaHeight - VerticalOffset*i)*i) > 0)
                {
                    //Position of the grid 
                    var ptSecondGrid = new Point(HorizontalOffset, -sizeGrid.Height + diff + VerticalOffset);

                    // Layout of the grid
                    Content.Arrange(new Rect(ptSecondGrid, sizeGrid));

                    //print
                    int k = i + 1;
                    dlg.PrintVisual(Content, Title + " (Page " + k + ")");

                    i++;
                }

                return i;
            }
            throw new PrintDialogException("print aborted");
        }
    }
}
