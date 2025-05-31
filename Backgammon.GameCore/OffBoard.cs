namespace Backgammon.Core;

public class OffBoard : IPlaceForCheckers
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
    
    public int CountCheckers(Color color)
    {
        return color switch
        {
            Color.Black => _countBlackCheckers,
            Color.White => _countWhiteCheckers,
            _ => throw new ArgumentException("Invalid color")
        };
    }
    
    public bool HasAllCheckers(Color color)
    {
        return color switch
        {
            Color.Black => _countBlackCheckers == Board.PlayerCheckersCount,
            Color.White => _countWhiteCheckers == Board.PlayerCheckersCount,
            _ => throw new ArgumentException("Invalid color")
        };
    }
}