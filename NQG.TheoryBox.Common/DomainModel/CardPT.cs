namespace NQG.TheoryBox.DomainModel
{
    using Newtonsoft.Json;

    public class CardPT
    {
        [JsonProperty("p")]
        public string P
        {
            get;
            set;
        }

        [JsonProperty("t")]
        public string T
        {
            get;
            set;
        }
    }
}
