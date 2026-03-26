namespace PointofSaleModels.Settings
{
    public static class RabbitMqQueues
    {
        public const string Services = "services";
        public const string MenuService = "menu";
        public const string SettingService = "setting";
        public const string OrderHistoryService = "orderhistory";
        public const string OrderService = "order";
        public const string GatewayService = "gateway";
        public const string OrderUpdateService = "orderstatus";
        public const string ClientNotificationService = "clientnotification";
        public const string PushNotificationService = "pushnotification";
        public const string ImportService = "import";
        public const string ExportService = "export";
        public const string CustomerOrderHistoryService = "customerorderhistory";
        public const string Request = "request";
        public const string Response = "response";

        public const string OrderRequestQueue = $"{Services}.{OrderService}.{Request}";
        public const string OrderResponseQueue = $"{Services}.{OrderService}.{Response}";
        public const string OrderUpdateRequestQueue = $"{Services}.{OrderUpdateService}.{Request}";
        public const string OrderUpdateResponseQueue = $"{Services}.{OrderUpdateService}.{Response}";
        public const string ClientNotificationRequestQueue = $"{Services}.{ClientNotificationService}.{Request}";
        public const string ClientNotificationResponseQueue = $"{Services}.{ClientNotificationService}.{Response}";
        public const string ClientNotificationGatewayResponse = $"{Services}.{ClientNotificationService}.{GatewayService}.{Response}";
        public const string GatewayRequestQueue = $"{Services}.{GatewayService}.{Request}";
        public const string GatewayResponseQueue = $"{Services}.{GatewayService}.{Response}";
        public const string ImportResponseQueue = $"{Services}.{ImportService}.{Response}";
        public const string ImportRequestQueue = $"{Services}.{ImportService}.{Request}";
        public const string ExportRequestQueue = $"{Services}.{ExportService}.{Request}";
        public const string SettingRequestQueue = $"{Services}.{SettingService}.{Request}";
        public const string SettingResponseQueue = $"{Services}.{SettingService}.{Response}";
        public const string MenuRequestQueue = $"{Services}.{MenuService}.{Request}";
        public const string MenuResponseQueue = $"{Services}.{MenuService}.{Response}";
        public const string OrderHistoryRequestQueue = $"{Services}.{OrderHistoryService}.{Request}";
        public const string OrderHistoryResponseQueue = $"{Services}.{OrderHistoryService}.{Response}";
        public const string PushNotificationRequestQueue = $"{Services}.{PushNotificationService}.{Request}";
        public const string CustomerOrderHistoryRequestQueue = $"{Services}.{CustomerOrderHistoryService}.{Request}";
        public const string CustomerOrderHistoryResponseQueue = $"{Services}.{CustomerOrderHistoryService}.{Response}";
    }
}