namespace Distributr.WPF.Lib.Services.Service.Utility
{
    public enum LogType { Debug = 1, Info = 2, Error = 3 }
    public class Log
    {
        public int OID { get; set; }
        public LogType LogType { get; set; }
        public string Message { get; set; }
        public string MoreInfo { get; set; }
    }
}
