namespace NQG.TheoryBox.DomainModel
{
    using Newtonsoft.Json;
    using System.Collections.Generic;

    public class CardDetailsOtherSet
    {
        [JsonProperty("metaverseId")]
        public string MetaverseId
        {
            get;
            set;
        }

        [JsonProperty("set")]
        public string Set
        {
            get;
            set;
        }
    }
}
