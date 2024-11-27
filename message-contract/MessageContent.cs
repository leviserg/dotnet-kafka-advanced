using Newtonsoft.Json;

namespace message_contract
{
    public class MessageContent
    {
        [JsonRequired]
        [JsonProperty("id")]
        public required string id { get; set; }

        [JsonRequired]
        [JsonProperty("description")]
        public required string description { get; set; }

        [JsonProperty("price")]
        public decimal? price { get; set; }

        public MessageContent()
        {

        }

        public override string ToString()
        {
            return $"{id}\t{description}\t{Math.Round(price.Value, 2).ToString() ?? "null"}";
        }
    }
}
