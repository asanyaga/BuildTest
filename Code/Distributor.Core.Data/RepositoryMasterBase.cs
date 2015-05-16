using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Transactions;
using log4net;
using System.Collections;

namespace Distributr.Core.Repository.Master
{
    abstract class RepositoryMasterBase
    {
        //private static readonly ILog _log = LogManager.GetLogger("MyLogger");
        protected static readonly ILog _log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        protected abstract string _cacheRegion {get;}
        protected abstract string _cacheGet { get; }
        /*protected abstract IEnumerable GetAll(bool includeDeactivated = false);

        public virtual bool GetItemUpdatedSinceDateTime(DateTime dateTime) {
            return false;
        }*/
    }
}
