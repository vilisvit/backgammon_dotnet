namespace Backgammon.Core.Exceptions;

public class CommentException(string message, Exception? inner = null) : Exception(message, inner);