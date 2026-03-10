using PointofSaleModels.Application;

namespace PointofSaleModels.ServicePayloads
{
    public class ClientNotificationServicePayload : ServicePayload
    {
        public ClientNotificationServicePayload() : base()
        {

        }

        public ClientNotificationServicePayload(ClientNotificationServicePayload payload) : base(payload)
        {
        }

        public ClientNotificationServicePayload(ServicePayload payload) : base(payload)
        {
        }

        public ClientNotificationType NotificationType { get; set; }

        public string Payload { get; set; }

        public T? GetPayload<T>()
        {
            return System.Text.Json.JsonSerializer.Deserialize<T>(Payload); 
        }

        public List<ClientNotificationIdentity> NotificationKeys { get; set; } = [];
    }

    public enum ClientNotificationType
    {
        NewOrder,
        OrderStatusUpdate
    }

    public abstract class ClientNotificationIdentity
    {
        public string ClientId { get; set; }
    }

    public class BranchNotification : ClientNotificationIdentity
    {
        public int UserId { get; set; }
        public string ClientId { get; set; }
    }

    public class UserNotification : ClientNotificationIdentity
    {
        public int BranchId { get; set; }
    }
}
