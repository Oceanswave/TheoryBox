namespace NQG.TheoryBox.Owin
{
    using Nancy;
    using Nancy.Bootstrapper;
    using Nancy.TinyIoc;
    using Newtonsoft.Json;

    public class Bootstrapper : DefaultNancyBootstrapper
    {
        protected override IRootPathProvider RootPathProvider
        {
            get
            {
#if DEBUG
                return new DebugRootPathProvider();
#else
                return base.RootPathProvider;
#endif
            }
        }

        //protected override void ConfigureConventions(Nancy.Conventions.NancyConventions nancyConventions)
        //{

        //  Conventions.StaticContentsConventions.Add(
        //        StaticContentConventionBuilder.AddDirectory("BaristaFiddle/components")
        //    );

        //  base.ConfigureConventions(nancyConventions);
        //}

        protected override void ApplicationStartup(TinyIoCContainer container, IPipelines pipelines)
        {
            base.ApplicationStartup(container, pipelines);

            //BundleConfiguration.RegisterBundles();
        }

        protected override void ConfigureApplicationContainer(TinyIoCContainer container)
        {
            base.ConfigureApplicationContainer(container);
            container.Register<JsonSerializer>(new CustomJsonSerializer());
        }
    }
}
