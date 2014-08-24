namespace NQG.TheoryBox.Owin
{
    using global::Owin;
    using Nancy;
    using Nancy.Owin;

    public static class AppBuilderExtensions
    {
        public static IAppBuilder UseBaristaLabsWeb(this IAppBuilder app, IKernel kernel)
        {
            return app.UseNancy(options => options.PassThroughWhenStatusCodesAre(HttpStatusCode.NotFound,
              HttpStatusCode.InternalServerError));

            //kernel.Bind<FileSystemsManager>().ToSelf();
            //app.Map("/_", adminApp =>
            //{
            //    var config = new HttpConfiguration();
            //    adminApp.UseWebApi(config);
            //    adminApp.Map("/account", accountApp =>
            //    {

            //    });

            //    adminApp.Map("/baristafiddle", baristaFiddleApp =>
            //    {
            //        baristaFiddleApp.UseBaristaLabsFiddle();
            //    });

            //    adminApp.Use(Admin);
            //});

            //app.UseErrorPage()
            //   .UseAppEngine(kernel);

            //return app;
        }
    }
}
