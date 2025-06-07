import React, { useEffect, useState } from 'react';
import PropTypes from 'prop-types';
import PointHint from "../PointHint/PointHint";

function BarPart({ position, barPartData: initialBarPartData }) {
    const [barPartData, setBarPartData] = useState(initialBarPartData);

    useEffect (() => {
        setBarPartData(initialBarPartData);
    } , [initialBarPartData]);

    const colorClass = barPartData.checkersColor === 'white' ? 'white-checker' : 'black-checker'

    return (
        <div className={`bar-${position}`}>
            {Array.from({ length: barPartData.checkersCount }).map((_, index) => (
                <div key={index} className={`${colorClass}`}></div>
            ))}
            {barPartData.selected && <PointHint type={'selected'} position={position}/>}
        </div>
    );
}

BarPart.propTypes = {
    position: PropTypes.oneOf(['top', 'bottom']).isRequired,
    barPartData: PropTypes.shape({
        checkersCount: PropTypes.number.isRequired,
        checkersColor: PropTypes.string.isRequired,
        selected: PropTypes.bool.isRequired,
    }).isRequired,
};

export default BarPart;