using System;

namespace MetaLibrary.Resources;

/// <summary>
/// Represents <see cref="ResourceName"/> errors that occur during application execution.
/// </summary>
public class ResourceNameException : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ResourceNameException"/> class.
    /// </summary>
    public ResourceNameException() { }

    /// <summary>
    /// Initializes a new instance of the <see cref="ResourceNameException"/> class with a
    /// specified error message.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    public ResourceNameException(string message) : base(message) { }

    /// <summary>
    /// Initializes a new instance of the <see cref="ResourceNameException"/> class with a
    /// specified error message and a reference to the inner exception that is the cause of
    /// this exception.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    /// <param name="inner">
    /// The exception that is the cause of the current exception, or a null reference
    /// (Nothing in Visual Basic) if no inner exception is specified.
    /// </param>
    public ResourceNameException(string message, Exception inner) : base(message, inner) { }
}
