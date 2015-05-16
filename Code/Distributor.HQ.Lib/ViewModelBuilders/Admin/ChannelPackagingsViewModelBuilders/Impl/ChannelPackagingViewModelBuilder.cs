using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Distributr.HQ.Lib.ViewModels.Admin.ChannelPackagingsViewModels;
using Distributr.Core.Domain.Master.ProductEntities;
using Distributr.Core.Domain.Master.CostCentreEntities;
using Distributr.Core.Repository.Master.ProductRepositories;
using Distributr.Core.Repository.Master.CostCentreRepositories;
using Distributr.Core.Domain.Master.ChannelPackagings;
using Distributr.Core.Repository.Master.ChannelPackagings;


namespace Distributr.HQ.Lib.ViewModelBuilders.Admin.ChannelPackagingsViewModelBuilders.Impl
{
    public class ChannelPackagingViewModelBuilder : IChannelPackagingViewModelBuilder
    {
        IProductPackagingRepository _productPackagingRepository;
        IOutletTypeRepository _outletTypeRepository;
        IChannelPackagingRepository _channelPackagingRepository;
        public ChannelPackagingViewModelBuilder(IProductPackagingRepository productPackagingRepository,
        IOutletTypeRepository outletTypeRepository,
            IChannelPackagingRepository channelPackagingRepository
            )
        {
            _outletTypeRepository = outletTypeRepository;
            _productPackagingRepository = productPackagingRepository;
            _channelPackagingRepository = channelPackagingRepository;
        }
        public IList<ChannelPackagingViewModel> GetAll()
        {
           


            //var items = _channelPackagingRepository.GetAll();
            //ChannelPackagingViewModel cpvm = new ChannelPackagingViewModel
            //{
            //    Packagings = items.Select(n => new ChannelPackagingViewModel.PackagingVM { 
            //     Id=n.Id,
            //      PackName=n.Name
            //    }).ToList()
            //};
            //return cpvm;
            throw new NotImplementedException();
        }

        public List<CPItem> ChannelPackagingItems()
        {
            AddChannelPackagingPlaceholders();
            return _channelPackagingRepository.GetAll().Select(n => new CPItem { Id = n.Id, Checked = n.IsChecked, PackagingId = n.Packaging.Id, OutletTypeId = n.OutletType.Id }).ToList();
        }

        private void AddChannelPackagingPlaceholders()
        {
            int count = _productPackagingRepository.GetAll().Count() * _outletTypeRepository.GetAll().Count();
            if (count != _channelPackagingRepository.GetAll().Count())
            {
                List<ChannelPackaging> cpitemsdb = null;
                cpitemsdb = _channelPackagingRepository.GetAll().ToList();

                var cpToDeactivate = cpitemsdb.Where(n => n.OutletType == null || n.Packaging == null);

                if (cpToDeactivate.Count() > 0)
                {
                    foreach (var item in cpToDeactivate)
                    {
                        if (item.IsChecked)
                            item.IsChecked = false;

                        _channelPackagingRepository.SetInactive(item);
                    }

                    cpitemsdb.Clear();
                    cpitemsdb = _channelPackagingRepository.GetAll().ToList();
                }

                List<CPItem> cpitems = new List<CPItem>();
                foreach (var pp in _productPackagingRepository.GetAll())
                {
                    foreach (var outlet in _outletTypeRepository.GetAll())
                    {
                        Guid ppid = pp.Id;
                        Guid oid = outlet.Id;
                        Guid dbid = Guid.Empty;
                        bool ischecked = false;
                        ChannelPackaging existing = null;
                        try
                        {
                            existing = cpitemsdb.First(n => n.OutletType.Id == oid && n.Packaging.Id == ppid);
                        }
                        catch
                        {
                            //deactivate this ChChanh
                            //if isChecked, unCheck
                        }

                        if (existing != null)
                        {
                            var dbitem = cpitemsdb.First(n => n.OutletType.Id == oid && n.Packaging.Id == ppid);
                            dbid = dbitem.Id;
                            ischecked = dbitem.IsChecked;
                        }

                        //if (cpitemsdb.Any(n => n.OutletType.Id == oid && n.Packaging.Id == ppid))
                        //    {
                        //        var dbitem = cpitemsdb.First(n => n.OutletType.Id == oid && n.Packaging.Id == ppid);
                        //        dbid = dbitem.Id;
                        //        ischecked = dbitem.IsChecked;

                        //    }
                        cpitems.Add(new CPItem {Id = dbid, Checked = ischecked, OutletTypeId = oid, PackagingId = ppid});
                    }
                }
                foreach (var itemtoadd in cpitems.Where(n => n.Id == Guid.Empty))
                {
                    ChannelPackaging cp = new ChannelPackaging(Guid.NewGuid())
                    {
                        IsChecked = false,
                        OutletType = _outletTypeRepository.GetById(itemtoadd.OutletTypeId),
                        Packaging = _productPackagingRepository.GetById(itemtoadd.PackagingId)
                    };
                    _channelPackagingRepository.Save(cp);
                }
            }
        }



