using System.Windows.Controls;
using Distributr.Core.Resources.Util;
using StructureMap;

namespace Distributr.WPF.UI.Views.ReceiptDocuments
{
    public partial class ReceiptDocument : Page
    {
       private IMessageSourceAccessor _messageResolver = ObjectFactory.GetInstance<IMessageSourceAccessor>();
       
        public ReceiptDocument()
        {
            InitializeComponent();
           
            LabelControls();
        }

        void LabelControls()
        {
            PrintButton.Content = _messageResolver.GetText("sl.receipt.print");
            btnBack.Content = _messageResolver.GetText("sl.receipt.back");

        }


        
    }
}
