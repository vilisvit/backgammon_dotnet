import React, { useState } from 'react';
import './Rating.css';
import PropTypes from "prop-types";
import {setRating} from "../../api/http/rating.service";
import {Star} from "./Star";


export const RatingSelector = ({ currentRating, game, onRate }) => {
    const [hovered, setHovered] = useState(0);

    const handleClick = (value) => {
        setRating(game, value)
            .then(() => {
                if (onRate) {
                    onRate(value);
                }
            })
            .catch((error) => {
                console.error('Error setting rating:', error);
            });
    };

    return (
        <div className="rating-container">
            {[1, 2, 3, 4, 5].map((i) => (
                <Star
                    key={i}
                    filled={i <= (hovered || currentRating)}
                    onClick={() => handleClick(i)}
                    onMouseEnter={() => setHovered(i)}
                    onMouseLeave={() => setHovered(0)}
                />
            ))}
        </div>
    );
};

RatingSelector.propTypes = {
    currentRating: PropTypes.number,
    game: PropTypes.string.isRequired,
    onRate: PropTypes.func.isRequired,
}
