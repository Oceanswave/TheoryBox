namespace NQG.TheoryBox.Owin
{
    using SquishIt.Framework;

    public static class BundleConfiguration
    {
        public const string HeadScriptsBundleName = "scripts/head";

        public const string ComponentsStylesBundleName = "styles/components";
        public const string ComponentsScriptBundleName = "scripts/components";

        public const string HomeAppStyleBundleName = "styles/apps/home";
        public const string HomeAppScriptBundleName = "scripts/apps/home";

        public static void RegisterBundles()
        {
            RegisterStyleBundles();
            RegisterScriptBundles();
        }

        private static void RegisterStyleBundles()
        {
            Bundle.Css()
                .Add("~/Styles/bootstrap-3.2.0-dist/css/bootstrap.css")
                .Add("~/Styles/font-awesome-4.1.0/css/font-awesome.css")
                .Add("~/Styles/site.css")
#if DEBUG
.WithMinifier<SquishIt.Framework.Minifiers.CSS.NullMinifier>()
#else
                .WithMinifier<SquishIt.Framework.Minifiers.CSS.MsMinifier>()
#endif
.AsCached(ComponentsStylesBundleName, "~/styles.css");

            Bundle.Css()
                .Add("~/Home/home.css")
#if DEBUG
.WithMinifier<SquishIt.Framework.Minifiers.CSS.NullMinifier>()
#else
                .WithMinifier<SquishIt.Framework.Minifiers.CSS.MsMinifier>()
#endif
.AsCached(HomeAppStyleBundleName, "~/home.css");
        }

        private static void RegisterScriptBundles()
        {
            Bundle.JavaScript()
                .Add("~/Components/modernizr/modernizr.custom.56232.js")
#if DEBUG
.WithMinifier<SquishIt.Framework.Minifiers.JavaScript.NullMinifier>()
#else
                .WithMinifier<SquishIt.Framework.Minifiers.JavaScript.MsMinifier>()
#endif
.AsCached(HeadScriptsBundleName, "~/head.js");


            Bundle.JavaScript()
                .Add("~/Components/jquery/jquery-2.1.1.js")
                .Add("~/Components/auth0/auth0-widget-5.js")
                .Add("~/Components/angular/angular.js")
                .Add("~/Components/angular/angular-cookies.js")
                .Add("~/Components/angular/angular-route.js")
                .Add("~/Components/angular/angular-sanitize.js")
                .Add("~/Components/angular-bootstrap/ui-bootstrap-tpls-0.11.0.js")
                .Add("~/Components/angular-ui-router/angular-ui-router.js")
                .Add("~/Components/auth0/auth0-angular-1.js")
#if DEBUG
.WithMinifier<SquishIt.Framework.Minifiers.JavaScript.NullMinifier>()
#else
                .WithMinifier<SquishIt.Framework.Minifiers.JavaScript.MsMinifier>()
#endif
.AsCached(ComponentsScriptBundleName, "~/components.js");

            Bundle.JavaScript()
                .Add("~/Home/app.js")
                .Add("~/Home/homeCtrl.js")
                .Add("~/Home/cardDetailsCtrl.js")
                .Add("~/Home/cardsByColorCtrl.js")
#if DEBUG
.WithMinifier<SquishIt.Framework.Minifiers.JavaScript.NullMinifier>()
#else
                .WithMinifier<SquishIt.Framework.Minifiers.JavaScript.MsMinifier>()
#endif
.AsCached(HomeAppScriptBundleName, "~/app.js");
        }
    }
}