        public ChannelPackagingViewModel GetPackaging()
        {
            var items = _productPackagingRepository.GetAll();
            var items2 = _outletTypeRepository.GetAll();
            ChannelPackagingViewModel cpvm = new ChannelPackagingViewModel
            {
                Packagings = items.Select(n => new ChannelPackagingViewModel.PackagingVM
                {
                    Id = n.Id,
                    PackName = n.Name
                }).ToList()
                ,
                OutletTypes = items2.Select(n => new ChannelPackagingViewModel.OutletTypeVM
                {
                    Id = n.Id,
                    OutletTypeName = n.Name
                }
                ).ToList()
            };
            return cpvm;
        }

        public ChannelPackagingViewModel GetOutLetTypes()
        {
            var items2 = _outletTypeRepository.GetAll();
            ChannelPackagingViewModel cpvm = new ChannelPackagingViewModel
            {
                OutletTypes = items2.Select(n => new ChannelPackagingViewModel.OutletTypeVM
                {
                    Id = n.Id,
                    OutletTypeName = n.Name
                }
                 ).ToList()
            };
            return cpvm;
        }







        public void Savex(ChannelPackagingViewModel cpvm)
        {
            ChannelPackaging cp = new ChannelPackaging(cpvm.ChannelPId)
            {
                OutletType = _outletTypeRepository.GetById(cpvm.OutletTypeId),
                Packaging = _productPackagingRepository.GetById(cpvm.PackageId),
                IsChecked = cpvm.IsChecked

            };
            _channelPackagingRepository.Save(cp);
        }

        public void Save(string[] cb)
        {
            List<ChannelPackagingViewModel> xx = new List<ChannelPackagingViewModel>();
            if (cb != null)
            {
                for (int i = 0; i < cb.Length; i++)
                {
                    ChannelPackagingViewModel cpvmmoto = new ChannelPackagingViewModel();
                    string[] s = cb[i].ToString().Split(',');
                    string packId = s[0];
                    string outId = s[1];
                    string cId = s[2];

                    cpvmmoto.ChannelPId = Guid.Parse(cId);
                    cpvmmoto.PackageId = Guid.Parse(packId);
                    cpvmmoto.OutletTypeId = Guid.Parse(outId);
                    xx.Add(cpvmmoto);
                }
           
                foreach (CPItem cp in ChannelPackagingItems())
                {
                    ChannelPackagingViewModel sa = null;
                    if (xx.Any(p => p.ChannelPId == cp.Id))
                    {
                        sa = xx.FirstOrDefault(p => p.ChannelPId == cp.Id);
                        sa.IsChecked = true;
                    }
                    else
                    {
                        sa = new ChannelPackagingViewModel();
                        sa.OutletTypeId = cp.OutletTypeId;
                        sa.PackageId = cp.PackagingId;
                        sa.ChannelPId = cp.Id;
                        sa.IsChecked = false;
                    }
                    Savex(sa);
                } 
            }
            else
            {
                foreach (CPItem cp in ChannelPackagingItems())
                {
                    ChannelPackagingViewModel sa = null;
                    if (xx.Any(p => p.ChannelPId == cp.Id))
                    {
                        sa = xx.FirstOrDefault(p => p.ChannelPId == cp.Id);
                        sa.IsChecked = true;
                    }
                    else
                    {
                        sa = new ChannelPackagingViewModel();
                        sa.OutletTypeId = cp.OutletTypeId;
                        sa.PackageId = cp.PackagingId;
                        sa.ChannelPId = cp.Id;
                        sa.IsChecked = false;
                    }
                    Savex(sa);
                } 
            }
            
        }
      
