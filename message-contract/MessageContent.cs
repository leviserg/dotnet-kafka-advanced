namespace message_contract
{
    public class MessageContent
    {
        public required string id { get; set; }
        public required string description { get; set; }
        public decimal? price { get; set; }

        public MessageContent()
        {

        }

        public override string ToString()
        {
            return $"{id}\t{description}\t{Math.Round(price.GetValueOrDefault(), 2).ToString()}";
        }
    }
}
