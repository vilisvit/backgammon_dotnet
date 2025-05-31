namespace Backgammon.Core;

public class Point : IPlaceForCheckers
{
    public int CheckersCount { get; private set; }
    public Color ?CheckersColor { get; private set; }

    public void PutChecker(Color color)
    {
        PutCheckers(color, 1);
    }

    public void PutCheckers(Color color, int count)
    {
        if (CheckersColor != null && CheckersColor != color)
            throw new InvalidOperationException("Point is occupied by opponent's checkers");
        CheckersColor = color;
        CheckersCount += count;
    }
    
    public bool HasCheckers => CheckersCount > 0;

    public void RemoveChecker()
    {
        if (CheckersCount == 0)
        {
            throw new InvalidOperationException("No checkers on the point");
        }
        CheckersCount--;
        if (CheckersCount == 0)
        {
            CheckersColor = null;
        }
    }
}