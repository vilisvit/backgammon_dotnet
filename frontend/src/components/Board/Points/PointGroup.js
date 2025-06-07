import React from 'react';
import Point from './Point';
import PropTypes from 'prop-types';

function PointGroup({ position, pointsData, sessionId, myTurn }) {
    const containerStyle = {
        width: '100%',
        height: '250px',
    };

    return (
        <div className={`point-group-${position}`} style={containerStyle}>
            {pointsData.map((pointData) => (
                <Point
                    key={pointData.id}
                    pointGroupPosition={position}
                    pointData={pointData}
                    sessionId={sessionId}
                    myTurn={myTurn}
                />
            ))}
        </div>
    );
}

PointGroup.propTypes = {
    range: PropTypes.arrayOf(PropTypes.number).isRequired,
    position: PropTypes.oneOf(['top', 'bottom']).isRequired,
    pointsData: PropTypes.arrayOf(
        PropTypes.shape({
            id: PropTypes.number.isRequired,
            checkersCount: PropTypes.number.isRequired,
            checkersColor: PropTypes.string.isRequired,
        })
    ).isRequired,
    sessionId: PropTypes.string.isRequired,
    myTurn: PropTypes.bool.isRequired,
};

export default PointGroup;