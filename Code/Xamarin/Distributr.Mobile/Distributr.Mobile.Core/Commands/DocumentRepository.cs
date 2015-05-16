using System;
using System.Collections.Generic;
using System.Text;
using Distributr.Core.Repository.Transactional.DocumentRepositories;

namespace Distributr.Mobile.Core.Documents
{
    public class DocumentRepository<T> :IDocumentRepository<T>
    {
        public void CancelDocument(Guid documentId)
        {
            throw new NotImplementedException();
        }

        public T GetById(Guid Id)
        {
            throw new NotImplementedException();
        }

        public List<T> GetAll()
        {
            throw new NotImplementedException();
        }

        public List<T> GetAll(DateTime startDate, DateTime endDate)
        {
            throw new NotImplementedException();
        }

        public int GetCount()
        {
            throw new NotImplementedException();
        }
    }
}
