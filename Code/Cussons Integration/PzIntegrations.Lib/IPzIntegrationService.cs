using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using PzIntegrations.Lib.MasterDataImports;

namespace PzIntegrations.Lib
{
    public interface IPzIntegrationService
    {
        void ImportMasterData(string[] masterdata,bool isAll=false);
        void Start();
        void Stop();
        List<string> GetImportErrors();
        bool IsTaskCompleted();
        void FindAndExportOrder(string externalRef); 
    }

    public interface IOrderExportService
    {
        Task GetAndExportOrders(DateTime startDate, DateTime endDate);
        Task GetAndExportOrders(string externalRef);
        Task<int> GetAndExportOrders();


    }


}
