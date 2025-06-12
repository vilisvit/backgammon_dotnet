namespace Backgammon.GameCore.Game;

public enum Color
{
    White,
    Black
}

public static class ColorExtensions
{
    private static readonly Random Random = new();

    public static Color GetRandom()
    {
        return Random.NextDouble() < 0.5 ? Color.White : Color.Black;
    }
}
