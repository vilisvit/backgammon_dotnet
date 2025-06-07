import React from 'react';
import PropTypes from 'prop-types';
import './EndGameLabel.css';

function EndGameLabel({ winner, onClick }) {
    return (
        <div className="end-game-label" onClick={onClick}>
            <div className="end-game-message">
                {winner} wins!
            </div>
        </div>
    );
}

EndGameLabel.propTypes = {
    winner: PropTypes.string.isRequired,
};

export default EndGameLabel;