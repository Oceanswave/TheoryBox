namespace NQG.TheoryBox.Extensions
{
    using System.Collections.Specialized;
    using System.Linq;

    public static class NameValueCollectionExtensions
    {
        public static string ToQueryString(this NameValueCollection nvc, bool addQueryPrefix = true)
        {
            var array = (from key in nvc.AllKeys
                from value in nvc.GetValues(key)
                select string.Format("{0}={1}", key.UrlEncode(), value.UrlEncode()))
                .ToArray();

            if (addQueryPrefix)
                return "?" + string.Join("&", array);
            return string.Join("&", array);
        }
    }
}
