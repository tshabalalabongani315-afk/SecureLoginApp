using System;

namespace SecureLoginApp1.Exceptions
{
    /// <summary>
    /// Base type for application-specific exceptions that PageModels catch explicitly
    /// and map to user-facing ModelState errors, instead of letting a raw exception reach the view.
    /// </summary>
    public abstract class AppException : Exception
    {
        protected AppException(string message) : base(message)
        {
        }
    }
}
