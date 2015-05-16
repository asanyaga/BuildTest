using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Distributr.Core.Domain.Transactional.DocumentEntities
{
    public class SalesInvoice : Document
    {
        public SalesInvoice(Guid id) : base(id)
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
}
