namespace Backgammon.Core;

public class Dice
{
    public const int NotRolled = 0;
    
    public int FirstDiceValue { get; private set; } = NotRolled;
    public int SecondDiceValue { get; private set; } = NotRolled;

    private readonly Random _random = new();
    
    public void Roll() 
    {
        FirstDiceValue = _random.Next(1, 7);
        SecondDiceValue = _random.Next(1, 7);
    }
    
    public bool AreRolled => FirstDiceValue != NotRolled && SecondDiceValue != NotRolled;
    
    public bool IsDouble => FirstDiceValue == SecondDiceValue;
}