using System.Collections.Generic;
using System.Linq;
using Distributr.WPF.Lib.Data.EF;

using Distributr.WPF.Lib.Impl.Model.Utility;
using Distributr.WPF.Lib.Impl.Repository.Utility;


namespace Distributr.WPF.Lib.Data.Repository.Utility
{
    public class UnExecutedCommandRepository : IUnExecutedCommandRepository
    {
        private DistributrLocalContext _ctx;
        public UnExecutedCommandRepository(DistributrLocalContext _ctx)
        {
            this._ctx = _ctx;
        }
        public int Save(UnExecutedCommandLocal log)
        {
            UnExecutedCommandLocal exist = GetById(log.Id);
            if (exist == null)
            {
                exist = new UnExecutedCommandLocal();
                _ctx.UnExecutedCommandLocal.Add(exist);
            }
            exist.Id = log.Id;
            exist.CommandType = log.CommandType;
            exist.Command = log.Command;
            exist.Reason = log.Reason;
            exist.DocumentId = log.DocumentId;
            _ctx.SaveChanges();
            return log.Id;
        }

        public List<UnExecutedCommandLocal> GetAll()
        {
            /*IObjectList<UnExecutedCommandLocal> all = siaqodb.LoadAll<UnExecutedCommandLocal>();
            return all.ToList();*/
            var all = _ctx.UnExecutedCommandLocal.ToList();
            return all.ToList();
        }
        public UnExecutedCommandLocal GetById(int id)
        {
            return _ctx.UnExecutedCommandLocal.Cast<UnExecutedCommandLocal>().FirstOrDefault(n => n.Id == id);
        }
    }
}
