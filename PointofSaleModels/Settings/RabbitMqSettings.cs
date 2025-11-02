namespace PointofSaleModels.Settings
{
    public class RabbitMqSettings
    {
        public string HostName { get; set; } = string.Empty;
        public int Port { get; set; }
        public string UserName { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string? RequestQueueName { get; set; }
        public string? ResponseQueueName { get; set; }
    }
}
