namespace GGHN.DigitalLearning.Application.Exceptions;

public abstract class AuthException : Exception
{
    public string Type { get; }
    public string Title { get; }
    public int Status { get; }

    protected AuthException(string type, string title, int status, string detail)
        : base(detail)
    {
        Type = type;
        Title = title;
        Status = status;
    }
}

public class InvalidCredentialsException : AuthException
{
    public InvalidCredentialsException()
        : base(
            type: "https://tools.ietf.org/html/rfc7235#section-3.1",
            title: "Invalid credentials",
            status: 401,
            detail: "The email or password you entered is incorrect. Please try again.") { }
}

public class AccountLockoutException : AuthException
{
    public DateTimeOffset? LockoutEnd { get; }

    public AccountLockoutException(DateTimeOffset? lockoutEnd)
        : base(
            type: "https://tools.ietf.org/html/rfc7231#section-6.5.29",
            title: "Account locked",
            status: 423,
            detail: "Your account has been locked due to too many failed login attempts. Please try again later or reset your password.")
    {
        LockoutEnd = lockoutEnd;
    }
}

public class EmailNotConfirmedException : AuthException
{
    public string Email { get; }

    public EmailNotConfirmedException(string email)
        : base(
            type: "https://tools.ietf.org/html/rfc7231#section-6.5.1",
            title: "Email not verified",
            status: 403,
            detail: "Please verify your email address before signing in. Check your inbox for a verification link.")
    {
        Email = email;
    }
}

public class RegistrationValidationException : AuthException
{
    public IReadOnlyList<string> Errors { get; }

    public RegistrationValidationException(IReadOnlyList<string> userFriendlyErrors)
        : base(
            type: "https://tools.ietf.org/html/rfc4918#section-11.2",
            title: "Registration failed",
            status: 422,
            detail: "Please fix the following issues and try again.")
    {
        Errors = userFriendlyErrors;
    }
}