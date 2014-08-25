namespace NQG.TheoryBox.DomainModel
{
    using System;
    using Newtonsoft.Json;

    public class CardDetailsRuling
    {
        [JsonProperty("rulingDate")]
        public DateTime RulingDate
        {
            get;
            set;
        }

        [JsonProperty("rulingText")]
        public string RulingText
        {
            get;
            set;
        }
    }
}
