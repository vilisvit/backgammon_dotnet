namespace Backgammon.GameCore.Game;

public class Move
{
    public IPlaceForCheckers From { get; private set; }
    public IPlaceForCheckers To { get; private set; }
    public List<int> UsedDistances { get; }
    
    public Move(IPlaceForCheckers from, IPlaceForCheckers to, List<int> usedDistances)
    {
        From = from;
        To = to;
        UsedDistances = usedDistances;
    }
}