using System;
using Sqo;
using Sqo.Attributes;

namespace Distributr.WPF.Lib.Impl.Model.Utility
{
    public class UnExecutedCommandLocal 
    {
        public int Id { set; get; }
        public string CommandType { set; get; }
        [MaxLength(1000)]
        public string Command { set; get; }
        [MaxLength(1000)]
        public string Reason { set; get; }
        public Guid DocumentId { set; get; }

       
    }
}
