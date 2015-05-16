using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using Distributr.Core.Domain.Master.OutletVisitReasonsTypeEntities;
using Distributr.HQ.Lib.Helper;
using MvcContrib.Pagination;

namespace Distributr.HQ.Lib.ViewModels.Admin.OutletVisitReasonsTypeViewModels
{
    public class OutletVisitReasonsTypeViewModel
    {
        public Guid id { get; set; }

        [LocalizedRequired(ErrorMessage = "Name field is Required")]
        [RegularExpression(@"^[a-zA-Z0-9'.\s]{1,40}$", ErrorMessage = "Special characters are not allowed")]
        public string Name { get; set; }

        [LocalizedRequired(ErrorMessage = "Description field is Required")]
        public string Description { get; set; }
        [LocalizedRequired(ErrorMessage = "outletVisitAction field is Required")]
        public OutletVisitAction outletVisitAction { get; set; }

    [DisplayFormat(DataFormatString = "{0:dd MMM yyyy}")]
        public DateTime _DateCreated { get; set; }
       [DisplayFormat(DataFormatString = "{0:dd MMM yyyy}")]
        public DateTime _DateLastUpdated { get; set; }
       
        public bool isActive { get; set; }
        public IPagination<OutletVisitReasonsTypeViewModel> outletVisitReasonsTypesPagedList { get; set; }

    }
}
