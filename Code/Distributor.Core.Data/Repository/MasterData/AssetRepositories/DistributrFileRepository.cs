using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using Distributr.Core.Data.EF;
using Distributr.Core.Data.Utility;
using Distributr.Core.Data.Utility.Caching;
using Distributr.Core.Domain.Master;
using Distributr.Core.Domain.Master.AssetEntities;
using Distributr.Core.Repository.Master;
using Distributr.Core.Repository.Master.AssetRepositories;
using Distributr.Core.Utility.Caching;
using Distributr.Core.Utility.Validation;

namespace Distributr.Core.Data.Repository.MasterData.AssetRepositories
{
    internal class DistributrFileRepository : RepositoryMasterBase<DistributrFile>, IDistributrFileRepository
    {

        CokeDataContext _ctx;
        ICacheProvider _cacheProvider;

        public DistributrFileRepository(CokeDataContext ctx, ICacheProvider cacheProvider)
        {
            _ctx = ctx;
            _cacheProvider = cacheProvider;
        }

        public ValidationResultInfo Validate(DistributrFile itemToValidate)
        {
            ValidationResultInfo vri = itemToValidate.BasicValidation();
            if (itemToValidate._Status == EntityStatus.Inactive || itemToValidate._Status == EntityStatus.Deleted)
                return vri;
            if (itemToValidate.Id == Guid.Empty)
                vri.Results.Add(new ValidationResult("Enter Valid  Guid ID"));

            return vri;
        }

        public Guid Save(DistributrFile entity, bool? isSync = null)
        {
            ValidationResultInfo vri = Validate(entity);

            if (!vri.IsValid)
            {
                _log.Debug("Files   not valid");
                throw new DomainValidationException(vri, "Files Not valid");
            }
            DateTime dt = DateTime.Now;

            tblFiles files = _ctx.tblFiles.FirstOrDefault(n => n.Id == entity.Id);
            if (files == null)
            {
                files = new tblFiles();
                files.IM_Status = (int)EntityStatus.Active; ;
                files.IM_DateCreated = dt;
                files.Id = entity.Id;
                _ctx.tblFiles.AddObject(files);

            }
            var entityStatus = (entity._Status == EntityStatus.New) ? EntityStatus.Active : entity._Status;
            if (files.IM_Status != (int)entityStatus)
                files.IM_Status = (int)entity._Status;
            files.FileType = (int)entity.FileType;
            files.FileExtension = entity.FileExtension;
            files.FileDescription = entity.Description;
            files.FileData = Convert.FromBase64String(entity.FileData);
            files.IM_DateLastUpdated = dt;
            _ctx.SaveChanges();
           // _cacheProvider.InvalidateRegion(_cacheRegion);
            return files.Id; 
        }
        private byte[] stringToBase64ByteArray(String input)
        {
            byte[] ret = System.Text.Encoding.Unicode.GetBytes(input);
            ret = System.Text.Encoding.Unicode.GetBytes(input);
            return ret;
        }

        public void SetInactive(DistributrFile entity)
        {
            throw new NotImplementedException();
        }

        public void SetActive(DistributrFile entity)
        {
            throw new NotImplementedException();
        }

        public void SetAsDeleted(DistributrFile entity)
        {
            throw new NotImplementedException();
        }

        public DistributrFile GetById(Guid Id, bool includeDeactivated)
        {
            tblFiles file = _ctx.tblFiles.FirstOrDefault(p => p.Id == Id);
            if (file == null) return null;
            else
                return Map(file);

        }

        private DistributrFile Map(tblFiles file)
        {
            if (file == null)
                return null;
            DistributrFile xfile = new DistributrFile(file.Id)
                                       {
                                           Description = file.FileDescription,
                                           FileData = Convert.ToBase64String(file.FileData),
                                           FileExtension = file.FileExtension,
                                           FileType = (DistributrFileType) file.FileType,

                                       };
                        xfile._SetDateCreated(file.IM_DateCreated);
            xfile._SetDateLastUpdated(file.IM_DateLastUpdated);
            xfile._SetStatus((EntityStatus)file.IM_Status);
                  
            return xfile;
        }


        protected override string _cacheKey
        {
            get { throw new NotImplementedException(); }
        }

        protected override string _cacheListKey
        {
            get { throw new NotImplementedException(); }
        }

        public override IEnumerable<DistributrFile> GetAll(bool includeDeactivated)
        {
            return _ctx.tblFiles.ToList().Select(s => Map(s)).ToList();
        }
    }
}
