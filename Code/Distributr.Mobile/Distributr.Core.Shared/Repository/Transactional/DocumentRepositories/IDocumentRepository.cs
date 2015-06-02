using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Distributr.Core.Domain.Transactional.DocumentEntities;

namespace Distributr.Core.Repository.Transactional.DocumentRepositories
{
    public interface IDocumentRepository<T> 
    {

        /// <summary>
        /// RULE Can only make pending document inactive
        /// </summary>
        /// <param name="documentId"></param>
        [Obsolete("To be retired")]
        void CancelDocument(Guid documentId);

        //void Save(T entity);
        T GetById(Guid Id);
        List<T> GetAll();
        List<T> GetAll(DateTime startDate, DateTime endDate);
        int GetCount();
       
    }
    [Obsolete("Extract save into this contract so can gradually be removed")]
    public interface IDocumentRepositorySaveable<T>
    {
        void Save(T entity);
    }

}
