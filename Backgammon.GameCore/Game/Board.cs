namespace Backgammon.GameCore.Game;

public class Board
{
    private const bool Debug = false;

    public const int NotSelected = -1;
    public const int PlayerCheckersCount = Debug ? 1 : 15;

    private const int PointsCount = 24;
    private const int FirstPlayerInnerTableStart = 18;
    private const int FirstPlayerInnerTableEnd = 23;
    private const int SecondPlayerInnerTableStart = 0;
    private const int SecondPlayerInnerTableEnd = 5;

    public GameState GameState { get; private set; }

    private readonly Point[] _points = new Point[PointsCount];
    public int SelectedPointNum { get; private set; } = NotSelected;

    private readonly List<int> _currentMoveDistances = [];
    private readonly List<Move> _possibleMoves = [];
    private readonly List<IPlaceForCheckers> _possibleStartPoints = [];

    public Bar Bar { get; private set; } = new Bar();
    public OffBoard OffBoard { get; private set; } = new OffBoard();

    public Player? CurrentPlayer { get; private set; }
    public Player Player1 { get; private set; }
    public Player Player2 { get; private set; }

    public Dice Dice { get; private set; } = new Dice();

    public bool NoMovesWereAvailable { get; private set; } = false;

    public Board(Player player1, Player player2)
    {
        Player1 = player1;
        Player2 = player2;
        SetupCheckers();
        GameState = GameState.ChoosingPlayer;
    }

    private void SelectFirstPlayer(Player player)
    {
        if (GameState != GameState.ChoosingPlayer)
            throw new InvalidOperationException("Game state is not CHOOSING_PLAYER");

        CurrentPlayer = player;
        GameState = GameState.Move;
    }

    private bool CanPutCheckerOnPoint(Color color, Point point)
    {
        return !point.HasCheckers ||
               point.CheckersCount == 1 ||
               point.CheckersColor == color;
    }

    private List<Move> GetPossibleMovesForBar(List<int> moveDistances)
    {
        if (CurrentPlayer == null) 
            throw new InvalidOperationException("Current player is not selected");
        
        List<Move> moves = [];
        foreach (var moveDistance in moveDistances)
        {
            var position = (CurrentPlayer == Player1) ? moveDistance - 1 : 23 - moveDistance + 1;
            if (!CanPutCheckerOnPoint(CurrentPlayer.Color, _points[position])) continue;
            
            moves.Add(new Move(Bar, _points[position], [moveDistance]));
                
            var remainingMoveDistances = new List<int>(moveDistances);
            remainingMoveDistances.Remove(moveDistance);
                
            moves.AddRange(GetPossibleMovesForPoint(CurrentPlayer.Color, position, remainingMoveDistances, [moveDistance]));
        }
        return moves;
    }
    
    private bool PlayerCanBearOff(Player player)
    {
        return CountAllPlayersCheckers(player) == CountCheckersInInnerTable(player);
    }
    
    private int CountAllPlayersCheckers(Player player)
    {
        var checkersCount = 0;
        for (var i = 0; i < PointsCount; i++)
        {
            if (_points[i].HasCheckers && _points[i].CheckersColor == player.Color)
            {
                checkersCount += _points[i].CheckersCount;
            }
        }
        return checkersCount;
    }
    
    private int CountCheckersInInnerTable(Player player)
    {
        var checkersCount = 0;
        var start = (player == Player1) ? FirstPlayerInnerTableStart : SecondPlayerInnerTableStart;
        var end = (player == Player1) ? FirstPlayerInnerTableEnd : SecondPlayerInnerTableEnd;

        for (var i = start; i <= end; i++)
        {
            if (_points[i].HasCheckers && _points[i].CheckersColor == player.Color)
            {
                checkersCount += _points[i].CheckersCount;
            }
        }
        return checkersCount;
    }

