using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Distributr.Core.Data.EF;
using Distributr.Core.Domain.Master.CostCentreEntities;
using Distributr.Core.Domain.Transactional.DocumentEntities;


namespace Distributr.Reports
{
    public static class ReportConnection
    {
        public static string connectionString = ConfigurationSettings.AppSettings["cokeconnectionstring"];

        public static IEnumerable<TSource> DistinctBy<TSource, TKey>(
            this IEnumerable<TSource> source,
            Func<TSource, TKey> keySelector,
            IEqualityComparer<TKey> comparer = null)
        {
            HashSet<TKey> knownKeys = new HashSet<TKey>(comparer);
            return source.Where(element => knownKeys.Add(keySelector(element)));
        }

        public static IQueryable<tblDocument> WhereIsSale(this IQueryable<tblDocument> source)
        {
            return from doc in source
                   where doc.Id == doc.OrderParentId
                   where doc.DocumentTypeId == (int)DocumentType.Order
                   where (doc.OrderOrderTypeId == (int)OrderType.DistributorPOS
                          || (doc.OrderOrderTypeId == (int)OrderType.OutletToDistributor
                              && doc.DocumentStatusId == (int)DocumentStatus.Closed))
                   select doc;
        }

        public static Expression<Func<tblLineItems, bool>> IsDistributorPurchase(this tblLineItems line)
        //public static bool IsDistributorPurchase(this tblLineItems line)
        {
            var doc = line.tblDocument;
            return l => (doc.DocumentTypeId == /*DocumentType.Order*/ 1
                         && (doc.OrderOrderTypeId == /*OrderType.DistributorToProducer*/ 2
                             && doc.DocumentStatusId == /*DocumentStatus.Closed*/ 99))
                        && line.OrderLineItemType == /*OrderLineItemType.PostConfirmation*/ 2;
        }


        public static Expression<Func<tblLineItems, bool>> IsDistributorReturn(this tblDocument doc)
        {
            return l => doc.DocumentTypeId == (int)DocumentType.ReturnsNote
                        && doc.OrderOrderTypeId == (int)ReturnsNoteType.DistributorToHQ;
        }
        public static Expression<Func<tblLineItems, bool>> IsDistributorReturn(this tblLineItems line)
        {
            return null;// line.tblDocument.IsDistributorReturn();
        }

        public static Expression<Func<tblDocument, tblCostCentre>> GetDistributor(this tblDocument doc, CokeDataContext ctx = null)
        {
            ctx = ctx ?? new CokeDataContext(connectionString);

            var distributor = from cc in ctx.tblCostCentre
                              where cc.Id == doc.DocumentIssuerCostCentreId
                              select
                                  (cc.CostCentreType == (int)CostCentreType.Distributor ||
                                   cc.CostCentreType == (int)CostCentreType.Producer)
                                      ? cc
                                      : (from cc_ in ctx.tblCostCentre
                                         where cc_.Id == cc.ParentCostCentreId.Value
                                         select cc_).FirstOrDefault();

            return d => distributor.FirstOrDefault();
        }
        public static Expression<Func<tblDocument, tblCostCentre>> GetSalesman(this tblDocument doc, CokeDataContext ctx = null)
        {
            ctx = ctx ?? new CokeDataContext(connectionString);
            var salesman = from cc in ctx.tblCostCentre
                           where cc.Id == doc.DocumentIssuerCostCentreId
                           select (cc.CostCentreType == (int)CostCentreType.DistributorSalesman)
                                   ? cc : null;
            return d => salesman.FirstOrDefault();
        }

    }
}

