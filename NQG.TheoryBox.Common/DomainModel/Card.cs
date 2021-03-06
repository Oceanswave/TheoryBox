﻿namespace NQG.TheoryBox.DomainModel
{
    using System;
    using Newtonsoft.Json;

    public class Card
    {
        public Card()
        {
            Details = new CardDetails();
            History = new CardHistory();
            Discussion = new CardDiscussion();
        }

        [JsonProperty("_id")]
        public string MultiverseId
        {
            get;
            set;
        }

        [JsonProperty("details")]
        public CardDetails Details
        {
            get;
            set;
        }

        [JsonProperty("history")]
        public CardHistory History
        {
            get;
            set;
        }

        [JsonProperty("discussion")]
        public CardDiscussion Discussion
        {
            get;
            set;
        }

        [JsonProperty("firstSeen")]
        public DateTime? FirstSeen
        {
            get;
            set;
        }
    }
}
