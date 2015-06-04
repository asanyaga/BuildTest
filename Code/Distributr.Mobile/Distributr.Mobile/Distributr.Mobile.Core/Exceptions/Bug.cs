namespace Distributr.Mobile.Core.Exceptions
{
    public class Bug : System.Exception
    {
        public Bug(string message, params object [] args) : base(string.Format(message, args)) { }
    }
}
