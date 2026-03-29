namespace ResultsKit;

/// <summary>
/// Represents a validation error.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="ValidationError"/> class.
/// </remarks>
/// <param name="messages">List of validation error messages</param>
public class ValidationError(string[] messages)
    : Error(ResultType.VALIDATION_ERROR, "One or more validation errors occurred.")
{
    public string[] Messages { get; } = messages;
}
