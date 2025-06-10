import React, { useEffect, useState } from 'react';
import PropTypes from 'prop-types';
import PointHint from "../PointHint/PointHint";
import { sendOffBoardClick } from "../../../api/ws/socket";

function OffBoardPart({ position, offBoardPartData: initialOffBoardPartData, sessionId, myTurn }) {
    const [offBoardPartData, setOffBoardPartData] = useState(initialOffBoardPartData);

    useEffect(() => {
        setOffBoardPartData(initialOffBoardPartData);
    }, [initialOffBoardPartData]);

    const handleClick = () => {
        sendOffBoardClick(sessionId);
    };

    const colorClass = offBoardPartData.checkersColor === 'white' ? 'white-checker-side' : 'black-checker-side';

    return (
        <div className={`off-board-${position}`} onClick={handleClick}>
            {Array.from({ length: offBoardPartData.checkersCount }).map((_, index) => (
                <div key={index} className={`${colorClass}`}></div>
            ))}
            {myTurn && offBoardPartData.possibleMove && <PointHint type={'possible-move'} position={position} />}
        </div>
    );
}

OffBoardPart.propTypes = {
    myTurn: PropTypes.bool.isRequired,
    position: PropTypes.oneOf(['top', 'bottom']).isRequired,
    offBoardPartData: PropTypes.shape({
        checkersCount: PropTypes.number.isRequired,
        checkersColor: PropTypes.string.isRequired,
        possibleMove: PropTypes.bool.isRequired,
    }).isRequired,
    sessionId: PropTypes.string.isRequired,
};

export default OffBoardPart;