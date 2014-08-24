namespace NQG.TheoryBox.Crawler
{
    using Topshelf;

    class Program
    {
        private static void Main(string[] args)
        {
            HostFactory.Run(x =>
            {
                x.Service<TheoryBoxCrawler>(s =>
                {
                    s.ConstructUsing(name => new TheoryBoxCrawler());
                    s.WhenStarted(tc => tc.Start());
                    s.WhenStopped(tc => tc.Stop());
                });

                x.RunAsPrompt();

                x.SetDescription("Crawls the Gatherer for Content");
                x.SetDisplayName("No Quarter Gaming - TheoryBox Crawler");
                x.SetServiceName("NQG.TheoryBoxCrawler");
                x.StartAutomatically();
            });

        }
    }
}
