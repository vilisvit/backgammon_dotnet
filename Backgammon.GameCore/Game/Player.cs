namespace Backgammon.GameCore.Game;

public class Player
{
    public Player(Color color, string? name = null)
    {
        Color = color;
        Name = name ?? (color == Color.White ? "White Player" : "Black Player");
    }

    public Color Color { get; }
    public string Name { get; }
}