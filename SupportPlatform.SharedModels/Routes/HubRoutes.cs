namespace SupportPlatform.SharedModels.Routes
{
    public static class HubRoutes
    {
        public const string BASE_ROUTE = "/hub";

        public static class Parser
        {
            public static string ROUTE = BASE_ROUTE + "/parsers";

            public static string DispatchTask = "DispatchTask";
        }

        public static class Search
        {
            public static string ROUTE = BASE_ROUTE + "/search";
        }

        public static class Notifications
        {
            public static string ROUTE = BASE_ROUTE + "/notifications";
        }

        public static class Issues
        {
            public static string ROUTE = BASE_ROUTE + "/issues";
        }
    }
}
