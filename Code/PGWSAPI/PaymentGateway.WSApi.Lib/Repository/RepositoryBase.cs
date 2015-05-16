

namespace PaymentGateway.WSApi.Lib.Repository
{
    public abstract class RepositoryBase
    {
        protected abstract string _cacheKey { get; }
        protected abstract string _cacheListKey { get; }
    }
}
