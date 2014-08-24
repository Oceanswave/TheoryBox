namespace NQG.TheoryBox.DomainModel
{
    using Newtonsoft.Json;

    public class CardPrinting
    {
        [JsonProperty("name")]
        public string Name
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

        [JsonProperty("block")]
        public string Block
        {
            get;
            set;
        }
    }
}
