namespace Backgammon.GameCore.Game;

public class Player
{
    public Player(Color color, string name, Guid userId)
    {
        Color = color;
        Name = name ?? (color == Color.White ? "White Player" : "Black Player");
        UserId = userId;
    }

    public Color Color { get; }
    public string Name { get; }

    public Guid UserId { get; }
}