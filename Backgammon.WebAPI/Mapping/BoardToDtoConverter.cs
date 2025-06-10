using AutoMapper;
using Backgammon.GameCore.Game;
using Backgammon.WebAPI.Dtos.Board;

namespace Backgammon.WebAPI.Mapping;

public class BoardToDtoConverter : ITypeConverter<Board, BoardDto>
{
    public BoardDto Convert(Board source, BoardDto destination, ResolutionContext context)
    {
        var bar = source.Bar;
        var offBoard = source.OffBoard;
        var currentPlayer = source.CurrentPlayer;
        var player1 = source.Player1;
        var player2 = source.Player2;
        var dice = source.Dice;
        var selectedPointNum = source.SelectedPointNum;

        var pointDtos = new PointDto[24];
        for (var i = 0; i < 24; i++)
        {
            var point = source.GetPoint(i);
            pointDtos[i] = new PointDto
            {
                Id = i,
                CheckersCount = point.CheckersCount,
                CheckersColor = point.CheckersColor?.ToString().ToLower(),
                Selected = selectedPointNum == i,
                PossibleMove = source.IsPossibleMove(point),
                PossibleStartPoint = source.IsPossibleStartPoint(point) && selectedPointNum == Board.NotSelected
            };
        }

        var barDto = new BarDto
        {
            WhiteCheckersCount = bar.CountCheckers(Color.White),
            BlackCheckersCount = bar.CountCheckers(Color.Black),
            SelectedForBlackPlayer = source.IsPossibleStartPoint(bar) && currentPlayer == player1,
            SelectedForWhitePlayer = source.IsPossibleStartPoint(bar) && currentPlayer == player2
        };

        var offBoardDto = new OffBoardDto
        {
            BlackCheckersCount = offBoard.CountCheckers(Color.Black),
            WhiteCheckersCount = offBoard.CountCheckers(Color.White),
            PossibleMoveForBlackPlayer = source.IsPossibleMove(offBoard) && currentPlayer == player1,
            PossibleMoveForWhitePlayer = source.IsPossibleMove(offBoard) && currentPlayer == player2
        };

        var diceDto = new DiceDto
        {
            FirstDiceValue = dice.FirstDiceValue,
            SecondDiceValue = dice.SecondDiceValue,
            AreRolled = dice.AreRolled,
        };

        var player1Dto = new PlayerDto
        {
            Color = player1.Color.ToString().ToLower(),
            CurrentScore = source.GetScore(player1),
            Username = player1.Name
        };

        var player2Dto = new PlayerDto
        {
            Color = player2.Color.ToString().ToLower(),
            CurrentScore = source.GetScore(player2),
            Username = player2.Name
        };

        return new BoardDto
        {
            Bar = barDto,
            CurrentPlayerUsername = source.CurrentPlayer?.Name,
            Points = pointDtos,
            Dice = diceDto,
            GameState = source.GameState.ToString().ToLower(),
            OffBoard = offBoardDto,
            Player1 = player1Dto,
            Player2 = player2Dto,
            WinnerUsername = source.Winner?.Name,
            NoMovesWereAvailable = source.NoMovesWereAvailable
        };
    }
}
