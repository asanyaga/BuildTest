using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using Distributr.Core.Domain.Master;
using Distributr.Core.Domain.Master.CentreEntity;
using Distributr.Core.Domain.Master.CostCentreEntities;

namespace Distributr.HQ.Lib.ViewModels.Agrimanagr.CentreViewModels
{
    public class CentreViewModel
    {
        public CentreViewModel()
        {
            SelectedHubId = Guid.Empty;
            SelectedRouteId = Guid.Empty;
        }

        public Guid Id { get; set; }

        [StringLength(15, MinimumLength = 1, ErrorMessage = "Code length should not exceed 15 charachers.")]
        public string Code { get; set; }

        [Required(ErrorMessage = "Name is Required")]
        public string Name { get; set; }

        public string Description { get; set; }

        [Required(ErrorMessage = "Centre Type is Required")]
        public Guid CenterTypeId { get; set; }

        [Display(Name = "Centre Type")]
        public string CentreTypeName { get; set; }

        public Guid SelectedRouteId { get; set; }

        [Required(ErrorMessage = "Route is reguired")]
        [Display(Name = "Route")]
        public string SelectedRouteName { get; set; }

        [Required(ErrorMessage = "Hub is reguired")]
        public Guid SelectedHubId { get; set; }

        [Display(Name = "Hub")]
        public string SelectedHubName { get; set; }

        public bool IsActive { get; set; }

        public SelectList RouteList { get; set; }

        public SelectList HubList { get; set; }

        public SelectList CentreTypesList { get; set; }

    }

}
