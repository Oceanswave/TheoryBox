namespace NQG.TheoryBox.Crawler
{
    using Topshelf;

    class Program
    {
        private static void Main(string[] args)
        {
            HostFactory.Run(x =>
            {
                x.Service<TheoryBoxCrawlerService>();

                x.RunAsPrompt();

                x.SetDescription("Crawls the Gatherer for Content");
                x.SetDisplayName("No Quarter Gaming - TheoryBox Crawler");
                x.SetServiceName("NQG.TheoryBoxCrawler");
                x.StartAutomatically();
            });

        }
    }
}
