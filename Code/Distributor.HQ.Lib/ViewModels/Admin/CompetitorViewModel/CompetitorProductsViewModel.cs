using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;

namespace Distributr.HQ.Lib.ViewModels.Admin.CompetitorViewModel
{
  public  class CompetitorProductsViewModel
    {
      public Guid Id { get; set; }
      [Required(ErrorMessage="Product Name is required")]
      public string ProductName { get; set; }
      [Required(ErrorMessage = "Product Description is required")]
      public string ProductDescription { get; set; }
      [Required(ErrorMessage = "Select Competitor")]
      public Guid Competitor { get; set; }
      [Required(ErrorMessage = "Select Brand")]
      public Guid Brand { get; set; }
      [Required(ErrorMessage = "Select Flavour")]
      public Guid Flavour { get; set; }
      [Required(ErrorMessage = "Select Packaging Type")]
      public Guid PackagingType { get; set; }
      [Required(ErrorMessage = "Select Packaging")]
      public Guid Packaging { get; set; }
      [Required(ErrorMessage = "Select Product Type")]
      public Guid Type { get; set; }
      public bool isActive { get; set; }
      //Names
      public string BrandName { get; set; }
      public string FlavourName { get; set; }
      public string PackagingTypeName { get; set; }
      public string PackagingName { get; set; }
      public string TypeName { get; set; }
      public string CompetitorName { get; set; }
    }
}
