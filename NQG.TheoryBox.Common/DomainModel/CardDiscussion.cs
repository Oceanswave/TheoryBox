namespace NQG.TheoryBox.DomainModel
{
    using System;
    using Newtonsoft.Json;

    public class CardDiscussion
    {
        [JsonProperty("currentRating")]
        public decimal CurrentRating
        {
            get;
            set;
        }

        [JsonProperty("totalVotes")]
        public int TotalVotes
        {
            get;
            set;
        }

        //TODO: Comments

        [JsonProperty("lastRetrieved")]
        public DateTime? LastRetrieved
        {
            get;
            set;
        }
    }
}
