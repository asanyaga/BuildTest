using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using Distributr.Core.Domain.Master.ProductEntities;
using System.Web.Mvc;

namespace Distributr.HQ.Lib.ViewModels.Admin.Product
{

    public class ListProductViewModel
    {
        public ListProductViewModel()
        {
            CurrentPage = 1;
            PageSize = 20;
            Items = new List<ListProductViewModelItem>();
        }
        public string Title { get; set; }
        public Guid Brands { get; set; }
        public Guid SubBrands { get; set; }
        public Guid PackageType { get; set; }
        public Guid Packaging { get; set; }
        public Guid Returnable { get; set; }
        public string Description { get; set; }
        public decimal UnitCases { get; set; }
        public string ReturnableTypeName { get; set; }
        public string BrandsName { get; set; }
        public string SubBrandsName { get; set; }
        public string PackageTypeName { get; set; }
        public string PackagingName { get; set; }
        public string ProductTypeName { get; set; }
        public string ProductType { get; set; }
        public string ReturnableName { get; set; }
        public Guid VatClassId { get; set; }
        public string VatClass { get; set; }
        public bool isActive { get; set; }
        public Guid FlavourId { get; set; }
        public string ErrorText { get; set; }
        public Guid ProductId { get; set; }
        public string Brand { get; set; }
        public string Flavour { get; set; }
        public string RetProdFalvour { get; set; }
        public string Code { get; set; }
        public decimal ExFactoryPrice { get; set; }

      public List<ListProductViewModelItem> Items {get;set;}
      public List<ListProductViewModelItem> CurrentPageItems
      {
          get
          {
              return Items.Skip((CurrentPage - 1) * PageSize).Take(PageSize).ToList();
          }
      }

      public int PageSize { get; set; }
      public int NoPages
      {
          get
          {
              int totalpages = (int)Math.Ceiling((double)Items.Count() / (double)PageSize);
              return totalpages;
          }
      }
      public int CurrentPage { get; set; }

        public class ListProductViewModelItem
        {
            public Guid ProductId { get; set; }
            public string ProductType { get; set; }
            public string Description { get; set; }
            public string Code { get; set; }
            public string Brand { get; set; }
            public string Packaging { get; set; }
            public string Flavour { get; set; }
            public string RetProdFalvour { get; set; }
            public Guid FlavourId { get; set; }
            public string ReturnableTypeName { get; set; }
            public bool isActive { get; set; }
            public Guid VatClassId { get; set; }
            public string VatClass { get; set; }
        }
    }

    //public class ProductViewModel
    //{
    //    public int Id { get; set; }
    //    [Required(ErrorMessage = "Product Description Is Required")]
    //    public string Description { get; set; }

    //    [Required(ErrorMessage = "Product brand Is Required")]
    //    public ProductBrand Brand { get; set; }

    //    [Required(ErrorMessage = "Product Packaging Is Required")]
    //    public ProductPackaging Packaging { get; set; }

    //    [Required(ErrorMessage = "Product Packaging Type Is Required")]
    //    public ProductPackagingType PackagingType { get; set; }

    //    [Required(ErrorMessage = "Product Code Is Required")]
    //    public string ProductCode { get; set; }

    //    public List<ProductPricing> ProductPricings { get; internal set; }

    //}

    public abstract class EditProductViewModelBase
    {
        public string Title { get; set; }
        public Guid Id { get; set; }

        [RegularExpression(@"[A-Za-z0-9'.\s\-]+$",ErrorMessage="Special Characters not allowed")]
        [Required(ErrorMessage = "Product Description is required")]
        [StringLength (200)]
        public string Description { get; set; }
        [Required(ErrorMessage = "Product Brand is required")]
        public Guid BrandId { get; set; }
        [Required(ErrorMessage = "Product Packaging is required")]
        public Guid PackagingId { get; set; }
        [Required(ErrorMessage = "Product Packaging Type is required")]
        public Guid PackagingTypeId { get; set; }


