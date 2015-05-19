using System.Web.Optimization;
using Microsoft.Ajax.Utilities;

namespace Distributr.HQ.Web
{
    public class BundleConfig
    {
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/Content/distributrscripts")
                .Include("~/Content/js/jquery-1.4.2.min.js")
                .Include("~/Content/js/jquery.tablesorter.js")
                .Include("~/Content/js/tablesort.js")
                .Include("~/Content/js/eye.js")
                .Include("~/Content/js/utils.js")
                .Include("~/Content/js/tabs.js")
                .Include("~/Content/js/combined_#.js")
                );

            bundles.Add(new StyleBundle("~/Content/css")
                .Include("~/Content/styles/main.css")
                .Include("~/Content/styles/Site.css")
                .Include("~/Content/styles/layout.css")
                .Include("~/Content/styles/tabs.css")
                .Include("~/Content/styles/main-menu.css")
                .Include("~/Content/styles/MenuMatic.css")
                .Include("~/Content/styles/tabs.css")
                .Include("~/Content/styles/tables.css")
                .Include("~/Content/themes/base/jquery-ui.css")
                .Include("~/Content/jquery.ui.datepicker.css")
                .Include("~/Content/styles/combined_#.css")
                );
        }
    }
}