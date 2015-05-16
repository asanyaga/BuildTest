using System.Web;
using System.Web.Optimization;

namespace Agrimanagr.HQ
{
    public class BundleConfig
    {
        // For more information on Bundling, visit http://go.microsoft.com/fwlink/?LinkId=254725
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/jquery-{version}.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryui").Include(
                        "~/Scripts/jquery-ui-{version}.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/Scripts/jquery.unobtrusive*",
                        "~/Scripts/jquery.validate*"));
            bundles.Add(new ScriptBundle("~/bundles/agrimanagrscripts")
                          .Include("~/Content/Agrimanager/js/jquery-1.7.2.min.js")
                          .Include("~/Content/Agrimanager/js/jquery.tablesorter.js")
                          .Include("~/Content/Agrimanager/js/tablesort.js")
                          .Include("~/Content/Agrimanager/js/eye.js")
                          .Include("~/Content/Agrimanager/js/utils.js")
                          .Include("~/Content/Agrimanager/js/tabs.js")
                           .Include("~/Content/Agrimanager/js/jquery.ui.timepicker.js")
                          .Include("~/Content/Agrimanager/js/main-menu-drop-down.js")
                          .Include("~/Content/Agrimanager/js/Agricustomscripts.js"));

            //// Use the development version of Modernizr to develop with and learn from. Then, when you're
            //// ready for production, use the build tool at http://modernizr.com to pick only the tests you need.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));



            bundles.Add(new StyleBundle("~/Content/css")
                            .Include("~/Content/Agrimanager/styles/main.css")
                            .Include("~/Content/Agrimanager/styles/reset.css")
                            .Include("~/Content/Agrimanager/styles/Site.css")
                            .Include("~/Content/Agrimanager/styles/layout.css")
                              .Include("~/Content/Agrimanager/styles/jquery.ui.timepicker.css")
                            .Include("~/Content/Agrimanager/styles/tabs.css")
                            .Include("~/Content/Agrimanager/styles/main-menu.css")
                            .Include("~/Content/Agrimanager/styles/MenuMatic.css")
                            .Include("~/Content/Agrimanager/styles/tabs.css")
                            .Include("~/Content/Agrimanager/styles/tables.css"));
                           
                        

            bundles.Add(new StyleBundle("~/Content/themes/base/css").Include(
                        "~/Content/themes/base/jquery.ui.core.css",
                        "~/Content/themes/base/jquery.ui.resizable.css",
                        "~/Content/themes/base/jquery.ui.selectable.css",
                        "~/Content/themes/base/jquery.ui.accordion.css",
                        "~/Content/themes/base/jquery.ui.autocomplete.css",
                        "~/Content/themes/base/jquery.ui.button.css",
                        "~/Content/themes/base/jquery.ui.dialog.css",
                        "~/Content/themes/base/jquery.ui.slider.css",
                        "~/Content/themes/base/jquery.ui.tabs.css",
                        "~/Content/themes/base/jquery.ui.datepicker.css",
                        "~/Content/themes/base/jquery.ui.progressbar.css",
                        "~/Content/themes/base/jquery.ui.theme.css"));
        }
    }
}