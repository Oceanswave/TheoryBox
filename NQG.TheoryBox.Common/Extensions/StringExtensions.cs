namespace NQG.TheoryBox.Extensions
{
    using System;
    using System.Globalization;
    using System.Text;

    public static class StringExtensions
    {
        /// <summary>
        /// UrlEncodes a string without the requirement for System.Web
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static string UrlEncode(this string text)
        {
            // Sytem.Uri provides reliable parsing
            return Uri.EscapeDataString(text);
        }

        /// <summary>
        /// UrlDecodes a string without requiring System.Web
        /// </summary>
        /// <param name="text">String to decode.</param>
        /// <returns>decoded string</returns>
        public static string UrlDecode(this string text)
        {
            // pre-process for + sign space formatting since System.Uri doesn't handle it
            // plus literals are encoded as %2b normally so this should be safe
            text = text.Replace("+", " ");
            return Uri.UnescapeDataString(text);
        }

        /// <summary>
        /// Retrieves a value by key from a UrlEncoded string.
        /// </summary>
        /// <param name="urlEncoded">UrlEncoded String</param>
        /// <param name="key">Key to retrieve value for</param>
        /// <returns>returns the value or "" if the key is not found or the value is blank</returns>
        public static string GetUrlEncodedKey(this string urlEncoded, string key)
        {
            urlEncoded = "&" + urlEncoded + "&";

            var index = urlEncoded.IndexOf("&" + key + "=", StringComparison.OrdinalIgnoreCase);
            if (index < 0)
                return "";

            var lnStart = index + 2 + key.Length;

            var index2 = urlEncoded.IndexOf("&", lnStart, StringComparison.Ordinal);
            return index2 < 0 
                ? ""
                : UrlDecode(urlEncoded.Substring(lnStart, index2 - lnStart));
        }

        /// <summary>
        /// HTML-encodes a string and returns the encoded string.
        /// </summary>
        /// <param name="text">The text string to encode. </param>
        /// <returns>The HTML-encoded text.</returns>
        public static string HtmlEncode(this string text)
        {
            if (text == null)
                return null;

            var sb = new StringBuilder(text.Length);

            var len = text.Length;
            for (var i = 0; i < len; i++)
            {
                switch (text[i])
                {

                    case '<':
                        sb.Append("&lt;");
                        break;
                    case '>':
                        sb.Append("&gt;");
                        break;
                    case '"':
                        sb.Append("&quot;");
                        break;
                    case '&':
                        sb.Append("&amp;");
                        break;
                    default:
                        if (text[i] > 159)
                        {
                            // decimal numeric entity
                            sb.Append("&#");
                            sb.Append(((int)text[i]).ToString(CultureInfo.InvariantCulture));
                            sb.Append(";");
                        }
                        else
                            sb.Append(text[i]);
                        break;
                }
            }
            return sb.ToString();
        }
    }
}
