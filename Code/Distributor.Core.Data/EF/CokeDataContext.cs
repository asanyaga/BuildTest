
using log4net;

namespace Distributr.Core.Data.EF
{
    public class CokeDataContext : cokeIIEntities
    {
        private ILog _logger = LogManager.GetLogger("CokeDataContext");
        public CokeDataContext(string connectionString) : base(connectionString)
        {
            
            
            _logger.Debug("##### -------> Context ctor <----------#######");
        }

        protected override void Dispose(bool disposing)
        {
            _logger.Debug("##### -------> Context dispose <----------#######");
            base.Dispose(disposing);
        }
    }
   
}
