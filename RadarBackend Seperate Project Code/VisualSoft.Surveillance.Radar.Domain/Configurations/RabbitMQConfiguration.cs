namespace VisualSoft.Surveillance.Radar.Domain.Configurations
{
    public class RabbitMQConfiguration
    {
        public string ConnectionString { get; set; }
        public string ExchangeName { get; set; }
        public string ExchangeType { get; set; }
        public string ParserRoutingKey { get; set; }
        public string ParserQueueName { get; set; }
        public string AlaramEventRoutingKey { get; set; }
        public string AlaramEventQueueName { get; set; }
        //public string HumanDetectionQueueName { get; set; }
        public string HumanDetectionRoutingKey { get; set; }
    }
}
