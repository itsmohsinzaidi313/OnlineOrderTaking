namespace PointofSaleModels.Settings
{
    public static class RabbitMqQueues
    {
        public const string Services = "services";
        public const string JwtService = "jwt";
        public const string DataService = "data";
        public const string OrderService = "order";
        public const string GatewayService = "gateway";
        public const string LoginService = "login";
        public const string Request = "request";
        public const string Response = "response";

        public const string JwtRequestQueue = $"{Services}.{JwtService}.{Request}";
        public const string JwtResponseQueue = $"{Services}.{JwtService}.{Response}";
        public const string DataRequestQueue = $"{Services}.{DataService}.{Request}";
        public const string DataResponseQueue = $"{Services}.{DataService}.{Response}";
        public const string OrderRequestQueue = $"{Services}.{OrderService}.{Request}";
        public const string OrderResponseQueue = $"{Services}.{OrderService}.{Response}";
        public const string LoginRequestQueue = $"{Services}.{LoginService}.{Request}";
        public const string LoginResponseQueue = $"{Services}.{LoginService}.{Response}";
        public const string JwtDecryptQueue = $"{GatewayService}.{JwtService}.{Request}.decrypt";
    }
}