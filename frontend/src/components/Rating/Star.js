import React from "react";
import PropTypes from "prop-types";

export const Star = ({ filled, onClick, onMouseEnter, onMouseLeave }) => (
    <span
        className={`star ${filled ? 'filled' : ''}`}
        onClick={onClick}
        onMouseEnter={onMouseEnter}
        onMouseLeave={onMouseLeave}
    >
    ★
  </span>
);

Star.propTypes = {
    filled: PropTypes.bool.isRequired,
    onClick: PropTypes.func,
    onMouseEnter: PropTypes.func,
    onMouseLeave: PropTypes.func,
}