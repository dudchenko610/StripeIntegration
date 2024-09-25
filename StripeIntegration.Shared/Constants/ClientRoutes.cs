namespace StripeIntegration.Shared.Constants;

public static partial class Constants
{
    public static class ClientRoutes
    {
        public const string SubscriptionsBase = "/subscriptions";
        public const string Subscriptions = "/stripe-example/subscriptions";
        
        public const string PrivacyPolicy = "/stripe-example/privacy-policy";
        public const string TermsOfUse = "/stripe-example/terms-of-use";
        public const string License = "/stripe-example/license";
        
        public const string ProfileBase = "/profile";
        public const string Profile = "/stripe-example/profile";
        public const string PaymentSuccessBase = "/payment-success";
        public const string PaymentSuccess = "/stripe-example/payment-success";
        public const string PaymentErrorBase = "/payment-error";
        public const string PaymentError = "/stripe-example/payment-error";
        
        public const string SignUpBase = "/sign-up";
        public const string SignUp = "/stripe-example/sign-up";
        public const string SignInBase = "/sign-in";
        public const string SignIn = "/stripe-example/sign-in";
        public const string SocialAuthBase = "/social-auth";
        public const string SocialAuth = "/stripe-example/social-auth";
        public const string ForgotPasswordBase = "/auth/forgot-password";
        public const string ForgotPassword = "/stripe-example/auth/forgot-password";
        public const string ChangePasswordBase = "/auth/change-password";
        public const string ChangePassword = "/stripe-example/auth/change-password";
        public const string ConfirmEmailBase = "/auth/confirm-email";
        public const string ConfirmEmail = "/stripe-example/auth/confirm-email";
    }
}