using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Distributr.Core.Domain.Master.CostCentreEntities;
using Distributr.Core.Domain.Master.UserEntities;
using Distributr.Core.Domain.Transactional.DocumentEntities;

namespace Distributr.Core.Repository.Transactional.DocumentRepositories
{

    public class GenericDocument : Document
    {
        public GenericDocument(Guid id) : base(id)
        {
        }

        public GenericDocument(Guid id, string documentReference, CostCentre documentIssuerCostCentre, Guid documentIssuerCostCentreApplicationId, User documentIssuerUser, DateTime documentDateIssued, CostCentre documentRecipientCostCentre, DocumentStatus status) : base(id, documentReference, documentIssuerCostCentre, documentIssuerCostCentreApplicationId, documentIssuerUser, documentDateIssued, documentRecipientCostCentre, status)
        {
        }

        public override void Confirm()
        {
            throw new NotImplementedException();
        }







        protected override void _AddCreateCommandToExecute(bool isHybrid = false)
        {
            throw new NotImplementedException();
        }

        protected override void _AddAddLineItemCommandToExecute<T>(T lineItem, bool isHybrid = false)
        {
            throw new NotImplementedException();
        }


        protected override void _AddConfirmCommandToExecute(bool isHybrid = false)
        {
            throw new NotImplementedException();
        }
    }

    public interface IGenericDocumentRepository
    {
        GenericDocument GetById(Guid id);
        List<GenericDocument> GetAll();
        
    }
   
}
