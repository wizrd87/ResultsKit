namespace ResultsKit;

/// <summary>
/// Represents an error.
/// </summary>
public class Error
{
    /// <summary>
    /// Initializes a new instance of the <see cref="Error"/> class with the specified message.
    /// </summary>
    /// <param name="code">The error code.</param>
    /// <param name="message">The error message.</param>
    public Error(string code, string message)
    {
        Code = code;
        Message = message;
    }

    /// <summary>
    /// Gets or sets the error code.
    /// </summary>
    public string Code { get; set; }

    /// <summary>
    /// Gets or sets the error message.
    /// </summary>
    public string Message { get; set; }
}
