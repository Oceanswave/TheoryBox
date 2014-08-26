namespace NQG.TheoryBox
{
    using System;
    using System.Configuration;
    using MyCouch;
    using MyCouch.Cloudant;
    using MyCouch.EntitySchemes.Reflections;
    using MyCouch.Net;
    using NQG.TheoryBox.EntitySchemes;

    public static partial class Repository
    {
        public static Uri DbUri
        {
            get
            {
                return GetDbUri();
            }
        }

        public static IMyCouchCloudantClient GetDbClient(string databaseName = null)
        {
            var endpoint = ConfigurationManager.AppSettings.Get("TheoryBox_EndpointUrl");
            var authKey = ConfigurationManager.AppSettings.Get("TheoryBox_AuthorizationKey");
            var password = ConfigurationManager.AppSettings.Get("TheoryBox_Password");

            var uriBuilder = new MyCouchUriBuilder(endpoint)
                .SetBasicCredentials(authKey, password);

            if (databaseName != null)
                uriBuilder.SetDbName(databaseName);

            var uri = uriBuilder.Build();
            var bs = new MyCouchCloudantClientBootstrapper
            {
                EntityReflectorFn = () => new JsonNetEntityReflector(new IlDynamicPropertyFactory()),
            };

            var client = new MyCouchCloudantClient(new DbClientConnection(uri), bs);
            return client;
        }

        public static Uri GetDbUri(string databaseName = null)
        {
            var endpoint = ConfigurationManager.AppSettings.Get("TheoryBox_EndpointUrl");
            var authKey = ConfigurationManager.AppSettings.Get("TheoryBox_AuthorizationKey");
            var password = ConfigurationManager.AppSettings.Get("TheoryBox_Password");

            var uriBuilder = new MyCouchUriBuilder(endpoint)
                .SetBasicCredentials(authKey, password);

            if (databaseName != null)
                uriBuilder.SetDbName(databaseName);

            return uriBuilder.Build();
        }
    }
}
