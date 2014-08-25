namespace NQG.TheoryBox.Owin.Extensions
{
    using Nancy;
    using Nancy.Extensions;
    using Nancy.Responses;
    using System;
    using System.IO;
    using System.Text;

    public static class NancyModuleExtensions
    {
        public static GenericFileResponse RootFileResponse<T>(this T module, string filePath, string contentType = null)
          where T : NancyModule
        {
            return String.IsNullOrWhiteSpace(contentType)
              ? new GenericFileResponse(filePath)
              : new GenericFileResponse(filePath, contentType);
        }

        public static GenericFileResponse ModuleFileResponse<T>(this T module, string filePath, string contentType = null)
          where T : NancyModule
        {
            return String.IsNullOrWhiteSpace(contentType)
              ? new GenericFileResponse(Path.Combine(module.GetModuleName(), filePath))
              : new GenericFileResponse(Path.Combine(module.GetModuleName(), filePath), contentType);
        }

        public static StreamResponse StringStreamResponse<T>(this T module, string content, string contentType = null)
          where T : NancyModule
        {
            return String.IsNullOrWhiteSpace(contentType)
                ? new StreamResponse(
                    () => new MemoryStream(Encoding.UTF8.GetBytes(content)),
                    "application/octet-stream")
                : new StreamResponse(
                    () => new MemoryStream(Encoding.UTF8.GetBytes(content)),
                    contentType);
        }

        public static void GetAny<T>(this T module, string basePath, params string[] filePaths)
          where T : NancyModule
        {
            foreach (var filePath in filePaths)
            {
                module.Get[filePath] = parameters => module.ModuleFileResponse(basePath + (string)parameters.fileName);
            }
        }
    }
}
