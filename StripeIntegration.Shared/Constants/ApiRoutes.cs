namespace StripeIntegration.Shared.Constants;

public static partial class Constants
{
    public static class ApiRoutes
    {
        public const string AppTypeKey = "AppType";
        
        public const string SUBSCRIPTION = "api/subscription";
        public const string ACCOUNT = "api/account";
        public const string APPLICATION = "api/application";

        public const string CHECK_LICENSE_KEY = "check-key";

        public const string SIGN_IN = "sign-in";
        public const string SIGN_UP = "sign-up";
        public const string CONFIRM_EMAIL = "confirm-email";
        public const string FORGOT_PASSOWRD = "forgot-password";
        public const string GET_USER = "get-user";
        public const string UPDATE_USER = "update-user";
        public const string CHANGE_PASSWORD = "change-password";

        public const string UPDATE_TOKENS = "update-tokens";

        public const string GET_PRODUCTS = "get-products";
        public const string GET_PAYMENT_SESSION = "get-payment-session";
        public const string GET_RENEW_PAYMENT_SESSION = "get-renew-payment-session";
        public const string GET_SUBSCRIPTION = "get-subscription";
        public const string CHANGE_SUBSCRIPTION = "change-subscription";
        public const string CANCEL_SUBSCRIPTION = "cancel-subscription";

        public const string GET_APPLICATIONS = "get-applications";
        public const string ADD_APPLICATION = "add-application";
        public const string UPDATE_APPLICATION = "update-application";
        public const string REMOVE_APPLICATION = "remove-application";
    }
}