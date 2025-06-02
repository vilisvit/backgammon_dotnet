namespace Backgammon.Infrastructure.Exceptions;

public class RatingException(string message, Exception? inner = null) : Exception(message, inner);