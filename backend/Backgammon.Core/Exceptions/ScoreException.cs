namespace Backgammon.Core.Exceptions;

public class ScoreException(string message, Exception? inner = null) : Exception(message, inner);