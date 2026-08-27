namespace SecureLoginApp1.Exceptions
{
    public class UserNotFoundException : AppException
    {
        public UserNotFoundException(string message = "The requested user could not be found.")
            : base(message)
        {
        }
    }
}
