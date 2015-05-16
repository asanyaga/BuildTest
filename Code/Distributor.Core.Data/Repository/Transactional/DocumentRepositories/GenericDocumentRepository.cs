using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Distributr.Core.Data.EF;
using Distributr.Core.Data.Repository.DocumentRepositories;
using Distributr.Core.Repository.Master.CostCentreRepositories;
using Distributr.Core.Repository.Master.ProductRepositories;
using Distributr.Core.Repository.Master.UserRepositories;
using Distributr.Core.Repository.Transactional.DocumentRepositories;

namespace Distributr.Core.Data.Repository.Transactional.DocumentRepositories
{
   public class GenericDocumentRepository : DocumentRepository, IGenericDocumentRepository
    {
       public GenericDocumentRepository(CokeDataContext ctx, ICostCentreRepository costCentreRepository, IUserRepository userRepository, IProductRepository productRepository) : base(ctx, costCentreRepository, userRepository, productRepository)
       {
       }
       public GenericDocument GetById(Guid id)
        {
            var tblDoc = _GetById(id);
            if (tblDoc == null) return null;
           return Map(tblDoc);
        }

       private GenericDocument Map(tblDocument tblDoc)
       {
           var doc = new GenericDocument(tblDoc.Id);
           _Map(tblDoc, doc);
           return doc;
       }

       public List<GenericDocument> GetAll()
       {
           return _ctx.tblDocument.Select(Map).ToList();
       }
    }
}
