namespace NQG.TheoryBox.Owin.API
{
    using Nancy;

    public class HomeModule : NancyModule
    {
        public HomeModule()
            : base("/Api")
        {
            Get[""] = _ =>
            {
                var providers = this.interactiveDiagnostics
                    .AvailableDiagnostics
                    .Select(
                        p =>
                            new
                            {
                                p.Name,
                                p.Description,
                                Type = p.GetType().Name,
                                p.GetType().Namespace,
                                Assembly = p.GetType().Assembly.GetName().Name
                            })
                    .ToArray();

                return Response.AsJson(providers);
            };
        }
    }
}
