using BlazorApp.Shared.Exceptions;
using SeleniumExtras.PageObjects;

namespace Data.Exceptions;

/// <summary>The exception that is thrown when an attempt to access a <see cref="FindsByAttribute"/> attribute fails due to the absence of the attribute.</summary>
public class FindsByAttributeNotFoundException : AttributeNotFoundException
{
    /// <summary>Initializes a new instance of the <see cref="FindsByAttributeNotFoundException" /> class.</summary>
    public FindsByAttributeNotFoundException() : base(typeof(FindsByAttributeNotFoundException))
    {
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="FindsByAttributeNotFoundException" /> class with a specified error
    ///     message.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    public FindsByAttributeNotFoundException(string? message) : base(message)
    {
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="FindsByAttributeNotFoundException" /> class with a specified error
    ///     message and a reference to the inner exception that is the cause of this exception.
    /// </summary>
    /// <param name="message">The error message that explains the reason for the exception.</param>
    /// <param name="innerException">
    ///     The exception that is the cause of the current exception, or a null reference (
    ///     <see langword="Nothing" /> in Visual Basic) if no inner exception is specified.
    /// </param>
    public FindsByAttributeNotFoundException(string? message, Exception? innerException) : base(message, innerException)
    {
    }
}