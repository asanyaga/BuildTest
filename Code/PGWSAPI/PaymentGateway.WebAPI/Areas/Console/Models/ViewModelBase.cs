using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace PaymentGateway.WebAPI.Areas.Console.Models
{
    public static class ViewModelBase
    {
        static ViewModelBase()
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