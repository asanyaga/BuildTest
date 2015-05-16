using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

using Distributr.Core.Domain.Master.CostCentreEntities;
using Distributr.Core.Domain.Master.Util;
using log4net;
using Distributr.Core.Domain.Master;
using Distributr.Core.Utility.Validation;

namespace Distributr.Core.Repository.Master
{
    public abstract class RepositoryMasterBase<T>
    {
        /*private string _cacheRegion = "";
        private string _cacheGet = "";*/

        //protected abstract string _cacheRegion { get; }
        //protected abstract string _cacheGet { get; }

        protected abstract string _cacheKey { get; }

        protected abstract string _cacheListKey { get; }

        protected static readonly ILog _log = LogManager.GetLogger("HQ");

        /*protected abstract string CacheRegion { set; }
        protected abstract string CacheGet { set; }*/

        public abstract IEnumerable<T> GetAll(bool includeDeactivated = false);

        public virtual bool GetItemUpdatedSinceDateTime(DateTime dateTime)
        {
            _log.Debug("GetItemUpdatedSinceDateTime");
            bool updated=GetAll(true).Select(n => n as IInfrastructureMetadata).Any(n => n._DateLastUpdated > dateTime);
            _log.Debug("GetItemUpdatedSinceDateTime: updated:"+updated);
            return updated;           
        }

        public virtual DateTime GetLastTimeItemUpdated()
        {
            _log.Debug("GetLastTimeItemUpdated");
            DateTime date = (from region in GetAll(true).Select(n => n as IInfrastructureMetadata) select region._DateLastUpdated).Max();
            _log.Debug("GetLastTimeItemUpdated DateTime:"+date);
            return date;
        }

        public virtual IEnumerable<T> GetItemUpdated(DateTime dateTime)
        {
            _log.Debug("GetItemUpdated");
            IEnumerable<T> item = GetAll(true).Select(p => p as IInfrastructureMetadata).Where(s => s._DateLastUpdated > dateTime).OfType<T>().ToList<T>();
            _log.Debug("GetItemUpdated DateTime:" + dateTime);
            return item;
        }

        public virtual void LogErrors(MasterEntity itemToValidate, ValidationResultInfo vri)
        {
            var errors = vri.Results.Aggregate("Error: Invalid target fields. MasterId: " + itemToValidate.Id,
                (current, msg) => current + ("- " + msg.ErrorMessage + "\n"));
            _log.Error(errors);
        }

        public IPagenatedList<T> GetAll(int currentPage, int itemPerPage, string searchText, bool includeDeactivated = false)
        {
            searchText = searchText.Trim().ToLower();
            var allMasterData = GetAll(includeDeactivated).ToList();
            IEnumerable<T> toTake = null;
            toTake = allMasterData.Where(n =>
                                         {
                                             if (searchText.Equals(string.Empty)) return true;
                                             var nameProperty = n.GetType().GetProperty("Name");
                                             if (nameProperty != null)
                                             {
                                                 if (nameProperty.GetValue(n).ToString().ToLower().Contains(searchText))
                                                     return true;
                                             }
                                             var codeProperty = n.GetType().GetProperty("Code");
                                             if (codeProperty != null)
                                             {
                                                 if (codeProperty.GetValue(n).ToString().ToLower().Contains(searchText))
                                                     return true;
                                             }
                                             var descriptionProperty = n.GetType().GetProperty("Description");
                                             if (descriptionProperty != null)
                                             {
                                                 if (
                                                     descriptionProperty.GetValue(n).ToString().ToLower().Contains(
                                                         searchText))
                                                     return true;
                                             }
                                             return false;
                                         }
                );

         //   var toTakeOrdered = CreateSortList<T>(toTake, "Name", SortDirection.Ascending);
           // toTakeOrdered = CreateSortList<T>(toTakeOrdered, "Description", SortDirection.Ascending);

            IPagenatedList<T> pagenatedMasterData = new PagenatedList<T>(toTake.AsQueryable(), currentPage, itemPerPage, allMasterData.Count());
            return pagenatedMasterData;
        }

        //private List<T> CreateSortList<T>(IEnumerable<T> dataSource, string fieldName, SortDirection sortDirection)
        //{
        //    List<T> returnList = new List<T>();
        //    returnList.AddRange(dataSource);
        //    PropertyInfo propInfo = typeof (T).GetProperty(fieldName);
        //    if (propInfo != null)
        //    {
        //        Comparison<T> compare = delegate(T a, T b)
        //                                    {
        //                                        bool asc = sortDirection == SortDirection.Ascending;
        //                                        object valueA = asc
        //                                                            ? propInfo.GetValue(a, null)
        //                                                            : propInfo.GetValue(b, null);
        //                                        object valueB = asc
        //                                                            ? propInfo.GetValue(b, null)
        //                                                            : propInfo.GetValue(a, null);

        //                                        return valueA is IComparable
        //                                                   ? ((IComparable) valueA).CompareTo(valueB)
        //                                                   : 0;
        //                                    };
        //        returnList.Sort(compare);
        //    }
        //    return returnList;
        //}


        public int GetCount(bool includeDeactivated = false)
        {
            return GetAll(includeDeactivated).Count();
        }
    }
}
