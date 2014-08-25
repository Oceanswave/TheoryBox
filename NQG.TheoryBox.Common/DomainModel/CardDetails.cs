namespace NQG.TheoryBox.DomainModel
{
    using System;
    using System.Collections.Generic;
    using Newtonsoft.Json;

    public class CardDetails
    {
        public CardDetails()
        {
            OtherSets = new List<CardDetailsOtherSet>();
            Rulings = new List<CardDetailsRuling>();
        }

        [JsonProperty("name")]
        public string Name
        {
            get;
            set;
        }

        [JsonProperty("rarity")]
        public string Rarity
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

        [JsonProperty("manaCost")]
        public IList<string> ManaCost
        {
            get;
            set;
        }

        [JsonProperty("convertedManaCost")]
        public decimal? ConvertedManaCost
        {
            get;
            set;
        }

        [JsonProperty("types")]
        public IList<string> Types
        {
            get;
            set;
        }

        [JsonProperty("text")]
        public string Text
        {
            get;
            set;
        }

        [JsonProperty("flavorText")]
        public string FlavorText
        {
            get;
            set;
        }

        [JsonProperty("pt")]
        public CardPT PT
        {
            get;
            set;
        }

        [JsonProperty("artist")]
        public string Artist
        {
            get;
            set;
        }

        [JsonProperty("cardNumber")]
        public string CardNumber
        {
            get;
            set;
        }

        [JsonProperty("otherSets")]
        public IList<CardDetailsOtherSet> OtherSets
        {
            get;
            set;
        }

        [JsonProperty("rulings")]
        public IList<CardDetailsRuling> Rulings
        {
            get;
            set;
        }

        [JsonProperty("imageUrl")]
        public string ImageUrl
        {
            get;
            set;
        }

        [JsonProperty("originalUrl")]
        public string OriginalUrl
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
