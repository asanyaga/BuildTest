using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Reporting.WinForms;


namespace Distributr.WPF.Lib.UI.Pages
{
   public interface ICustomReportViewer
   {
       void ShowReportView(ReportViewer viewer);
   }

   
}
