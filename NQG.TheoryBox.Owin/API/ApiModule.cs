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
                return Response.AsJson("Hello, world");
            };

            Get["/metaverseid/{metaverseId}", true] = async (_, token) =>
            {
                string id = _.metaverseId;
                var card = await Repository.GetCard(id);
                return Response.AsJson(card);
            };

            Get["/logs/", true] = async (_, token) =>
            {
                var card = await Repository.GetAllCardLogs();
                return Response.AsJson(card);
            };
        }
    }
}
