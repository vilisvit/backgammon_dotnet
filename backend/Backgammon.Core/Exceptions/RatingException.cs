namespace Backgammon.Core.Exceptions;

public class RatingException(string message, Exception? inner = null) : Exception(message, inner);