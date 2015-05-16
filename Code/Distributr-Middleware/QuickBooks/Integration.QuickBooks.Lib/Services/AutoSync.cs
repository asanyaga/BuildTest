using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;
using Distributr.Core.Repository.Transactional.DocumentRepositories.IIntegrationDocumentRepository;
using GalaSoft.MvvmLight.Messaging;
using Integration.QuickBooks.Lib.Services.Impl;

namespace Integration.QuickBooks.Lib.Services
{
   public  class AutoSync
    {
       
       private readonly ITransactionsDownloadService _transactionsDownloadService;
       private IEnumerable<QuickBooksOrderDocumentDto> _data;

       public AutoSync()
       {
           _transactionsDownloadService=new TransactionsDownloadService();
           _data=new List<QuickBooksOrderDocumentDto>();
         
       }

       
    }
}