    private List<Move> GetPossibleMovesForPoint(Color checkersColor, int pointNum, List<int> moveDistances, List<int> previousMoveDistances)
    {
        if (CurrentPlayer == null)
            throw new InvalidOperationException("Current player is not selected");
        
        if (moveDistances.Count == 0)
        {
            return [];
        }
        
        List<Move> moves = [];
        var direction = (CurrentPlayer == Player1) ? 1 : -1;

        foreach (var moveDistance in moveDistances)
        {
            var destinationPointNum = pointNum + direction * moveDistance;
            if (!IsValidDestination(destinationPointNum)) continue;
            var destinationPoint = _points[destinationPointNum];
            if (CanPutCheckerOnPoint(checkersColor, destinationPoint))
            {
                moves = moves.Union(GenerateMovesForPoint(pointNum, destinationPointNum, moveDistance, moveDistances, previousMoveDistances)).ToList();
            }
        }
        
        if (PlayerCanBearOff(CurrentPlayer)) 
        {
            moves.AddRange(GenerateBearOffMoves(pointNum, moveDistances, previousMoveDistances));
        }
        
        return moves;
    }
    
    private bool IsValidDestination(int destinationPointNum) {
        return destinationPointNum is >= 0 and < PointsCount;
    }
    
    private List<Move> GenerateMovesForPoint(int pointNum, int destinationPointNum, int moveDistance, List<int> moveDistances, List<int> previousMoveDistances)
    {
        if (CurrentPlayer == null) 
            throw new InvalidOperationException("Current player is not selected");
        
        List<Move> moves = [];
        var destinationPoint = _points[destinationPointNum];
        moves.Add(new Move(_points[pointNum], destinationPoint, previousMoveDistances.Concat([moveDistance]).ToList()));

        var remainingMoveDistances = new List<int>(moveDistances);
        remainingMoveDistances.Remove(moveDistance);
        
        moves.AddRange(GetPossibleMovesForPoint(CurrentPlayer.Color, destinationPointNum, remainingMoveDistances, previousMoveDistances.Concat([moveDistance]).ToList()));
        
        return moves;
    }
    
    private List<Move> GenerateBearOffMoves(int pointNum, List<int> moveDistances, List<int> previousMoveDistances)
    {
        if (CurrentPlayer == null) 
            throw new InvalidOperationException("Current player is not selected");
        
        List<Move> moves = [];
        var edgePointNum = GetInnerTableEdgePointNum(CurrentPlayer);
        foreach (var moveDistance in moveDistances)
        {
            if (IsValidBearOffMove(pointNum, moveDistance, edgePointNum)) 
            {
                moves.Add(new Move(_points[pointNum], OffBoard, previousMoveDistances.Concat([moveDistance]).ToList()));
            }
        }
        return moves;
    }
    
    private int GetInnerTableEdgePointNum(Player player)
    {
        var edgePointNum = 0;
        var start = (player == Player1) ? FirstPlayerInnerTableEnd : SecondPlayerInnerTableStart;
        var end = (player == Player1) ? FirstPlayerInnerTableStart : SecondPlayerInnerTableEnd;
        var direction = (CurrentPlayer == Player1) ? 1 : -1;
        for (var i = start; i != end; i += direction)
        {
            if (_points[i].HasCheckers && _points[i].CheckersColor == player.Color)
            {
                edgePointNum = i;
            }
        }
        return edgePointNum;
    }
    
    private bool IsValidBearOffMove(int pointNum, int moveDistance, int edgePointNum)
    {
        return (CurrentPlayer == Player2 && (moveDistance == pointNum + 1 ||
                                             (pointNum == edgePointNum && pointNum + 1 - moveDistance < 0))) ||
               (CurrentPlayer == Player1 && (moveDistance == 23 - pointNum + 1 ||
                                             (pointNum == edgePointNum && pointNum + moveDistance > 23)));
    }
    
