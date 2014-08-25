namespace NQG.TheoryBox.Owin.Home
{
    using Nancy;
    using NQG.TheoryBox.Owin.Extensions;
    using SquishIt.Framework;

    public class HomeModule : NancyModule
    {
        public HomeModule()
            : base("")
        {
            Get[""] = _ =>
            {
                var model = new
                {
                    head = Bundle.JavaScript().RenderNamed(BundleConfiguration.HeadScriptsBundleName),

                    styles = Bundle.Css().RenderNamed(BundleConfiguration.ComponentsStylesBundleName),
                    components = Bundle.JavaScript().RenderNamed(BundleConfiguration.ComponentsScriptBundleName),

                    appStyles = Bundle.Css().RenderNamed(BundleConfiguration.HomeAppStyleBundleName),
                    app = Bundle.JavaScript().RenderNamed(BundleConfiguration.HomeAppScriptBundleName),
                };

                return View["index.html", model];
            };

            Get["CardsByColor"] = _ =>
            {
                return View["cardsByColor.html"];
            };

            Get["CardDetails"] = _ =>
            {
                return View["cardDetails.html"];
            };

            //Script/CSS files

            Get["head.js"] = _ =>
            {
                var minified = Bundle.JavaScript().RenderCached(BundleConfiguration.HeadScriptsBundleName);
                return this.StringStreamResponse(minified, Configuration.Instance.JavascriptMimeType);
            };

            Get["styles.css"] = _ =>
            {
                var minified = Bundle.Css().RenderCached(BundleConfiguration.ComponentsStylesBundleName);
                return this.StringStreamResponse(minified, Configuration.Instance.CssMimeType);
            };

            Get["home.css"] = _ =>
            {
                var minified = Bundle.Css().RenderCached(BundleConfiguration.HomeAppStyleBundleName);
                return this.StringStreamResponse(minified, Configuration.Instance.CssMimeType);
            };

            Get["components.js"] = _ =>
            {
                var minified = Bundle.JavaScript().RenderCached(BundleConfiguration.ComponentsScriptBundleName);
                return this.StringStreamResponse(minified, Configuration.Instance.JavascriptMimeType);
            };

            Get["app.js"] = _ =>
            {
                var minified = Bundle.JavaScript().RenderCached(BundleConfiguration.HomeAppScriptBundleName);
                return this.StringStreamResponse(minified, Configuration.Instance.JavascriptMimeType);
            };

            //Components FFA

            Get["/styles/font-awesome-4.0.3/fonts/{fileName*}"] = parameters =>
            {
                var fileName = ((string)parameters.fileName).Replace("/", "\\");
                fileName = fileName.Replace("..", "");
                return this.RootFileResponse("styles\\font-awesome-4.0.3\\fonts\\" + fileName);
            };

            Get["/content/{fileName*}"] = parameters =>
            {
                var fileName = ((string)parameters.fileName).Replace("/", "\\");
                fileName = fileName.Replace("..", "");
                return this.RootFileResponse("content\\" + fileName);
            };

            Get["/styles/bootstrap/fonts/{fileName*}"] = parameters =>
            {
                var fileName = ((string)parameters.fileName).Replace("/", "\\");
                fileName = fileName.Replace("..", "");
                return this.RootFileResponse("styles\\bootstrap\\fonts\\" + fileName);
            };
        }
    }
}
