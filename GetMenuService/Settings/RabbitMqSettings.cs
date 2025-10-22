// Settings/RabbitMqSettings.cs
namespace GetMenuService.Settings
{
    public class RabbitMqSettings
    {
        public string HostName { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;

        // Standardized with GatewayService
        public string? RequestQueueName { get; set; }
        public string? ResponseQueueName { get; set; }
    }
}
