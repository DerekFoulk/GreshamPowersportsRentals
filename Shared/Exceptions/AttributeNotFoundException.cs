using System;

namespace BlazorApp.Shared.Exceptions;

/// <summary>The exception that is thrown when an attempt to access an <see cref="Attribute"/> fails due to the absence of the attribute.</summary>
public class AttributeNotFoundException : Exception
{
    /// <summary>Initializes a new instance of the <see cref="AttributeNotFoundException" /> class.</summary>
    public AttributeNotFoundException()
    {
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="AttributeNotFoundException" /> class with a specified error
    ///     message.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    public AttributeNotFoundException(string? message) : base(message)
    {
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="AttributeNotFoundException" /> class with a specified error
    ///     message and a reference to the inner exception that is the cause of this exception.
    /// </summary>
    /// <param name="message">The error message that explains the reason for the exception.</param>
    /// <param name="innerException">
    ///     The exception that is the cause of the current exception, or a null reference (
    ///     <see langword="Nothing" /> in Visual Basic) if no inner exception is specified.
    /// </param>
    public AttributeNotFoundException(string? message, Exception? innerException) : base(message, innerException)
    {
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="AttributeNotFoundException" /> class with a specified attribute
    ///     type.
    /// </summary>
    /// <param name="attributeType">The attribute type that was not found, causing the exception.</param>
    public AttributeNotFoundException(Type attributeType) : base($"The '{attributeType.FullName}' attribute was not found")
    {
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="AttributeNotFoundException" /> class with a specified attribute
    ///     type and a reference to the inner exception that is the cause of this exception.
    /// </summary>
    /// <param name="attributeType">The attribute type that was not found, causing the exception.</param>
    /// <param name="innerException">
    ///     The exception that is the cause of the current exception, or a null reference (
    ///     <see langword="Nothing" /> in Visual Basic) if no inner exception is specified.
    /// </param>
    public AttributeNotFoundException(Type attributeType, Exception? innerException) : base($"The '{attributeType.FullName}' attribute was not found", innerException)
    {
    }
}