namespace Backgammon.Infrastructure.Exceptions;

public class ScoreException(string message, Exception? inner = null) : Exception(message, inner);