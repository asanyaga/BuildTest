using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Agrimanagr.DataImporter.Lib.ViewModels;

namespace Agrimanagr.DataImporter.Lib.Utils
{
   public interface IMapEntityColumnPosition
   {
       Dictionary<int, string> GetEntityMapping(ImportEntity entity);
   }
}
