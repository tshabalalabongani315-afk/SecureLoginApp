namespace SecureLoginApp1.Exceptions
{
    public class InvalidTwoFactorCodeException : AppException
    {
        public InvalidTwoFactorCodeException(string message = "The verification code is invalid or has expired.")
            : base(message)
        {
        }
    }
}
