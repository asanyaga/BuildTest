using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Distributr.Core.Domain.Master.BankEntities;

namespace Distributr.Core.Domain.Master.Agrimanagr
{
#if !SILVERLIGHT
    [Serializable]
#endif
    public class ServiceProvider : MasterEntity
    {
        public ServiceProvider(Guid id) : base(id)
        {
        }

        public ServiceProvider(Guid id, DateTime dateCreated, DateTime dateLastUpdated, EntityStatus isActive)
            : base(id, dateCreated, dateLastUpdated, isActive)
        {
        }

        [StringLength(50)]
        public string Code { get; set; }


        [StringLength(250), Required(ErrorMessage = "Name is a Required Field")]
        public string Name { get; set; }

        [StringLength(50), Required(ErrorMessage = "ID Number is a Required Field!")]
        public string IdNo { get; set; }

        [StringLength(50), Required(ErrorMessage = "PIN number is a Required Field!")]
        public string PinNo { get; set; }

        [Required(ErrorMessage = "Phone number is a Required Field!")]
        [RegularExpression(@"((\(\d{3,4}\)|\d{3,4}-)\d{4,9}(-\d{1,5}|\d{0}))|(\d{4,12})", ErrorMessage = "Entered phone format is not valid.")]
        [StringLength(25, MinimumLength = 1, ErrorMessage = "Mobile number too long.")]
        public string MobileNumber { get; set; }

         [StringLength(250)]
        public string AccountName { get; set; }
         [StringLength(250)]
        public string AccountNumber { get; set; }

        [StringLength(450)]
        public string Description { get; set; }

        public Bank Bank { get; set; }
        public BankBranch BankBranch { get; set; }

         public Gender Gender { get; set; }

    }
}
