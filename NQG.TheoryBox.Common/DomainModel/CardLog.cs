namespace NQG.TheoryBox.DomainModel
{
    using System.Collections.Generic;
    using Newtonsoft.Json;

    public class CardLog
    {
        [JsonProperty("errors")]
        public IList<string> Errors
        {
            get;
            set;
        }
    }
}
