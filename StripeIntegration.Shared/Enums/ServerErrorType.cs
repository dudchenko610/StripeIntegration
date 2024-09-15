namespace StripeIntegration.Shared.Enums;

public enum ServerErrorType
{
    General,
    
    UserNotConfirmed,
    UserNotFound,
    WrongLoginOrPassword,
    
    EmptyEmail,
    EmptyName,
    EmptyPassword,
    
    UserAlreadyExists,
    RegistrationFailed,
    
    PasswordsDoNotMatch,
    IncorrectEmail
}