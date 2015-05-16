namespace Distributr.WPF.Lib.Services.Payment
{
    public abstract class ResponseBase
    {
        public string ErrorInfo { get; set; }
    }

    public class ResponseBasic : ResponseBase
    {
        public string Result { get; set; }
    }
}
