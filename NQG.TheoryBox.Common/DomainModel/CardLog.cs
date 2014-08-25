namespace NQG.TheoryBox.DomainModel
{
    using System.Collections.Generic;
    using Newtonsoft.Json;

    public class CardLog
    {
        public CardLog()
        {
            Errors = new List<string>();
        }

        [JsonProperty("id")]
        public string MultiverseId
        {
            get;
            set;
        }

        [JsonProperty("errors")]
        public IList<string> Errors
        {
            get;
            set;
        }
    }
}
