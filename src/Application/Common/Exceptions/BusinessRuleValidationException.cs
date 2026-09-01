namespace Capri.Sgr.Application.Common.Exceptions;

/// <summary>Represents a business invariant that makes an otherwise valid request unacceptable.</summary>
public sealed class BusinessRuleValidationException(string message) : Exception(message);
