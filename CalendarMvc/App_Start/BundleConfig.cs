using System.Web.Optimization;

namespace CalendarMvc {
    public class BundleConfig {
        public static void RegisterBundles(BundleCollection bundles) {
            const string jQueryVersion = "2.2.3";
            const string jsLibraryFolder = "~/Scripts/";
            const string jqueryFolder = jsLibraryFolder + "jQuery/jquery-";
            const string stylesFolder = "~/Content/css/";


            #region CDN Constants

            const string jQueryCdn = @"//code.jquery.com/jquery-" + jQueryVersion + ".min.js";
            //const string respondJsCDN = "http://cdnjs.cloudflare.com/ajax/libs/respond.js/1.4.2/respond.min.js"
            #endregion


            #region jQuery

            bundles.Add(new ScriptBundle("~/bundles/jquery", jQueryCdn)
                        .Include(jqueryFolder + jQueryVersion + ".js") //if no CDN
            );
            #endregion
            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        jsLibraryFolder + "jquery.validate*"));

            // Use the development version of Modernizr to develop with and learn from. Then, when you're
            // ready for production, use the build tool at http://modernizr.com to pick only the tests you need.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                         jsLibraryFolder + "modernizr-*"));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                jsLibraryFolder + "bootstrap.js",
                jsLibraryFolder + "moment.js",
                jsLibraryFolder + "bootstrap-datetimepicker.js",
                jsLibraryFolder + "kendo.all.min.js",
                jsLibraryFolder + "kendo.timezones.min.js",
                jsLibraryFolder + "jQueryExtend.js",
                jsLibraryFolder + "jQueryExtend.fn.js",
                jsLibraryFolder + "detect-browser.js",
                jsLibraryFolder + "kendo-override.js",
                jsLibraryFolder + "custom.js",
                jsLibraryFolder + "respond.js"
                ));
            bundles.Add(new StyleBundle(stylesFolder + "css").Include(
                    stylesFolder + "bootstrap.css",
                    stylesFolder + "kendo/kendo.common.css",
                    stylesFolder + "kendo/kendo.default.css",
                    //stylesFolder + "kendo/kendo.common-bootstrap.min.css",
                    //stylesFolder + "kendo/kendo.bootstrap.min.css",
                    //stylesFolder + "kendo/kendo.material.min.css",
                    stylesFolder + "bootstrap.css",

                    stylesFolder + "less-imports.css",
                    stylesFolder + "animate.min.css",
                    stylesFolder + "font-awesome.min.css",
                    stylesFolder + "calendar.css",
                    stylesFolder + "site.css",
                    stylesFolder + "header.css",

                    stylesFolder + "bootstrap-datetimepicker.css",
                    stylesFolder + "bootstrap-table.css",
                    stylesFolder + "bootstrap-select.css",
                    stylesFolder + "bootstrap-select-overrides.css",
                    stylesFolder + "bootstrap-tagsinput.css",
                    stylesFolder + "bootstrap-tagsinput-override.css",
                //stylesFolder + "ckedit-skin-bootstrap.css",

                    stylesFolder + "color-fonts.css",
                    stylesFolder + "override-mvc.css",
                    stylesFolder + "validator.css",
                    stylesFolder + "editor-templates.css",


                    stylesFolder + "seo-optimize.css",
                    stylesFolder + "front-developer.css",
                    stylesFolder + "footer-fixing.css",
                    stylesFolder + "utilities.css",
                    stylesFolder + "ie-fix.css",


                      stylesFolder + "site.css"));
            #region Configs

            bundles.UseCdn = true;
            BundleTable.EnableOptimizations = false;

            #endregion
        }

    }
}
