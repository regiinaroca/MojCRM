using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Optimization;

namespace MojCRM
{
    public class BundleConfig
    {
        // For more information on bundling, visit http://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                "~/Scripts/jquery-{version}.js",
                "~/Scripts/jquery-{version}.min.js",
                "~/Scripts/jquery-ui.js",
                "~/Scripts/jquery-ui.min.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                "~/Scripts/jquery.unobtrusive*",
                "~/Scripts/jquery.validate*"));

            bundles.Add(new ScriptBundle("~/bundles/knockout").Include(
                "~/Scripts/knockout-{version}.js",
                "~/Scripts/knockout.validation.js"));

            bundles.Add(new ScriptBundle("~/bundles/app").Include(
                "~/Scripts/sammy-{version}.js",
                "~/Scripts/app/common.js",
                "~/Scripts/app/app.datamodel.js",
                "~/Scripts/app/app.viewmodel.js",
                "~/Scripts/app/home.viewmodel.js",
                "~/Scripts/app/_run.js"));

            // Use the development version of Modernizr to develop with and learn from. Then, when you're
            // ready for production, use the build tool at http://modernizr.com to pick only the tests you need.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                "~/Scripts/modernizr-*"));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                "~/Scripts/bootstrap.js",
                "~/Scripts/bootstrap.min.js",
                "~/Scripts/respond.js",
                "~/Scripts/bootstrap-datepicker.min.js",
                "~/Scripts/bootstrap-tour.custom.js"));

            bundles.Add(new StyleBundle("~/Content/css").Include(
                 "~/Content/bootstrap.css",
                 "~/Content/bootstrap.min.css",
                 "~/Content/Site.css",
                 "~/Content/bootstrap-datepicker3.min.css",
                 "~/Content/font-awesome.css",
                 "~/Content/font-awesome.min.css",
                 "~/Content/main.css",
                 "~/Content/my-custom-styles.css"));

            bundles.Add(new ScriptBundle("~/bundles/scripts").Include(
                "~/Scripts/merscripts.js",
                "~/Scripts/king-common.js",
                "~/Scripts/jquery.slimscroll.min.js",
                "~/Scripts/jquery.easypiechart.min.js",
                "~/Scripts/raphael-2.1.0.min.js",
                "~/Scripts/raphael-2.1.0.min.js",
                "~/Scripts/jquery.flot.min.js",
                "~/Scripts/jquery.flot.pie.min.js",
                "~/Scripts/jquery.flot.resize.min.js",
                "~/Scripts/jquery.flot.time.min.js",
                "~/Scripts/jquery.flot.tooltip.min.js",
                "~/Scripts/jquery.sparkline.min.js",
                //"~/Scripts/dataTables.bootstrap.js",
                "~/Scripts/jquery.mapael.js",
                "~/Scripts/king-chart-stat.js",
                "~/Scripts/king-components.js"/*,
                "~/Scripts/king-table.js"*/));
        }
    }
}
