namespace SupportPlatform.SharedModels.Routes
{
    public static class APIRoutes
    {
        public const string API_ROUTE = "/api";

        public static class V1
        {
            public const string V1Base = API_ROUTE + "/v1/";

            public static class Auth
            {
                public const string Base = V1Base + "auth/";

                public const string Login = Base + "login";
                public const string Register = Base + "register";
                public const string RestorePassword = Base + "restorePassword";
                public const string ResetPassword = Base + "resetPassword";
            }
            public static class User
            {
                public const string Base = V1Base + "user/";

                public const string GetId = Base + "id";
                public const string GetInfo = Base + "info";
                public const string GetOtherUserInfo = Base + "other/info/{userId}";
                public const string GetSettings = Base + "settings";
                public const string GetSubscription = Base + "subscription";
                public const string Update = Base + "update";
                public const string UpdatePassword = Base + "update/password";
            }
            public static class Requests
            {
                public const string Base = V1Base + "support/";

                //public const string GetEntities = Base + "entities"; // get companies or employees with filters
                public const string GetRequests = Base + "requests";

                public const string CreateOrUpdateRequest = Base + "requests/createOrUpdate";
            }
        }
    }
}
