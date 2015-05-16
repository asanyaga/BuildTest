using System.Collections.Generic;
using System.Linq;
using Distributr.WPF.Lib.Data.EF;

using Distributr.WPF.Lib.Impl.Model.Utility;
using Distributr.WPF.Lib.Impl.Repository.Utility;
using Distributr.WPF.Lib.Services.Service;
using Distributr.WPF.Lib.Services.Service.Utility;


namespace Distributr.WPF.Lib.Data.Repository.Utility
{
    public class GeneralSettingRepository : IGeneralSettingRepository
    {

        private DistributrLocalContext _ctx;
         public GeneralSettingRepository(DistributrLocalContext _ctx)
        {
            this._ctx = _ctx;
        }
        public int Save(GeneralSetting log)
        {
            GeneralSettingLocal setting = new GeneralSettingLocal
            {
                SettingKey = (int)log.SettingKey,
                SettingValue = log.SettingValue
            };

            GeneralSettingLocal exist = _GetByKey(setting.SettingKey);
            if (exist == null)
            {
                exist = new GeneralSettingLocal();
                _ctx.GeneralSettingLocal.Add(exist);
                exist.Id = setting.Id;
                exist.SettingKey = setting.SettingKey;
            }
            exist.SettingValue = setting.SettingValue;
            
            _ctx.SaveChanges();
            return exist.Id;
        }

        public List<GeneralSetting> GetAll()
        {
            return _ctx.GeneralSettingLocal.Select(n => Map(n)).ToList();
        }

        public GeneralSetting GetById(int Id)
        {
            return Map(_GetById(Id)); 
        }

        private GeneralSettingLocal _GetById(int id)
        {
            return _ctx.GeneralSettingLocal.FirstOrDefault(p => p.Id == id);
        }
        private GeneralSettingLocal _GetByKey(int id)
        {
            return _ctx.GeneralSettingLocal.FirstOrDefault(p => p.SettingKey == id);
        }
        GeneralSetting Map(GeneralSettingLocal m)
        {
            if (m == null) return null;
            return new GeneralSetting
            {
                SettingKey = (GeneralSettingKey)m.SettingKey,
                SettingValue = m.SettingValue
            };
        }
        public GeneralSetting GetByKey(GeneralSettingKey key)
       {
            return Map(_GetByKey((int) key));
        }
    }
}
