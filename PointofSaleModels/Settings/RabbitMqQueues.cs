namespace PointofSaleModels.Settings
{
    public static class RabbitMqQueues
    {
        public const string Services = "services";
        public const string DataService = "data";
        public const string OrderService = "order";
        public const string GatewayService = "gateway";
        public const string OrderStatusService = "orderstatus";
        public const string OrderNotificationService = "ordernotification";
        public const string ImportService = "import";
        public const string Request = "request";
        public const string Response = "response";

        public const string DataRequestQueue = $"{Services}.{DataService}.{Request}";
        public const string DataResponseQueue = $"{Services}.{DataService}.{Response}";
        public const string OrderRequestQueue = $"{Services}.{OrderService}.{Request}";
        public const string OrderResponseQueue = $"{Services}.{OrderService}.{Response}";
        public const string OrderStatusRequestQueue = $"{Services}.{OrderStatusService}.{Request}";
        public const string OrderStatusResponseQueue = $"{Services}.{OrderStatusService}.{Response}";
        public const string OrderNotificationRequestQueue = $"{Services}.{OrderNotificationService}.{Request}";
        public const string OrderNotificationResponseQueue = $"{Services}.{OrderNotificationService}.{Response}";
        public const string GatewayRequestQueue = $"{Services}.{GatewayService}.{Request}";
        public const string GatewayResponseQueue = $"{Services}.{GatewayService}.{Response}";
        public const string ImportResponseQueue = $"{Services}.{ImportService}.{Response}";
        public const string ImportRequestQueue = $"{Services}.{ImportService}.{Request}";
    }
}