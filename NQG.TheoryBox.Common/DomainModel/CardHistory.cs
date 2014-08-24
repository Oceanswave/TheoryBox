namespace NQG.TheoryBox.DomainModel
{
    using System;
    using System.Collections.Generic;
    using Newtonsoft.Json;

    public class CardHistory
    {
        [JsonProperty("printings")]
        public IList<CardPrinting> Printings
        {
            get;
            set;
        }

        [JsonProperty("restrictions")]
        public IList<CardRestriction> Restrictions
        {
            get;
            set;
        }

        [JsonProperty("lastRetrieved")]
        public DateTime? LastRetrieved
        {
            get;
            set;
        }
    }
}