        public ChannelPackagingViewModel Get()
        {
            List<CPItem> dbitems = ChannelPackagingItems();
            ChannelPackagingViewModel vm = new ChannelPackagingViewModel();
            var items = _productPackagingRepository.GetAll();
            var items2 = _outletTypeRepository.GetAll();
            var item3=_channelPackagingRepository.GetAll();

            vm.Packagings = items.Select(n => new ChannelPackagingViewModel.PackagingVM
            {
                Id = n.Id,
                PackName = n.Name
            }).ToList();

            vm.OutletTypes = items2.Select(n => new ChannelPackagingViewModel.OutletTypeVM
            {
                Id = n.Id,
                OutletTypeName = n.Name
            }
            ).ToList();
            vm.chanId = item3.Select(n => new ChannelPackagingViewModel.ChannelPacksVm
                {
                    cId = n.Id
                }
                ).ToList();
            List<ChannelPackagingViewModel.RowItem[]> cpitems = new List<ChannelPackagingViewModel.RowItem[]>();
            //ChannelPackagingViewModel.RowItem[] row1 = new ChannelPackagingViewModel.RowItem[vm.Packagings.Count()+1];
            ChannelPackagingViewModel.RowItem[] row1 = new ChannelPackagingViewModel.RowItem[vm.OutletTypes.Count() + 1];
            
            var topRow = new ChannelPackagingViewModel.RowItem[vm.OutletTypes.Count + 1];
            for (int i = 0; i < vm.OutletTypes.Count + 1; i++)
            {
                var rowItem = new ChannelPackagingViewModel.RowItem();
                topRow[i] = rowItem;
                if(i == 0)
                {
                    topRow[0].CPLookup = "";
                }
                else
                {
                    topRow[i].CPLookup = vm.OutletTypes[i - 1].OutletTypeName;
                }
            }
            cpitems.Add(topRow);
            foreach (var packagingType in vm.Packagings)
            {
                var row = new ChannelPackagingViewModel.RowItem[vm.OutletTypes.Count + 1];
                for (int i = 0; i <= vm.OutletTypes.Count; i++)
                {
                    var rowItem = new ChannelPackagingViewModel.RowItem();
                    row[i] = rowItem;
                    if (i == 0)
                    {
                        row[i].CPLookup = packagingType.PackName;
                    }
                    else
                    {
                        bool isChecked = false;
                        Guid outletTypeId = vm.OutletTypes[i - 1].Id;
                        Guid packagingTypeId = packagingType.Id;
                        var df = dbitems.FirstOrDefault(n => n.OutletTypeId == outletTypeId && n.PackagingId == packagingTypeId);
                        Guid cID = df.Id;
                        string cellref = string.Format("{0},{1},{2}", packagingTypeId, outletTypeId, cID);
                        row[i].CPLookup = cellref;                     
                        if(df!=null)
                        isChecked=df.Checked;                        
                        row[i].IsChecked = isChecked;
                    }
                }
                cpitems.Add(row);
            }
            vm.RowItems = cpitems;
            return vm;
            //----ORIGINAL DESIGN----//
            ////for(int i=0;i<vm.Packagings.Count()+1;i++)
            ////{
            ////    ChannelPackagingViewModel.RowItem ri = new ChannelPackagingViewModel.RowItem();
            ////    row1[i] = ri;
            ////    if(i == 0)
            ////        row1[0].CPLookup = "";
            ////    else
            ////        row1[i].CPLookup = vm.Packagings[i-1].PackName;
            ////}
            ////cpitems.Add(row1);
            ////    foreach (var ot in vm.OutletTypes)
            ////    {
            ////        ChannelPackagingViewModel.RowItem[] row = new ChannelPackagingViewModel.RowItem[vm.Packagings.Count() + 1];              

            ////        for (int i = 0; i <= vm.Packagings.Count(); i++)
            ////        {
            ////            ChannelPackagingViewModel.RowItem ri1 = new ChannelPackagingViewModel.RowItem();
            ////            row[i] = ri1;
            ////            if (i == 0)
            ////                row[0].CPLookup = ot.OutletTypeName;
            ////            else
            ////            {
            ////                bool isChecked = false;
            ////                Guid outid = ot.Id;
            ////                Guid ppid = vm.Packagings[i - 1].Id;
            ////                var df = dbitems.FirstOrDefault(n => n.OutletTypeId == outid && n.PackagingId == ppid);
            ////                Guid cID = df.Id;//vm.chanId[i-1].cId > 0 ? vm.chanId[i-1].cId : 0;
            ////                string cellref = string.Format("{0},{1},{2}", ppid, outid,cID);
            ////                row[i].CPLookup = cellref;                     
            //                if(df!=null)
            ////                isChecked=df.Checked;                        
            ////                row[i].IsChecked = isChecked;
            ////            }
            ////        }
            ////        cpitems.Add(row);

            ////    }

            ////    vm.RowItems = cpitems;
            ////    return vm;
        }
    }
}
