using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace Distributr.CustomerSupport.Paging
{
    public static class Paginator
    {
        static Paginator()
        {
            ItemsPerPage = 10;
        }

        public static int ItemsPerPage { get; set; }
        [UIHint("SelectItemsPerPage")]
        public static SelectList ItemsPerPageList
        {
            get
            {
                var selectList = new Dictionary<string, string>()
                                     {
                                         {"10", "10"},
                                         {"15", "15"},
                                         {"20", "20"},
                                         {"25", "25"},
                                         {"30", "30"}
                                     };

                return new SelectList(selectList, "Key", "Value", "----");
            }
        }
    }
}