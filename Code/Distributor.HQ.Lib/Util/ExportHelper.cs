using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Reporting.WebForms;

namespace Distributr.HQ.Lib.Util
{
  public static  class ExportHelper
    {
      public static byte[] ExportPdf(LocalReport localReport, out string mimeType)
      {
          string reportType = "PDF";

          string fileNameExtension;
          string encoding;




          //The DeviceInfo settings should be changed based on the reportType

          //http://msdn2.microsoft.com/en-us/library/ms155397.aspx

          string deviceInfo =
              "<DeviceInfo>" +
              "  <OutputFormat>PDF</OutputFormat>" +
              "  <PageWidth>8.5in</PageWidth>" +
              "  <PageHeight>11in</PageHeight>" +
              //"  <PageWidth>11in</PageWidth>" +
              //"  <PageHeight>8.5in</PageHeight>" +
              "  <MarginTop>0.5in</MarginTop>" +
              "  <MarginLeft>1in</MarginLeft>" +
              "  <MarginRight>1in</MarginRight>" +
              "  <MarginBottom>0.5in</MarginBottom>" +
              "</DeviceInfo>";


          Warning[] warnings;

          string[] streams;

          byte[] renderedBytes;


          //Render the report

          renderedBytes = localReport.Render(
              reportType,
              deviceInfo,
              out mimeType,
              out encoding,
              out fileNameExtension,
              out streams,
              out warnings);
          return renderedBytes;
      }

      public static byte[] ExportExcel(LocalReport localReport, out string mimeType)
      {
          string reportType = "EXCEL";

          string fileNameExtension;
          string encoding;




          //The DeviceInfo settings should be changed based on the reportType

          //http://msdn2.microsoft.com/en-us/library/ms155397.aspx

         


          Warning[] warnings;

          string[] streams;

          byte[] renderedBytes;


          //Render the report

          renderedBytes = localReport.Render(
              reportType,
              null,
              out mimeType,
              out encoding,
              out fileNameExtension,
              out streams,
              out warnings);
          return renderedBytes;
      }
    }
}
