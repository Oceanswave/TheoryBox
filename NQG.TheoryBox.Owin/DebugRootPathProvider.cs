namespace NQG.TheoryBox.Owin
{
    using Nancy;
    using System.IO;

    public class DebugRootPathProvider : IRootPathProvider
    {
        public string GetRootPath()
        {
            //Hack to be able to edit files while the app is running in VS.
            var currentDirectory = Directory.GetCurrentDirectory();
            var di = new DirectoryInfo(currentDirectory);

            // ReSharper disable PossibleNullReferenceException
            return di.Parent.Parent.FullName + "\\NQG.TheoryBox.Owin\\";
            // ReSharper restore PossibleNullReferenceException
        }
    }
}
