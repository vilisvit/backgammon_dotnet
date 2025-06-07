import React from "react";
import PropTypes from "prop-types";

export const RatingDisplay = ({ rating }) => (
    <div className="rating-display">
        {[1, 2, 3, 4, 5].map((i) => (
            <span key={i} className={`display-star ${i <= rating ? 'filled' : ''}`}>
        ★
      </span>
        ))}
      </div>
);

RatingDisplay.propTypes = {
    rating: PropTypes.number.isRequired,
}