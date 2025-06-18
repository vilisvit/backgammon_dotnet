import React, { useState, useEffect } from 'react';
import PropTypes from 'prop-types';

function Dice({ style, diceValue: initialDiceValue }) {
    const [diceValue, setDiceValue] = useState(initialDiceValue);

    useEffect(() => {
        setDiceValue(initialDiceValue);
    }, [initialDiceValue]);

    const diceStyle = {
        ...style,
        backgroundImage: diceValue !== null ? `url('/assets/images/dice/${diceValue}.svg')` : 'none',
    };

    return (
        <div className="dice" style={diceStyle}>
            {diceValue === null && <div className="dice-loading">Loading...</div>}
        </div>
    );
}

Dice.propTypes = {
    diceValue: PropTypes.number,
    style: PropTypes.object,
};

export default Dice;