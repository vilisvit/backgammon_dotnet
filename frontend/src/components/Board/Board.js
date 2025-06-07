import React, { useEffect, useState } from 'react';
import PointGroup from './Points/PointGroup';
import Dice from './Dice/Dice';
import './Board.css';
import Bar from './Bar/Bar';
import OffBoard from "./OffBoard/OffBoard";
import EndGameLabel from "./EndGameLabel/EndGameLabel";
import PropTypes from "prop-types";
import { sendMessage } from "../../api/ws/socket";

function Board({ gameSessionId, gameUpdates, username, onGameFinish }) {
    const [boardData, setBoardData] = useState(null);

    useEffect(() => {
        if (gameUpdates) {
            console.log('Game updates:', gameUpdates);
            setBoardData(gameUpdates.board);
        }
    }, [gameUpdates]);

    if (!boardData) {
        return <div>Loading board...</div>;
    }

    const handleClick = () => {
        if (boardData.gameState === 'choosing_player' || boardData.gameState === 'roll') {
            sendMessage(`/app/game/${gameSessionId}/roll`, {}); // Send an empty payload or include relevant data if needed
        }
    };

    const mirrored = boardData.player1.username === username;

    console.log('I am black:', mirrored);

    return (
        <div id="board-container">
            {boardData.winnerUsername && (
                <EndGameLabel
                    winner={boardData.winnerUsername}
                    onClick={onGameFinish}
                />
            )}

            {boardData.noMovesWereAvailable && (
                <div className="no-moves-label">No moves available</div>
            )}

            <div id="left-board-half" className="board-half" style={{ left: '25px' }}>
                {/* Points 13 to 18 */}
                <PointGroup
                    range={ mirrored ? [12, 17] : [11, 6] }
                    position="bottom"
                    pointsData={ mirrored ? boardData.points.slice(12, 18) : boardData.points.slice(6, 12).reverse() }
                    sessionId={gameSessionId}
                    myTurn={boardData.currentPlayerUsername === username}
                />
                {/* Points 12 to 7 */}
                <PointGroup
                    range={ mirrored ? [11, 6] : [12, 17] }
                    position="top"
                    pointsData={ mirrored ? boardData.points.slice(6, 12).reverse() : boardData.points.slice(12, 18) }
                    sessionId={gameSessionId}
                    myTurn={boardData.currentPlayerUsername === username}
                />

                {/* Dice */}
                {!(boardData.gameState === 'choosing_player' || boardData.gameState === 'roll') && (
                    <div className="dice-container">
                        <Dice
                            diceValue={boardData.dice.firstDiceValue}
                        />
                        <Dice
                            diceValue={boardData.dice.secondDiceValue}
                        />
                    </div>
                )}

                {(boardData.gameState === 'choosing_player' || (boardData.gameState === 'roll' && boardData.currentPlayerUsername === username)) && (
                    <button
                        className="roll-dice-button"
                        onClick={handleClick}
                    >
                        Roll the dice
                    </button>
                )}
            </div>

            <div id="right-board-half" className="board-half" style={{ left: '410px' }}>
                {/* Points 19 to 24 */}
                <PointGroup
                    range={ mirrored ? [18, 23] : [5, 0] }
                    position="bottom"
                    pointsData={ mirrored ? boardData.points.slice(18, 24) : boardData.points.slice(0, 6).reverse() }
                    sessionId={gameSessionId}
                    myTurn={boardData.currentPlayerUsername === username}
                />
                {/* Points 6 to 1 */}
                <PointGroup
                    range={ mirrored ? [5, 0] : [18, 23] }
                    position="top"
                    pointsData={ mirrored ? boardData.points.slice(0, 6).reverse() : boardData.points.slice(18, 24) }
                    sessionId={gameSessionId}
                    myTurn={boardData.currentPlayerUsername === username}
                />
                {!boardData.winnerUsername && (
                    <div className="turn-label">
                        {boardData.gameState === 'choosing_player' ? (
                            <>
                                Roll dice to choose the first player.
                            </>
                        ) : (
                            <>
                                {boardData.currentPlayerUsername === username ? (
                                    <strong>Your turn</strong>
                                ) : (
                                    <>
                                        <strong>{boardData.currentPlayerUsername}</strong>'s Turn
                                    </>
                                )}
                            </>
                        )}
                    </div>
                )}
            </div>

            {/* Bar */}
            <Bar
                style={{ left: '360px', top: '25px', width: '50px', height: '650px' }}
                barData={boardData.bar}
                mirrored={mirrored}
            />

            {/* Off Board */}
            <OffBoard
                style={{ left: '770px', top: '25px', width: '50px', height: '650px' }}
                offBoardData={boardData.offBoard}
                sessionId={gameSessionId}
                myTurn={boardData.currentPlayerUsername === username}
                playerBlackScore={boardData.player2.color === 'black' ? boardData.player2.currentScore : boardData.player1.currentScore}
                playerWhiteScore={boardData.player2.color === 'white' ? boardData.player2.currentScore : boardData.player1.currentScore}
                mirrored={mirrored}
            />

        </div>
    );
}

Board.propTypes = {
    gameSessionId: PropTypes.string.isRequired,
    gameUpdates: PropTypes.shape({
        status: PropTypes.string,
        message: PropTypes.string,
        board: PropTypes.shape({
            points: PropTypes.arrayOf(
                PropTypes.shape({
                    id: PropTypes.number.isRequired,
                    checkersCount: PropTypes.number.isRequired,
                    checkersColor: PropTypes.string.isRequired,
                    selected: PropTypes.bool.isRequired,
                    possibleMove: PropTypes.bool.isRequired,
                    possibleStartPoint: PropTypes.bool.isRequired,
                })
            ).isRequired,
            bar: PropTypes.arrayOf(
                PropTypes.shape({
                    whiteCheckersCount: PropTypes.number.isRequired,
                    blackCheckersCount: PropTypes.number.isRequired,
                    selectedForBlackPlayer: PropTypes.bool.isRequired,
                    selectedForWhitePlayer: PropTypes.bool.isRequired,
                })
            ).isRequired,
            offBoard: PropTypes.arrayOf(
                PropTypes.shape({
                    whiteCheckersCount: PropTypes.number.isRequired,
                    blackCheckersCount: PropTypes.number.isRequired,
                    possibleMoveForBlackPlayer: PropTypes.bool.isRequired,
                    possibleMoveForWhitePlayer: PropTypes.bool.isRequired,
                })
            ).isRequired,
        }).isRequired,
        player1: PropTypes.shape({
            username: PropTypes.string.isRequired,
            color: PropTypes.string.isRequired,
            currentScore: PropTypes.number.isRequired,
        }),
        player2: PropTypes.shape({
            username: PropTypes.string.isRequired,
            color: PropTypes.string.isRequired,
            currentScore: PropTypes.number.isRequired,
        }),
        currentPlayerUsername: PropTypes.string.isRequired,
        winnerUsername: PropTypes.string,
        noMovesWereAvailable: PropTypes.bool,
        username: PropTypes.string.isRequired,
    }),
    onGameFinish: PropTypes.func.isRequired,
};

export default Board;