        [RegularExpression("[A-Za-z0-9]+$", ErrorMessage = "Special Characters not allowed")]
        [Required(ErrorMessage = "Product Code is required")]
        public string ProductCode { get; set; }
        [Required(ErrorMessage="Returnable Type is Required!")]
        public int ReturnableType { get; set; }
        public string RetunableTypeName { get; set; }
        public Guid FlavourID { get; set; }
        [Required(ErrorMessage = "Vat Class is required")]
        public Guid VatClassId { get; set; }
        public int Capacity { get; set; }
        public Guid? ReturnableProductId { get; set; }
        public SelectList ReturnableProduct { get; set; }
        public SelectList VatClass { get; set; }
        public SelectList SubBrand { get; set; }
        public decimal ExFactoryPrice { get; set; }
 
    }

    public class EditSaleProductViewModelIn : EditProductViewModelBase
    {
        public Guid FlavourID { get; set; }
        public Guid ProductTypeID { get; set; }
    }

    public class EditSaleProductViewModelOut : EditProductViewModelBase
    {
        public SelectList ProductBrands { get; set; }
        public SelectList ProductPackagings { get; set; }
        public SelectList ProductPackagingTypes { get; set; }

        public Guid ProductTypeID { get; set; }
        public Guid FlavourID { get; set; }
        public Guid? ReturnableProductId { get; set; }
        
        public SelectList Flavours { get; set; }
        public SelectList ProductTypes { get; set; }
        public SelectList Returnable { get; set; }
        public SelectList ReturnableProduct { get; set; }
        public SelectList VatClass { get; set; }
        //public SelectList ReturnableTypeList { get; set; }
    }

    public class EditReturnableProductViewModelIn : EditProductViewModelBase
    {
        //[Required(ErrorMessage = "Product brand is required")]
        //public int Brands { get; set; }
        //[Required(ErrorMessage = "Product package type is required")]
        //public int PackageType { get; set; }
        //[Required(ErrorMessage = "Product packaging is required")]
        //public int Packaging { get; set; }
        //Import Properties//
        public string brandCode { get; set; }
        public string subBrandCode { get; set; }
        public string packCode { get; set; }
        public string packTypeCode { get; set; }
        public string productTypeCode { get; set; }
        public string vatClass { get; set; }
        
    }

    public class EditReturnableProductViewModelOut : EditProductViewModelBase
    {
        public SelectList ProductBrands { get; set; }
        public SelectList ProductPackagings { get; set; }
        public SelectList ProductPackagingTypes { get; set; }
        public SelectList ProductReturnableTypes { get; set; }
        public SelectList ReturnableTypes { get; set; }
        [Required(ErrorMessage = "Product brand is required")]
        public Guid Brands { get; set; }
        [Required(ErrorMessage = "Product package type is required")]
        public Guid PackageType { get; set; }
        [Required(ErrorMessage = "Product packaging is required")]
        public Guid Packaging { get; set; }
        [Required(ErrorMessage = "Vat is required")]
        public Guid VatClassId { get; set; }
        //[Required(ErrorMessage="ReturnableType is Required!")]
        //public int ReturnableType{ get; set; }
    }

    public class EditConsolidatedProductIn : EditProductViewModelBase
    {
        public Guid FlavourID { get; set; }
        public Guid ProductTypeID { get; set; }
         [Required(ErrorMessage = "Quantity is required")]
         [Range(0.1, double.MaxValue, ErrorMessage = "Quantity must be greater than 0 (Zero) ")]
        public int Quantity { get; set; }
       // public List<ConsolidatedProduct.ProductDetail> ProductDetails { get; set; }

    }

    public class EditConsolidatedProductOut : EditProductViewModelBase
    {
         [Required(ErrorMessage = "Quantity is required")]
         [Range(0.1, double.MaxValue, ErrorMessage = "Quantity must be greater than 0 (Zero) ")]
        public int Quantity { get; set; }
         public Guid ProductID { get; set; }
         public decimal ExFactoryPrice { get; set; }

