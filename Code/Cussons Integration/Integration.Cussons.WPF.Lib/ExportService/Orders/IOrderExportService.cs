using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Distributr.Core.Domain.Master.Util;
using Distributr.Core.Repository.Transactional.DocumentRepositories.IIntegrationDocumentRepository;

namespace Integration.Cussons.WPF.Lib.ExportService.Orders
{
    public interface IOrderExportService
    {
        Task GetAndExportOrders(DateTime startDate, DateTime endDate);
        Task GetAndExportOrders(string externalRef);
        Task<int> GetAndExportOrders();
        
        
    }
}
