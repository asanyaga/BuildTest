using Distributr.WPF.Lib.Data.EF;
using System.Configuration;
namespace DistributrAgrimanagrFeatures.Helpers.DB
{
    /// <summary>
    /// USED IN THE INTEGRATION TESTS NOT PRODUCTION
    /// </summary>
    public class DistributrLocalContextTest : DistributrLocalContext
    {
        public DistributrLocalContextTest()
            : base(ConfigurationManager.AppSettings["Hub_RoutingConnectionString"])
        {

        }

        
    }

   
    
}