        public SelectList ProductBrands { get; set; }
        public SelectList ProductPackagings { get; set; }
        public SelectList ProductPackagingTypes { get; set; }
        public SelectList ProductList { get; set; }
        public List<ProductDetailViewModel> ProductDetails { get; set; }

        public class ProductDetailViewModel
        {
            public Guid ProductId { get; set; }
            public string ProductCode { get; set; }
            public string PackagingType { get; set; }
            public string Packaging { get; set; }
            public string Brand { get; set; }
            public string Descritpion { get; set; }
             [Required(ErrorMessage = "Quantity is required")]
             [Range(0.1, double.MaxValue, ErrorMessage = "Quantity must be greater than 0 (Zero) ")]
            public int Qty { get; set; }
        }
        //Imports for consolidated products (Properties)
        public string brandCode { get; set; }
        public string packagingCode { get; set; }
        public string packagingTypeCode { get; set; }
    }

    public class SaleProductViewModel
    {
        public Guid Id { get; set; }
        public string Title { get; set; }

        [Required(ErrorMessage = "Product brand is required")]
        public Guid Brands { get; set; }

        [Required(ErrorMessage = "Product package type is required")]
        public Guid PackageType { get; set; }

        [Required(ErrorMessage = "Product packaging is required")]
        public Guid Packaging { get; set; }

        [Required(ErrorMessage = "Product type is required")]
        public Guid ProductType { get; set; }

        public Guid Returnable { get; set; }

        [RegularExpression(@"[A-Za-z0-9'.\s]+$", ErrorMessage = "Special Characters not allowed")]
        [Required(ErrorMessage = "Product description is required")]
        public string Description { get; set; }

        public decimal UnitCases { get; set; }
        public bool isActive { get; set; }

        [Required(ErrorMessage = "Product Code is required")]
        // [StringLength(15)]
        //<([A-Z][A-Z0-9]*)\b[^>]*>.*?</\1>

       // [RegularExpression("[A-Za-z0-9]+$", ErrorMessage = "Special Characters not allowed")]
        [StringLength(49)]
        public string ProductCode { get; set; }

        [Required(ErrorMessage = "Sub Product  is required")]
        public Guid FlavourID { get; set; }

        [Required(ErrorMessage = " Product Category is required")]
        public Guid ProductTypeID { get; set; }

        [Required(ErrorMessage = "Vat is required")]
        public Guid VatClassId { get; set; }

        public Guid? ReturnableProductId { get; set; }

        //Import Properties//
        public string brandCode { get; set; }
        public string subBrandCode { get; set; }
        public string packCode { get; set; }
        public string packTypeCode { get; set; }
        public string productTypeCode { get; set; }
        public string vatClass { get; set; }
        [RegularExpression(@"^\d+(\.\d{1,4})?$", ErrorMessage = "Please enter a numeric value with up to Four decimal places.")]
        public decimal ExFactoryPrice { get; set; }
    }

    public class ReturnableProductViewModel
    {
        [Required(ErrorMessage = "Product brand is required")]
        public Guid Brands { get; set; }
        [Required(ErrorMessage = "Product package type is required")]
        public Guid PackagingTypeId { get; set; }
        [Required(ErrorMessage = "Product packaging is required")]
        public Guid PackagingId { get; set; }
        [Required(ErrorMessage = "Vat is required")]
        public Guid VatClassId { get; set; }
        [Required(ErrorMessage = "Product Code is required")]
        [RegularExpression("[A-Za-z0-9]+$", ErrorMessage = "Special Characters not allowed")]
        public string ProductCode { get; set; }
        [Required(ErrorMessage = "Product Description is required")]
        [StringLength(200)]
        public string Description { get; set; }
        public int Capacity { get; set; }
        public Guid? ReturnableProductId { get; set; }
        public int ReturnableType { get; set; }
        public Guid BrandId { get; set; }
        [Required(ErrorMessage = "Sub brand is required")]
        public Guid FlavourID { get; set; }
        public Guid Id { get; set; }
        public decimal ExFactoryPrice { get; set; }
    }
}
