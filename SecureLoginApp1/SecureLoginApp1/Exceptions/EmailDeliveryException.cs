using System;

namespace SecureLoginApp1.Exceptions
{
    public class EmailDeliveryException : AppException
    {
        public EmailDeliveryException(string message, Exception? innerException = null)
            : base(message)
        {
            InnerExceptionDetail = innerException?.Message;
        }

        public string? InnerExceptionDetail { get; }
    }
}
