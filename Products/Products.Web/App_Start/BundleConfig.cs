namespace Products.Web
{
    using System.Collections.Generic;
    using System.Web.Optimization;
    using Helpers;
    public class BundleConfig
    {
        public static void RegisterBundles(BundleCollection bundles)
        {

            bundles.UseCdn = true;

            IList<Bundle> bundleBuildList = new List<Bundle>();

            // Scripts
            bundleBuildList.Add(new ScriptBundle("~/js/jquery-all", $"{WebClientHelper.Instance.BaseUrl}/js/jquery-all"));

            bundleBuildList.Add(new ScriptBundle("~/js/core", $"{WebClientHelper.Instance.BaseUrl}/js/core"));

            bundleBuildList.Add(new ScriptBundle("~/js/utils", $"{WebClientHelper.Instance.BaseUrl}/js/utils"));

            bundleBuildList.Add(new ScriptBundle("~/js/common/bank-details-checked-validation", $"{WebClientHelper.Instance.BaseUrl}/js/common/bank-details-checked-validation"));

            bundleBuildList.Add(new ScriptBundle("~/js/common/address", $"{WebClientHelper.Instance.BaseUrl}/js/common/address"));

            bundleBuildList.Add(new ScriptBundle("~/js/common/print-summary", $"{WebClientHelper.Instance.BaseUrl}/js/common/print-summary"));

            bundleBuildList.Add(new ScriptBundle("~/js/products", $"{WebClientHelper.Instance.BaseUrl}/js/products"));

            bundleBuildList.Add(new ScriptBundle("~/js/products/summary", $"{WebClientHelper.Instance.BaseUrl}/js/products/summary"));
            
            bundleBuildList.Add(new ScriptBundle("~/js/products/personal-details", $"{WebClientHelper.Instance.BaseUrl}/js/products/personal-details"));

            bundleBuildList.Add(new ScriptBundle("~/js/dd-mandate", $"{WebClientHelper.Instance.BaseUrl}/js/dd-mandate"));



            //Broadband

            bundleBuildList.Add(new ScriptBundle("~/js/products/broadband/transfer-number", $"{WebClientHelper.Instance.BaseUrl}/js/products/broadband/transfer-number"));

            bundleBuildList.Add(new ScriptBundle("~/js/products/broadband/available-packages", $"{WebClientHelper.Instance.BaseUrl}/js/products/broadband/available-packages"));

            bundleBuildList.Add(new ScriptBundle("~/js/products/broadband/line-speed", $"{WebClientHelper.Instance.BaseUrl}/js/products/broadband/line-speed"));

            bundleBuildList.Add(new ScriptBundle("~/js/products/broadband/select-address", $"{WebClientHelper.Instance.BaseUrl}/js/products/broadband/select-address"));

            bundleBuildList.Add(new ScriptBundle("~/js/products/broadband/available-tariffs", $"{WebClientHelper.Instance.BaseUrl}/js/products/broadband/available-tariffs"));

            // Energy

            bundleBuildList.Add(new ScriptBundle("~/js/products/energy/usage", $"{WebClientHelper.Instance.BaseUrl}/js/products/energy/usage"));

            bundleBuildList.Add(new ScriptBundle("~/js/products/energy/our-prices", $"{WebClientHelper.Instance.BaseUrl}/js/products/energy/our-prices"));
            bundleBuildList.Add(new ScriptBundle("~/js/products/energy/signup", $"{WebClientHelper.Instance.BaseUrl}/js/products/energy/signup"));
            bundleBuildList.Add(new ScriptBundle("~/js/products/energy/basket", $"{WebClientHelper.Instance.BaseUrl}/js/products/energy/basket"));
            bundleBuildList.Add(new ScriptBundle("~/js/products/energy/extras", $"{WebClientHelper.Instance.BaseUrl}/js/products/energy/extras"));
            bundleBuildList.Add(new ScriptBundle("~/js/products/energy/upgrades", $"{WebClientHelper.Instance.BaseUrl}/js/products/energy/upgrades"));
            bundleBuildList.Add(new ScriptBundle("~/js/products/energy/direct-debit-amount-update", $"{WebClientHelper.Instance.BaseUrl}/js/products/energy/direct-debit-amount-update"));

            // HomeServices

            bundleBuildList.Add(new ScriptBundle("~/js/products/home-services/cover-details", $"{WebClientHelper.Instance.BaseUrl}/js/products/home-services/cover-details"));
            
            // Tariffs

            bundleBuildList.Add(new ScriptBundle("~/js/products/tariffs/available-tariffs", $"{WebClientHelper.Instance.BaseUrl}/js/products/tariffs/available-tariffs"));

            bundleBuildList.Add(new ScriptBundle("~/js/products/tariffs/available-renewals", $"{WebClientHelper.Instance.BaseUrl}/js/products/tariffs/available-renewals"));

            bundleBuildList.Add(new ScriptBundle("~/js/products/tariffs/summary", $"{WebClientHelper.Instance.BaseUrl}/js/products/tariffs/summary"));

            bundleBuildList.Add(new ScriptBundle("~/js/products/tariffs/spinner", $"{WebClientHelper.Instance.BaseUrl}/js/products/tariffs/spinner"));

            bundleBuildList.Add(new ScriptBundle("~/js/products/energy/tab", $"{WebClientHelper.Instance.BaseUrl}/js/products/energy/tab"));

            // Styles
            bundleBuildList.Add(new StyleBundle("~/css/core", $"{WebClientHelper.Instance.BaseUrl}/css/core"));

            bundleBuildList.Add(new StyleBundle("~/css/tariffs", $"{WebClientHelper.Instance.BaseUrl}/css/tariffs"));

            bundleBuildList.Add(new StyleBundle("~/css/recaptcha", $"{WebClientHelper.Instance.BaseUrl}/css/recaptcha"));

            bundleBuildList.Add(new StyleBundle("~/Content/css/products/print", $"{WebClientHelper.Instance.BaseUrl}/Content/css/products/print"));

            bundleBuildList.Add(new StyleBundle("~/Content/css/products/mandate", $"{WebClientHelper.Instance.BaseUrl}/Content/css/products/mandate"));



            // Do not add bundles below this point

            foreach (Bundle bundle in bundleBuildList)
            {
                if (bundle.GetType() == typeof(ScriptBundle))
                {
                    bundles.Add(new ScriptBundle(bundle.Path, string.Format(bundle.CdnPath + "?v={0}", WebClientHelper.Instance.Version)));
                }
                if (bundle.GetType() == typeof(StyleBundle))
                {
                    bundles.Add(new StyleBundle(bundle.Path, string.Format(bundle.CdnPath + "?v={0}", WebClientHelper.Instance.Version)));
                }
            }

            BundleTable.EnableOptimizations = true;
        }
    }
}
