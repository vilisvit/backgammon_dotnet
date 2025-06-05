namespace Backgammon.GameCore.Game;

public class Bar : IPlaceForCheckers
{
    private int _countBlackCheckers;
    private int _countWhiteCheckers;
    
    public void PutChecker(Color color)
    {
        switch (color)
        {
            case Color.Black:
                _countBlackCheckers++;
                break;
            case Color.White:
                _countWhiteCheckers++;
                break;
            default:
                throw new ArgumentException("Invalid color");
        }
    }
    
    public void RemoveChecker(Color color)
    {
        switch (color)
        {
            case Color.Black:
                if (_countBlackCheckers == 0)
                {
                    throw new InvalidOperationException("No black checkers on the bar");
                }
                _countBlackCheckers--;
                break;
            case Color.White:
                if (_countWhiteCheckers == 0)
                {
                    throw new InvalidOperationException("No white checkers on the bar");
                }
                _countWhiteCheckers--;
                break;
            default:
                throw new ArgumentException("Invalid color");
        }
    }
    
    public bool HasCheckers(Color color)
    {
        return color switch
        {
            Color.Black => _countBlackCheckers > 0,
            Color.White => _countWhiteCheckers > 0,
            _ => throw new ArgumentException("Invalid color")
        };
    }
    
    public int CountCheckers(Color color)
    {
        return color switch
        {
            Color.Black => _countBlackCheckers,
            Color.White => _countWhiteCheckers,
            _ => throw new ArgumentException("Invalid color")
        };
    }
}