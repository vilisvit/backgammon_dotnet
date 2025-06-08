import React, { useEffect, useState } from 'react';
import PropTypes from 'prop-types';
import PointHint from "../PointHint/PointHint";
import { sendPointClick } from "../../../api/ws/socket";

function Point({ style, pointGroupPosition: initialPointGroupPosition, pointData: initialPointData, sessionId, myTurn }) {
    const [pointData, setPointData] = useState(initialPointData);

    useEffect(() => {
        setPointData(initialPointData);
    }, [initialPointData]);

    const handleClick = () => {
        sendPointClick(sessionId, pointData.id);
    };

    const colorClass = pointData.checkersColor === 'white' ? 'white-checker' : 'black-checker';

    return (
        <div className={`point-${initialPointGroupPosition}`}
             id={`point-${pointData.id}`}
             style={style}
             onClick={handleClick}>

            {Array.from({ length: pointData.checkersCount }).map((_, index) => (
                <div key={index} className={`${colorClass}`}></div>
            ))}
            {myTurn && pointData.selected && <PointHint type={'selected'} position={initialPointGroupPosition}/>}
            {myTurn && pointData.possibleMove && <PointHint type={'possible-move'} position={initialPointGroupPosition}/>}
            {myTurn && pointData.possibleStartPoint && <PointHint type={'possible-start-point'} position={initialPointGroupPosition}/>}
        </div>
    );
}

Point.propTypes = {
    style: PropTypes.object,
    pointGroupPosition: PropTypes.oneOf(['top', 'bottom']).isRequired,
    pointData: PropTypes.shape({
        id: PropTypes.number.isRequired,
        checkersCount: PropTypes.number.isRequired,
        checkersColor: PropTypes.string.isRequired,
        selected: PropTypes.bool.isRequired,
        possibleMove: PropTypes.bool.isRequired,
        possibleStartPoint: PropTypes.bool.isRequired,
    }).isRequired,
    sessionId: PropTypes.string.isRequired,
    myTurn: PropTypes.bool.isRequired,
};

export default Point;