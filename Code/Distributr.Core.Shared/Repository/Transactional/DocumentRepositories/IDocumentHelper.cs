using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Distributr.Core.Commands;

namespace Distributr.Core.Repository.Transactional.DocumentRepositories
{
   public interface IDocumentHelper
   {
       ICommand GetExternalRef(ICommand command);
       
      
   }
}
