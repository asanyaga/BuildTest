namespace Distributr.WPF.Lib.Services.Service.Utility
{
    public enum GeneralSettingKey: int
    {
        ReportUrl=1,
        RecordsPerPage=2,
        ReportUsername=3,
        ReportPassword=4,
        ReportFolder = 5,
       
        FiscalPrinterEnabled=6,
        FiscalPrinterPort=7,
        FiscalPrinterPortSpeed = 8,

        SyncPageSize = 9,


    }
    public class GeneralSetting
    {
        public GeneralSettingKey SettingKey { set; get; }
        public string SettingValue { set; get; }
    }
}
