using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Distributr.Core.Domain.Master.CostCentreEntities;
using Distributr.Core.Domain.Master.UserEntities;
using Distributr.Core.Domain.Transactional.DocumentEntities;
using Distributr.Core.Domain.Transactional.DocumentEntities.LineItems;
using Distributr.Core.Repository.Master.CostCentreRepositories;
using Distributr.Core.Repository.Master.UserRepositories;

namespace Distributr.Core.Factory.Documents
{
    public class BaseDocumentFactory
    {
        private ICostCentreRepository _costCentreRepository;
        private IUserRepository _userRepository;
        private ICostCentreApplicationRepository _costCentreApplicationRepository;

        public BaseDocumentFactory(ICostCentreRepository costCentreRepository, IUserRepository userRepository)
        {
            _costCentreRepository = costCentreRepository;
            _userRepository = userRepository;
        }

        protected void Map(Document document, CostCentre documentIssuerCostCentre,  Guid documentIssuerCostCentreApplicationId,
           CostCentre documentRecipientCostCentre,User documentIssuerUser,
           string DocumentReference, double? longitude, double? latitude)
        {
            document.DocumentIssuerCostCentreApplicationId = documentIssuerCostCentreApplicationId;
            document.DocumentIssuerCostCentre = documentIssuerCostCentre;
            document.DocumentRecipientCostCentre = documentRecipientCostCentre;
            document.DocumentIssuerUser = documentIssuerUser;
            document.DocumentReference = DocumentReference;
            document.Status = DocumentStatus.New;
            document.DocumentDateIssued = DateTime.Now;
            document.Longitude = longitude;
            document.Latitude = latitude;
        }

        protected void SetDefaultDates(Document document)
        {
            document.DocumentDateIssued = DateTime.Now;
            document.SendDateTime = DateTime.Now;
            document.StartDate = DateTime.Now;
            document.EndDate = DateTime.Now;
        }


        protected T DocumentPrivateConstruct<T>(Guid id) where T : Document
        {
            ConstructorInfo ctor = typeof(T)
                .GetConstructors(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public)[0];
            T doc = (T)ctor.Invoke(new object[] { id });
            return doc;
        }

        protected T DocumentLineItemPrivateConstruct<T>(Guid id) where T : ProductLineItem
        {
            ConstructorInfo ctor = typeof(T)
                .GetConstructors(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public)[0];
            T doc = (T)ctor.Invoke(new object[] { id });
            return doc;
        }
        protected T ReceiptLineItemPrivateConstruct<T>(Guid id) where T : DocumentLineItem
        {
            ConstructorInfo ctor = typeof(T)
                .GetConstructors(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public)[0];
            T doc = (T)ctor.Invoke(new object[] { id });
            return doc;
        }


    }
}
