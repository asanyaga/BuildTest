using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Distributr.Core.Domain.Transactional.DocumentEntities;
using Distributr.WPF.Lib.Reporting;
using Distributr.WPF.Lib.Services.DocumentReports;
using Distributr.WPF.Lib.UI.Pages;
using Distributr.WPF.Lib.ViewModels.PrintableDocuments;
using Microsoft.Reporting.WinForms;
using StructureMap;

namespace Distributr.WPF.UI.Views.DocumentReports
{
    public partial class DocumentReportViewer :Window, IPrintableDocumentViewer
    {
        private DocumentReportViewerViewModel _vm;
        public DocumentReportViewer()
        {
            InitializeComponent();
            _vm = DataContext as DocumentReportViewerViewModel;
        }

        private void ReportViewer_Load(object sender, EventArgs e)
        {
            _vm.ReportViewer_LoadCommand.Execute(sender as ReportViewer);
        }

        public void ViewDocument(Guid documentId, DocumentType docType)
        {
            _vm.DocumentId = documentId;
            _vm.DocumentType = docType;
            _vm.SetUp();
            this.Show();
        }
    }
}
