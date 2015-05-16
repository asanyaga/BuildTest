using Sqo;
using Sqo.Attributes;

namespace Distributr.WPF.Lib.Impl.Model.Utility
{
    public class GeneralSettingLocal 
    {
        public int Id { set;get;}
        [UniqueConstraint]
        public int SettingKey { set; get; }
        public string SettingValue { set; get; }

    }
}
