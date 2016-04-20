using System.Web;
using System.Web.Optimization;

namespace RDVMedicaux.App_Start
{
    /// <summary>
    /// Classe de configuration des bundles css
    /// </summary>
    public class BundleConfig
    {
        /// <summary>
        /// Défini les bundles
        /// </summary>
        /// <param name="bundles">collection de bundles</param>
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/respond").Include(
                        "~/Scripts/Libs/shim/respond.js"));

            bundles.Add(new StyleBundle("~/Content/css").Include(
                      "~/Content/bootstrap/css/bootstrap.css",
                      "~/Content/bootstrap/css/bootstrap-datepicker.css",
                      "~/Content/bootstrap/css/bootstrap-datepicker3.css",
                      "~/Content/css/site.css",
                      "~/Content/css/override.css"));

            //bundles.Add(new StyleBundle("~/Content/css/logon").Include(
            //          "~/Content/bootstrap/css/bootstrap.css",
            //          "~/Content/css/logon.css"));

            // Set EnableOptimizations to false for debugging. For more information,
            // visit http://go.microsoft.com/fwlink/?LinkId=301862
            BundleTable.EnableOptimizations = false;
        }
    }
}
