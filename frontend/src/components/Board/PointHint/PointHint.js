import React from 'react';
import PropTypes from 'prop-types';
import './PointHint.css';

function PointHint({ type, position }) {
    const hintClass = `hint-circle hint-${type} hint-${position}`;
    return <div className={hintClass}></div>;
}

PointHint.propTypes = {
    type: PropTypes.oneOf(['selected', 'possible-move', 'possible-start-point']).isRequired,
    position: PropTypes.oneOf(['top', 'bottom']).isRequired,
};

export default PointHint;