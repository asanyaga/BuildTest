using System;
using System.Reflection;
using Distributr.Core.Domain.Master.CostCentreEntities;
using Distributr.Core.Domain.Master.UserEntities;
using Distributr.Core.Domain.Transactional;
using Distributr.Core.Domain.Transactional.DocumentEntities;
using Distributr.Core.Domain.Transactional.DocumentEntities.LineItems;
using Distributr.Core.Domain.Transactional.SourcingDocumentEnitities;
using Distributr.Core.Domain.Transactional.SourcingDocumentEnitities.LineItems;
using Distributr.Core.Repository.Master.CostCentreRepositories;
using Distributr.Core.Repository.Master.UserRepositories;

namespace Distributr.Core.Factory.SourcingDocuments
{
    public class BaseSourcingDocumentFactory
    {
        private ICostCentreRepository _costCentreRepository;
        private IUserRepository _userRepository;
        private ICostCentreApplicationRepository _costCentreApplicationRepository;

        public BaseSourcingDocumentFactory(ICostCentreRepository costCentreRepository, IUserRepository userRepository)
        {
            _costCentreRepository = costCentreRepository;
            _userRepository = userRepository;
        }

        protected void Map(SourcingDocument document, CostCentre documentIssuerCostCentre, Guid documentIssuerCostCentreApplicationId,
           CostCentre documentRecipientCostCentre,User documentIssuerUser,
           string DocumentReference, double? longitude, double? latitude)
        {
            document.DocumentIssuerCostCentreApplicationId = documentIssuerCostCentreApplicationId;
            document.DocumentIssuerCostCentre = documentIssuerCostCentre;
            document.DocumentRecipientCostCentre = documentRecipientCostCentre;
            document.DocumentIssuerUser = documentIssuerUser;
            document.DocumentReference = DocumentReference;
            document.Status = DocumentSourcingStatus.New;
            document.DocumentDateIssued = DateTime.Now;
            //document.Longitude = longitude;
            //document.Latitude = latitude;
        }

        protected void SetDefaultDates(SourcingDocument document)
        {
            document.DocumentDateIssued = DateTime.Now;
            document.SendDateTime = DateTime.Now;
            //document.StartDate = DateTime.Now;
            //document.EndDate = DateTime.Now;
        }


        protected T DocumentPrivateConstruct<T>(Guid id) where T : SourcingDocument
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
