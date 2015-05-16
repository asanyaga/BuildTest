using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Distributr.Core.Domain.Transactional.SourcingDocumentEnitities;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;

namespace Distributr.WPF.Lib.ViewModels.Transactional.CommodityReception
{
   public class CommodityDocumentListViewModelBase:DistributrViewModelBase
    {
       public CommodityDocumentListViewModelBase()
       {
           SetUpCommand = new RelayCommand(SetUp);
       }

       public virtual void SetUp()
       {
       }

       public RelayCommand SetUpCommand { get; set; }


       public const string IsSelectAllCheckedPropertyName = "IsSelectAllChecked";
       private bool _isSelectAllChecked;
       public bool IsSelectAllChecked
       {
           get
           {
               return _isSelectAllChecked;
           }

           set
           {
               if (_isSelectAllChecked == value)
               {
                   return;
               }

               RaisePropertyChanging(IsSelectAllCheckedPropertyName);
               _isSelectAllChecked = value;
               RaisePropertyChanged(IsSelectAllCheckedPropertyName);

           }
       }

       public const string NameSrchParamPropertyName = "NameSrchParam";
       private string _nameSrchParam = "";
       public string NameSrchParam
       {
           get
           {
               return _nameSrchParam;
           }

           set
           {
               if (_nameSrchParam == value)
               {
                   return;
               }

               RaisePropertyChanging(NameSrchParamPropertyName);
               _nameSrchParam = value;
               RaisePropertyChanged(NameSrchParamPropertyName);
           }
       }
    }
   public class CommodityListingItemBase :ViewModelBase
   {
       public CommodityListingItemBase()
       {
       }


      
       
       public const string DocumentIdPropertyName = "DocumentId";
       private Guid _documentId = Guid.Empty;
       public Guid DocumentId
       {
           get
           {
               return _documentId;
           }

           set
           {
               if (_documentId == value)
               {
                   return;
               }

               RaisePropertyChanging(DocumentIdPropertyName);
               _documentId = value;
               RaisePropertyChanged(DocumentIdPropertyName);
           }
       }
      
       public const string DocumentReferencePropertyName = "DocumentReference";
       private string _docRefNo = "";
       public string DocumentReference
       {
           get
           {
               return _docRefNo;
           }

           set
           {
               if (_docRefNo == value)
               {
                   return;
               }

               RaisePropertyChanging(DocumentReferencePropertyName);
               _docRefNo = value;
               RaisePropertyChanged(DocumentReferencePropertyName);
           }
       }
      
       
       public const string DateIssuedPropertyName = "DateIssued";
       private DateTime _dateIssued = DateTime.Now;
       public DateTime DateIssued
       {
           get
           {
               return _dateIssued;
           }

           set
           {
               if (_dateIssued == value)
               {
                   return;
               }

               RaisePropertyChanging(DateIssuedPropertyName);
               _dateIssued = value;
               RaisePropertyChanged(DateIssuedPropertyName);
           }
       }


       public const string NetWeightPropertyName = "NetWeight";
       private decimal _netWeight =0m;
       public decimal NetWeight
       {
           get
           {
               return _netWeight;
           }

           set
           {
               if (_netWeight == value)
               {
                   return;
               }

               RaisePropertyChanging(NetWeightPropertyName);
               _netWeight = value;
               RaisePropertyChanged(ClerkNamePropertyName);
           }
       }
      
       public const string ClerkNamePropertyName = "ClerkName";
       private string _clerkName = "";
       public string ClerkName
       {
           get
           {
               return _clerkName;
           }

           set
           {
               if (_clerkName == value)
               {
                   return;
               }

               RaisePropertyChanging(ClerkNamePropertyName);
               _clerkName = value;
               RaisePropertyChanged(ClerkNamePropertyName);
           }
       }

      
       public const string StatusPropertyName = "Status";
       private DocumentSourcingStatus _status;
       public DocumentSourcingStatus Status
       {
           get
           {
               return _status;
           }

           set
           {
               if (_status == value)
               {
                   return;
               }

               RaisePropertyChanging(StatusPropertyName);
               _status = value;
               RaisePropertyChanged(StatusPropertyName);
           }
       }

      
       public const string DescriptionPropertyName = "Description";
       private string _description = "";
       public string Description
       {
           get
           {
               return _description;
           }

           set
           {
               if (_description == value)
               {
                   return;
               }

               RaisePropertyChanging(DescriptionPropertyName);
               _description = value;
               RaisePropertyChanged(DescriptionPropertyName);
           }
       }

       public const string NoOfContainersPropertyName = "NoOfContainers";
       private int _noofcontainer = 0;
       public int NoOfContainers
       {
           get
           {
               return _noofcontainer;
           }

           set
           {
               if (_noofcontainer == value)
               {
                   return;
               }

               RaisePropertyChanging(NoOfContainersPropertyName);
               _noofcontainer = value;
               RaisePropertyChanged(NoOfContainersPropertyName);
           }
       }
   }

    public class CompletedAndStoredCommodityListItem : CommodityListingItemBase
    {
        public const string IsCheckedPropertyName = "IsChecked";
        private bool _isChecked;
        public bool IsChecked
        {
            get
            {
                return _isChecked;
            }

            set
            {
                if (_isChecked == value)
                {
                    return;
                }

                RaisePropertyChanging(IsCheckedPropertyName);
                _isChecked = value;
                RaisePropertyChanged(IsCheckedPropertyName);

            }
        }
    }

    public class CommodityReceptionListItem : CommodityListingItemBase
    {
        public CommodityReceptionListItem()
        {
        }

        
       

        public const string IsCheckedPropertyName = "IsChecked";
        private bool _isChecked;
        public bool IsChecked
        {
            get
            {
                return _isChecked;
            }

            set
            {
                if (_isChecked == value)
                {
                    return;
                }

                RaisePropertyChanging(IsCheckedPropertyName);
                _isChecked = value;
                RaisePropertyChanged(IsCheckedPropertyName);

            }
        }
    }
}
