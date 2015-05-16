using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PaymentGateway.WSApi.Lib.Data.EF;
using PaymentGateway.WSApi.Lib.Domain.MasterData;
using PaymentGateway.WSApi.Lib.Repository.MasterData.Clients;
using PaymentGateway.WSApi.Lib.Util;
using PaymentGateway.WSApi.Lib.Validation;

namespace PaymentGateway.WSApi.Lib.Data.Repository.Clients
{
    public class ClientMemberRepository : IClientMemberRepository
    {
        private PGDataContext _ctx;

        public ClientMemberRepository(PGDataContext ctx)
        {
            _ctx = ctx;
        }

        public ValidationResultInfo Validate(ClientMember objToValidate)
        {
            return objToValidate.BasicValidation();
        }

        public int Save(ClientMember entity)
        {

            ValidationResultInfo vri = Validate(entity);
            if (!vri.IsValid)
            {
                throw new DomainValidationException(vri, "ClientMember provided not valid");
            }
            DateTime date = DateTime.Now;
            tblClientMember tbl = _ctx.tblClientMember.FirstOrDefault(s => s.Id == entity.Id || s.ExternalId==entity.ExternalId);
            if (tbl == null)
            {
                tbl = new tblClientMember();
                tbl.IM_DateCreated = date;
                _ctx.tblClientMember.Add(tbl);
            }
            tbl.IM_DateLastUpdated = date;
            tbl.Name = entity.Name;
            tbl.Code = entity.Code;
            tbl.MemberType =(int) entity.MemberType;
            tbl.ClientId = entity.Client.Id;
            tbl.ExternalId = entity.ExternalId;
            _ctx.SaveChanges();

            return tbl.Id;
        }

        public ClientMember GetById(int id)
        {
            var tbl = _ctx.tblClientMember.FirstOrDefault(s => s.Id == id);
            if (tbl != null)
                return Map(tbl);
            return null;
        }

        private ClientMember Map(tblClientMember tbl)
        {
            return new ClientMember
                       {
                           Client = Map(tbl.tblClient),
                           Name = tbl.Name,
                           Code = tbl.Code,
                           Id = tbl.Id,
                           ExternalId = tbl.ExternalId,
                           MemberType =(ClientMemberType) tbl.MemberType,

                       };
        }
        private Client Map(tblClient tbl)
        {
            if (tbl == null)
                return null;
            return new Client
            {
                Name = tbl.ClientName,
                Id = tbl.Id,
                ApplicationId = tbl.ApplicationId,
                Code = tbl.ClientKeyWord,
                ApplicationPassword = tbl.ApplicationPassword,
                ExternalId = tbl.ExternalId,
                Path = tbl.ClientUri,

            };
        }


        public List<ClientMember> GetAll()
        {
            return _ctx.tblClientMember.ToList().Select(Map).ToList();
        }

        public void Delete(int id)
        {
            var cm = _ctx.tblClientMember.Where(s => s.ClientId == id);
            foreach (var tblClientMember in cm)
            {
                _ctx.tblClientMember.Remove(tblClientMember);
            }
            _ctx.SaveChanges();
        }

        public PGQueryResult Query(PGQueryBase query)
        {
            var q = query as PGQueryClientMember;
            var entityQuery = _ctx.tblClientMember.AsQueryable();
            var queryResult = new PGQueryResult();
            if (!string.IsNullOrWhiteSpace(q.Name))
            {
                entityQuery = entityQuery
                    .Where(s => s.Name.Contains(q.Name));
            }
            if (q.ClientId.HasValue)
            {
                entityQuery = entityQuery
                    .Where(s => s.ClientId==q.ClientId.Value);
            }

            queryResult.Count = entityQuery.Count();
            entityQuery = entityQuery.OrderBy(s => s.Name);
            if (q.Skip.HasValue && q.Take.HasValue)
                entityQuery = entityQuery.Skip(q.Skip.Value).Take(q.Take.Value);
            queryResult.Result = entityQuery.ToList().Select(Map).OfType<MasterEntity>().ToList();
            return queryResult;
        }
    }
}