    private void CalculatePossibleMoves()
    {
        if (CurrentPlayer == null)
            throw new InvalidOperationException("Current player is not selected");
        
        _possibleMoves.Clear();
        if (Bar.HasCheckers(CurrentPlayer.Color))
        {
            _possibleMoves.AddRange(GetPossibleMovesForBar(_currentMoveDistances));
        } else if (SelectedPointNum != NotSelected)
        {
            _possibleMoves.AddRange(GetPossibleMovesForPoint(CurrentPlayer.Color, SelectedPointNum, _currentMoveDistances, []));
        }
    }
    
    public void RollDice()
    {
        do 
        {
            Dice.Roll();
        } while (GameState == GameState.ChoosingPlayer && Dice.IsDouble);
        
        AddDiceValuesToCurrentMoveDistances();
        
        if (GameState == GameState.ChoosingPlayer)
        {
            SelectFirstPlayer(Dice.FirstDiceValue > Dice.SecondDiceValue ? Player1 : Player2);
        }
        
        CalculatePossibleStartPoints();
        CalculatePossibleMoves();
        GameState = GameState.Move;
        NoMovesWereAvailable = false;
    }
    
    private void AddDiceValuesToCurrentMoveDistances()
    {
        _currentMoveDistances.Add(Dice.FirstDiceValue);
        _currentMoveDistances.Add(Dice.SecondDiceValue);
        if (Dice.IsDouble)
        {
            _currentMoveDistances.Add(Dice.FirstDiceValue);
            _currentMoveDistances.Add(Dice.SecondDiceValue);
        }
    }
    
    public void DeselectPoint()
    {
        SelectedPointNum = NotSelected;
        _possibleMoves.Clear();
        GameState = GameState.Move;
        CalculatePossibleStartPoints();
    }
    
    public bool SelectPoint(int pointNum)
    {
        if (CurrentPlayer == null)
            throw new InvalidOperationException("Current player is not selected");
        
        if (Bar.HasCheckers(CurrentPlayer.Color))
        {
            return false;
        }
        
        var point = _points[pointNum];
        if (IsPossibleStartPoint(point))
        {
            SelectedPointNum = pointNum;
            CalculatePossibleMoves();
            GameState = GameState.Move;
            return true;
        }
        return false;
    }

    public bool Move(IPlaceForCheckers destination)
    {
        if (CurrentPlayer == null)
            throw new InvalidOperationException("Current player is not selected");
        if (SelectedPointNum == NotSelected && !Bar.HasCheckers(CurrentPlayer.Color))
        {
            return false;
        }

        foreach (var move in _possibleMoves)
        {
            if (move.To == destination)
            {
                ExecuteMove(move);
                _possibleStartPoints.Clear();
                DeselectPoint();
                return true;
            }
        }
        return false;
    }

    private void ExecuteMove(Move move)
    {
        if (CurrentPlayer == null)
            throw new InvalidOperationException("Current player is not selected");
        var checkerColor = GetCheckerForMove();
        if (move.To != OffBoard)
        {
            var destinationPoint = (Point)move.To;
            if (destinationPoint.CheckersCount == 1 && destinationPoint.CheckersColor != CurrentPlayer.Color)
            {
                if (destinationPoint.CheckersColor == null)
                    throw new InvalidOperationException("No checker on the selected point");
                Bar.PutChecker((Color)destinationPoint.CheckersColor);
                destinationPoint.RemoveChecker();
            }
        }
        move.To.PutChecker(checkerColor);
        UpdateMoveDistances(move);
    }

    
    /// Removes the checker from the selected point or bar and returns its color
    private Color GetCheckerForMove()
    {
        if (CurrentPlayer == null)
            throw new InvalidOperationException("Current player is not selected");
        if (Bar.HasCheckers(CurrentPlayer.Color)) 
        {
            Bar.RemoveChecker(CurrentPlayer.Color);
            return CurrentPlayer.Color;
        }
        var checkerColor = _points[SelectedPointNum].CheckersColor;
        if (checkerColor == null)
            throw new InvalidOperationException("No checker on the selected point");
        _points[SelectedPointNum].RemoveChecker();
        return (Color)checkerColor;
    }

