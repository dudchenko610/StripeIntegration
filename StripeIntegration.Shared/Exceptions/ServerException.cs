using System.Net;
using StripeIntegration.Shared.Enums;

namespace StripeIntegration.Shared.Exceptions;

public class ServerException : Exception
{
    public HttpStatusCode Code { get; set; }
    public List<string> ErrorMessages { get; set; } = new();
    public ServerErrorType ServerErrorType { get; set; }

    public ServerException() {}
    
    public ServerException(
        string error,
        ServerErrorType serverErrorType = ServerErrorType.General,
        HttpStatusCode errorCode = HttpStatusCode.BadRequest)
    {
        Code = errorCode;
        ServerErrorType = serverErrorType;
        ErrorMessages.Add(error);
    }

    public ServerException(
        List<string> errorMessages,
        ServerErrorType serverErrorType = ServerErrorType.General,
        HttpStatusCode errorCode = HttpStatusCode.BadRequest)
    {
        Code = errorCode;
        ServerErrorType = serverErrorType;
        ErrorMessages.AddRange(errorMessages);
    }
}