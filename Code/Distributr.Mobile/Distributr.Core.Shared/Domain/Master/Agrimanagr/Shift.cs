using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Distributr.Core.Domain.Master.Agrimanagr
{
    public class Shift:MasterEntity
    {
        public Shift(Guid id) : base(id)
        {
        }

        public Shift(Guid id, DateTime dateCreated, DateTime dateLastUpdated, EntityStatus status) : base(id, dateCreated, dateLastUpdated, status)
        {
        }

        [StringLength(50), Required(ErrorMessage = "Code is a Required Field")]
        public string Code { get; set; }

        [StringLength(50), Required(ErrorMessage = "Name is a Required Field")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Start Time is a Required Field")]
        public DateTime StartTime { get; set; }

        [Required(ErrorMessage = "End Time is a Required Field")]
        public DateTime EndTime { get; set; }

        [StringLength(450)]
        public string Description { get; set; }

    }
}