    private void UpdateMoveDistances(Move move)
    {
        foreach (var moveUsedDistance in move.UsedDistances)
        {
            _currentMoveDistances.Remove(moveUsedDistance);
        }
    }
    
    public void ChangePlayer()
    {
        if (CurrentPlayer == null)
            throw new InvalidOperationException("Current player is not selected");
        
        CurrentPlayer = (CurrentPlayer == Player1) ? Player2 : Player1;
        GameState = GameState.Roll;
        _currentMoveDistances.Clear();
        _possibleStartPoints.Clear();
    }
    
    private void SetupCheckers()
    {
        if (Debug)
        {
            SetupPlayerCheckers(Player1, [0], [1]);
            SetupPlayerCheckers(Player2, [23], [1]);
        }
        else
        {
            SetupPlayerCheckers(Player2, [0, 16, 11, 18], [2, 3, 5, 5]);
            SetupPlayerCheckers(Player1, [23, 7, 5, 12], [2, 3, 5, 5]);
        }
    }

    private void SetupPlayerCheckers(Player player, int[] positions, int[] counts)
    {
        for (var i = 0; i < positions.Length; i++)
        {
            _points[positions[i]].PutCheckers(player.Color, counts[i]);
        }
    }

    public bool IsGameOver => GameState == GameState.GameOver;
    
    public Point GetPoint(int pointNum)
    {
        if (pointNum < 0 || pointNum >= PointsCount)
            throw new ArgumentOutOfRangeException(nameof(pointNum), "Point number must be between 0 and 23");
        return _points[pointNum];
    }
    
    public bool IsPossibleMove(IPlaceForCheckers destination)
    {
        return _possibleMoves.Any(possibleMove => possibleMove.To == destination);
    }
    
    private void CalculatePossibleStartPoints()
    {
        if (CurrentPlayer == null)
            throw new InvalidOperationException("Current player is not selected");
        
        _possibleStartPoints.Clear();
        if (Bar.HasCheckers(CurrentPlayer.Color))
        {
            if (GetPossibleMovesForBar(_currentMoveDistances).Count != 0)
            {
                _possibleStartPoints.Add(Bar);
            }
        }
        else
        {
            for (var i = 0; i < PointsCount; i++)
            {
                if (_points[i].HasCheckers && _points[i].CheckersColor == CurrentPlayer.Color &&
                    GetPossibleMovesForPoint(CurrentPlayer.Color, i, _currentMoveDistances, []).Count != 0)
                {
                    _possibleStartPoints.Add(_points[i]);
                }
            }
        }
    }
    
    public bool IsPossibleStartPoint(IPlaceForCheckers placeForCheckers)
    {
        return _possibleStartPoints.Contains(placeForCheckers);
    }

    public bool NoMovesAvailable => _possibleStartPoints.Count == 0;

    public IReadOnlyList<int> CurrentMoveDistances => _currentMoveDistances.AsReadOnly();
    
    public void ForceFinishGame()
    {
        GameState = GameState.GameOver;
    }
 
    public Player? Winner => 
        OffBoard.HasAllCheckers(Player1.Color) ? Player1 :
        OffBoard.HasAllCheckers(Player2.Color) ? Player2 :
        null;

    public int GetScore(Player player)
    {
        int score = 0;
        for (var i = 0; i < PointsCount; i++)
        {
            if (_points[i].HasCheckers && _points[i].CheckersColor == player.Color)
            {
                score += _points[i].CheckersCount * ((player == Player2) ? (i + 1) : (24 - i));
            }
        }
        score += Bar.CountCheckers(player.Color) * 25;
        return score;
    }
}