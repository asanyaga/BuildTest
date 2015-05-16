using PaymentGateway.WSApi.Lib.Domain.MasterData;
using PaymentGateway.WSApi.Lib.Util;

namespace PaymentGateway.WSApi.Lib.Repository.MasterData.Clients
{
    public interface IClientRepository : IBaseRepository<Client>
    {
        PGQueryResult Query(PGQueryBase query);
        Client GetByCode(string id);
    }
    public interface IClientMemberRepository : IBaseRepository<ClientMember>
    {
        PGQueryResult Query(PGQueryBase query);
    }

}
