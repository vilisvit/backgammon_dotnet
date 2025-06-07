import React from "react";

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