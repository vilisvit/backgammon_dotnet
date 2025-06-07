import React, { useEffect, useState } from 'react';
import PropTypes from 'prop-types';
import { fetchTopScores } from '../../api/http/score.service';
import './Scoreboard.css';

function Scoreboard({ game, username }) {
    const [scores, setScores] = useState([]);
    const [error, setError] = useState('');

    useEffect(() => {
        if (game) {
            fetchTopScores(game)
                .then((response) => {
                    setScores(response.data);
                })
                .catch((err) => {
                    console.error('Error fetching scores:', err);
                    setError('Failed to load scores. Please try again later.');
                });
        }
    }, [game]);

    return (
        <div className="scoreboard-container">
            <h2 className="scoreboard-title">Top Scores</h2>
            {error && <p className="scoreboard-error">{error}</p>}
            <table className="scoreboard-table">
                <thead>
                <tr>
                    <th>Rank</th>
                    <th>Player</th>
                    <th>Score</th>
                </tr>
                </thead>
                <tbody>
                {scores.map((score, index) => (
                    <tr
                        key={`score-${index}`}
                        className={score.player === username ? 'highlight-row' : ''}
                    >
                        <td>{index + 1}</td>
                        <td>{score.player}</td>
                        <td>{score.points}</td>
                    </tr>
                ))}
                </tbody>
            </table>
        </div>
    );
}

Scoreboard.propTypes = {
    game: PropTypes.string.isRequired,
    username: PropTypes.string.isRequired,
};

export default Scoreboard;