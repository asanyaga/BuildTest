using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Distributr.Core.Data.IOC;
using Distributr.WPF.Lib.Data.IOC;
using GalaSoft.MvvmLight.Threading;
using StructureMap;

namespace Agrimanagr.DataImporter.Lib.IoC
{
  public  class Initializer
    {
        private static bool _isInitialized = false;

        public static void Init()
        {
            if (!_isInitialized)
            {

               Distributr_Middleware.WPF.Lib.IOC.Initializer.Init();

                _isInitialized = true;
            }
        }
    }
}
