namespace Distributr.Core.Utility.Validation
{
    public interface IValidation<T> where T : class
    {
        ValidationResultInfo Validate(T itemToValidate);
    }
}
