namespace NQG.TheoryBox.DomainModel
{
    using Newtonsoft.Json;

    public class CardRestriction
    {
        [JsonProperty("format")]
        public string Format
        {
            get;
            set;
        }

        [JsonProperty("legality")]
        public string Legality
        {
            get;
            set;
        }
    }
}
