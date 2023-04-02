using System;

/// <summary>
/// An exception that was intentionally thrown by the user.
/// </summary>
public class IntentionalCrashException : Exception
{
    /// <summary>Initializes a new instance of the <see cref="T:IntentionalCrashException"></see> class.</summary>
    public IntentionalCrashException()
    {
    }

    /// <summary>Initializes a new instance of the <see cref="T:IntentionalCrashException"></see> class with a specified error message.</summary>
    /// <param name="message">The message that describes the error.</param>
    public IntentionalCrashException(string message) : base(message)
    {
    }

    /// <summary>Initializes a new instance of the <see cref="T:IntentionalCrashException"></see> class with a specified error message and a reference to the inner exception that is the cause of this exception.</summary>
    /// <param name="message">The error message that explains the reason for the exception.</param>
    /// <param name="innerException">The exception that is the cause of the current exception, or a null reference (Nothing in Visual Basic) if no inner exception is specified.</param>
    public IntentionalCrashException(string message, Exception innerException) : base(message, innerException)
    {
    }
}
