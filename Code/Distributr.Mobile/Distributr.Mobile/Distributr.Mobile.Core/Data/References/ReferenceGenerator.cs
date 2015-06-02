using System;
using Distributr.Core.Domain.Master.CostCentreEntities;
using Distributr.Core.Domain.Master.UserEntities;

namespace Distributr.Mobile.Core.Data.References
{
    public class ReferenceGenerator
    {
        private readonly Outlet outlet;
        private readonly DateTime now;
        private readonly long sequenceNumber;
        private readonly User user;

        public ReferenceGenerator(long sequenceNumber, User user, Outlet outlet, DateTime now)
        {
            this.sequenceNumber = sequenceNumber;
            this.outlet = outlet;
            this.user = user;
            this.now = now;
        }

        public string NextOrderReference()
        {
            //e.g  "O_john_O001_20150406_114353_00024"
            return CreateReference("O");
        }

        public string NextSaleReference()
        {
            //e.g  "S_john_O001_20150406_114353_00024"
            return CreateReference("S");
        }

        public string NextInvoiceReference()
        {
            //e.g "I_john_O001_20150406_114435_00025",
            return CreateReference("I");
        }

        public string NextReceiptReference()
        {
            //e.g "R_john_O001_20150406_114435_00025",
            return CreateReference("R");
        }

        public string NextExternalDocumentReference()
        {
            return string.Format("{0}{1}", user.Code, sequenceNumber.ToString().PadLeft(6, '0'));
        }

        private string CreateReference(string prefix)
        {
            return string.Format("{0}_{1}_{2}_{3}_{4}_{5}",
                prefix,
                user.Username,
                outlet.CostCentreCode,
                now.ToString("yyyyMMdd"),
                now.ToString("HHmmss"),
                sequenceNumber.ToString().PadLeft(5, '0'));            
        }
    }
}
