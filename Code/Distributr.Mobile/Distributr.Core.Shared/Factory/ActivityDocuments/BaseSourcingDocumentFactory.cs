using System;
using System.Reflection;
using Distributr.Core.Domain.Master.CostCentreEntities;
using Distributr.Core.Domain.Master.UserEntities;
using Distributr.Core.Domain.Transactional;
using Distributr.Core.Domain.Transactional.ActivityDocumentEntities;
using Distributr.Core.Domain.Transactional.SourcingDocumentEnitities;
using Distributr.Core.Repository.Master.CostCentreRepositories;
using Distributr.Core.Repository.Master.UserRepositories;

namespace Distributr.Core.Factory.ActivityDocuments
{
    public class BaseActivityDocumentFactory
    {
        public BaseActivityDocumentFactory()
        {
            
        }

      
        protected void SetDefaultDates(ActivityDocument document)
        {
            document.DocumentDateIssued = DateTime.Now;
            document.SendDateTime = DateTime.Now;
            //document.StartDate = DateTime.Now;
            //document.EndDate = DateTime.Now;
        }


        protected T DocumentPrivateConstruct<T>(Guid id) where T : ActivityDocument
        {
            ConstructorInfo ctor = typeof(T)
                .GetConstructors(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public)[0];
            T doc = (T)ctor.Invoke(new object[] { id });
            return doc;
        }

        protected T DocumentLineItemPrivateConstruct<T>(Guid id) where T : TransactionalEntity
        {
            ConstructorInfo ctor = typeof(T)
                .GetConstructors(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public)[0];
            T doc = (T)ctor.Invoke(new object[] { id });
            return doc;
        }
        


    }
}
