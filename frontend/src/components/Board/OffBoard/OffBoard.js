import PropTypes from "prop-types";
import { useEffect, useState } from "react";
import OffBoardPart from "./OffBoardPart";
import "./OffBoard.css";
import React from "react";

function OffBoard({ offBoardData: initialOffBoardData, sessionId, myTurn, playerBlackScore, playerWhiteScore, mirrored }) {
    const [offBoardData, setOffBoardData] = useState(initialOffBoardData);

    useEffect(() => {
        setOffBoardData(initialOffBoardData);
    }, [initialOffBoardData]);

    let topOffBoardPartData;
    let bottomOffBoardPartData;

    if (!mirrored) {
        topOffBoardPartData = {
            checkersCount: offBoardData.whiteCheckersCount,
            checkersColor: 'white',
            possibleMove: offBoardData.possibleMoveForWhitePlayer,
        };

        bottomOffBoardPartData = {
            checkersCount: offBoardData.blackCheckersCount,
            checkersColor: 'black',
            possibleMove: offBoardData.possibleMoveForBlackPlayer,
        };
    } else {
        topOffBoardPartData = {
            checkersCount: offBoardData.blackCheckersCount,
            checkersColor: 'black',
            possibleMove: offBoardData.possibleMoveForBlackPlayer,
        };

        bottomOffBoardPartData = {
            checkersCount: offBoardData.whiteCheckersCount,
            checkersColor: 'white',
            possibleMove: offBoardData.possibleMoveForWhitePlayer,
        };
    }

    return (
        <div className="off-board-container">
            <OffBoardPart
                position={'top'}
                offBoardPartData={topOffBoardPartData}
                sessionId={sessionId}
                myTurn={myTurn}
            />
            <div className="off-board-score">
                <div className="off-board-score-item">
                    <span className="off-board-score-value">{mirrored ? playerWhiteScore : playerBlackScore}</span>
                </div>
                <div className="off-board-score-item">
                    <span className="off-board-score-value">{mirrored ? playerBlackScore : playerWhiteScore}</span>
                </div>
            </div>
            <OffBoardPart
                position={'bottom'}
                offBoardPartData={bottomOffBoardPartData}
                sessionId={sessionId}
                myTurn={myTurn}
            />
        </div>
    );
}

OffBoard.propTypes = {
    offBoardData: PropTypes.shape({
        whiteCheckersCount: PropTypes.number.isRequired,
        blackCheckersCount: PropTypes.number.isRequired,
        possibleMoveForWhitePlayer: PropTypes.bool.isRequired,
        possibleMoveForBlackPlayer: PropTypes.bool.isRequired,
    }).isRequired,
    sessionId: PropTypes.string.isRequired,
    myTurn: PropTypes.bool.isRequired,
    playerBlackScore: PropTypes.number.isRequired,
    playerWhiteScore: PropTypes.number.isRequired,
    mirrored: PropTypes.bool,
};

export default OffBoard;