using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Distributr.Core.Domain.Transactional.DocumentEntities.Exceptions
{
#if !SILVERLIGHT
   [Serializable]
#endif
    public class InvalidDocumentOperationException : Exception
    {
        public InvalidDocumentOperationException(string message) : base(message)
        {

        }
    }
}
