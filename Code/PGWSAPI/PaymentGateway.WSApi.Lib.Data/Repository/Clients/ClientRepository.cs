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
   public class ClientRepository : IClientRepository
    {
       private PGDataContext _ctx;

       public ClientRepository(PGDataContext ctx)
       {
           _ctx = ctx;
       }

       public ValidationResultInfo Validate(Client objToValidate)
       {
           return objToValidate.BasicValidation();
       }

       public int Save(Client entity)
       {
           ValidationResultInfo vri = Validate(entity);
           if (!vri.IsValid)
           {
               throw new DomainValidationException(vri, "Product Details provided not valid");
           }
           DateTime date = DateTime.Now;
           tblClient tbl = _ctx.tblClient.FirstOrDefault(s=>s.Id==entity.Id);
           if (tbl == null)
           {
               tbl = new tblClient();
               tbl.IM_DateCreated = date;
               
               _ctx.tblClient.Add(tbl);
           }
           tbl.ClientName = entity.Name;
           tbl.ClientUri = entity.Path;
           tbl.ClientKeyWord = entity.Code;
           tbl.ApplicationId = entity.ApplicationId;
           tbl.ApplicationPassword = entity.ApplicationPassword;
           tbl.ExternalId = entity.ExternalId;
           tbl.IM_DateLastUpdated = date;
           _ctx.SaveChanges();
          
           return tbl.Id;
       }

       public Client GetById(int id)
       {
           var tbl= _ctx.tblClient.FirstOrDefault(s => s.Id == id);
           if (tbl != null)
               return Map(tbl);
           return null;
       }

       private Client Map(tblClient tbl)
       {
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

       public List<Client> GetAll()
       {
           return _ctx.tblClient.ToList().Select(Map).ToList();
       }

       public void Delete(int id)
       {
           var tbl = _ctx.tblClient.FirstOrDefault(p => p.Id == id);
           if (tbl != null)
           {
               tbl.IM_Status = 0;
               _ctx.SaveChanges();
          }
       }

       public PGQueryResult Query(PGQueryBase query)
       {
           var q = query as PGQuery;
           var entityQuery = _ctx.tblClient.AsQueryable();
           var queryResult = new PGQueryResult();
           if (q.ShowInactive)
               entityQuery = entityQuery.Where(k => k.IM_Status == 0);
           else
               entityQuery = entityQuery.Where(k => k.IM_Status == 1);
           if (!string.IsNullOrWhiteSpace(q.Name))
           {
               entityQuery = entityQuery
                   .Where(s => s.ClientName.Contains(q.Name) );
           }

           queryResult.Count = entityQuery.Count();
           entityQuery = entityQuery.OrderBy(s => s.ClientName);
           if (q.Skip.HasValue && q.Take.HasValue)
               entityQuery = entityQuery.Skip(q.Skip.Value).Take(q.Take.Value);
           queryResult.Result = entityQuery.ToList().Select(Map).OfType<MasterEntity>().ToList();
           return queryResult;
       }

       public Client GetByCode(string id)
       {
           var tbl = _ctx.tblClient.FirstOrDefault(s => s.ClientKeyWord == id);
           if (tbl != null)
               return Map(tbl);
           return null;
       }
    }
}
