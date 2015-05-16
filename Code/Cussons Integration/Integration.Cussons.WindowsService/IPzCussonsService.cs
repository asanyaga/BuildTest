using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Quartz;

namespace Integration.Cussons.WindowsService
{
    public interface IPzCussonsService
    {
        void Start();
        void Stop();
    }

    internal class MasterDataImportJob : IJob
    {
        public void Execute(IJobExecutionContext context)
        {
            Console.WriteLine("test");
        }
    }
}
