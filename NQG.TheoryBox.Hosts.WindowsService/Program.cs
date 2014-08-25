namespace NQG.TheoryBox.Host.WindowsService
{
    using Topshelf;

    class Program
    {
        static void Main(string[] args)
        {
            HostFactory.Run(x =>
            {
                x.Service<TheoryBoxService>();
                x.RunAsLocalSystem();

                x.SetDescription("Hosts TheoryBox as a Windows Service");
                x.SetDisplayName("No Quarter Gaming - TheoryBox Service");
                x.SetServiceName("NQG.Host.WindowsService");
            });
        }
    }
}
