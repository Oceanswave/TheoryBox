namespace NQG.TheoryBox.DomainModel
{
    using System;
    using Newtonsoft.Json;
    using NQG.TheoryBox.Extensions;

    public class CompactCardInfo
    {
        [JsonProperty("name")]
        public string Name
        {
            get;
            set;
        }

        [JsonProperty("url")]
        public string Url
        {
            get;
            set;
        }

        [JsonIgnore]
        public Uri BaseUri
        {
            get;
            set;
        }

        [JsonIgnore]
        public string MultiVerseId
        {
            get
            {
                return Url.Substring(Url.IndexOf('?') + 1).GetUrlEncodedKey("multiverseid");
            }
        }

        [JsonProperty("absoluteUrl")]
        public Uri AbsoluteUrl
        {
            get
            {
                Uri absUrl;
                return Uri.TryCreate(BaseUri, Url, out absUrl) ? absUrl : null;
            }
        }
    }
}
