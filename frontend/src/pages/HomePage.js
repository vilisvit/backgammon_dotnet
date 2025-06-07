import React, {useEffect, useState, useRef} from 'react';
import {useNavigate} from 'react-router-dom';
import gsAxios from '../api/http/axiosConfig';
import 'bootstrap/dist/css/bootstrap.min.css';
import './HomePage.css';
import Comments from "../components/Comments/Comments";
import Board from "../components/Board/Board";
import AddCommentForm from "../components/Comments/AddCommentForm";
import {fetchComments} from "../api/http/comment.service";
import Header from "../components/Header/Header";
import {RatingSelector} from "../components/Rating/RatingSelector";
import {RatingDisplay} from "../components/Rating/RatingDisplay";
import {fetchAverageRating, fetchUserRating} from "../api/http/rating.service";
import Lobby from "../components/Lobby/Lobby";
import Scoreboard from "../components/Scoreboard/Scoreboard";
import {connectToGame} from "../api/ws/signalr";

function HomePage() {
    const [token, setToken] = useState(localStorage.getItem('token') || null);
    const navigate = useNavigate();
    const [gameStarted, setGameStarted] = useState(false);
    const selectedGame = 'backgammon';
    const [username, setUsername] = useState(null);
    const [commentsList, setCommentsList] = useState([]);
    const [averageRating, setAverageRating] = useState(0);
    const [userRating, setUserRating] = useState(0);

    const [gameSessionId, setGameSessionId] = useState(null);
    const [gameUpdates, setGameUpdates] = useState(null);
    const connectedToGameRef = useRef(false);

    useEffect(() => {
        if (gameSessionId && token && !connectedToGameRef.current) {
            connectedToGameRef.current = true;

            connectToGame(token, gameSessionId,
                (boardUpdate) => {
                    setGameUpdates(boardUpdate);
                },
                (error) => {
                    alert(error);
                },
                () => {
                    alert("Game canceled.");
                    setGameStarted(false);
                    connectedToGameRef.current = false;
                });
        }
    }, [gameSessionId, token]);

    useEffect(() => {
        if (token) {
            gsAxios.get('/auth/whoami')
                .then(response => {
                    setUsername(response.data.username);
                })
                .catch(() => {
                    setToken(null); // Clear the token
                    localStorage.removeItem('token'); // Remove token from localStorage
                    navigate('/login'); // Redirect to login page
                });
        }
    }, [token, navigate]);

    useEffect(() => {
        if (username) {
            fetchUserRating(selectedGame, username)
                .then(response => {
                    setUserRating(response.data);
                })
                .catch(error => {
                    console.error('Error fetching user rating:', error);
                });
        }
    }, [username]);

    useEffect(() => {
        if (token) {
            fetchComments(selectedGame)
                .then(response => {
                    setCommentsList(response.data);
                })
                .catch(error => {
                    console.error('Error fetching comments:', error);
                });
            fetchAverageRating(selectedGame)
                .then(response => {
                    setAverageRating(response.data);
                })
                .catch(error => {
                    console.error('Error fetching average rating:', error);
                });
        }
    }, [selectedGame, token]);

    const handleBackToLobby = () => {
        setGameStarted(false);
    }

    const handleLogout = () => {
        setToken(null);
        localStorage.removeItem('token');
        navigate('/login'); // Redirect to login page
    };

    const handleCommentAdded = () => {
        if (token) {
            fetchComments(selectedGame)
                .then(response => {
                    setCommentsList(response.data);
                })
                .catch(error => {
                    console.error('Error fetching comments:', error);
                });
        }
    };

    const handleRatingChanged = () => {
        if (token) {
            fetchAverageRating(selectedGame)
                .then(response => {
                    setAverageRating(response.data);
                })
                .catch(error => {
                    console.error('Error fetching average rating:', error);
                });
            fetchUserRating(selectedGame, username)
                .then(response => {
                    setUserRating(response.data);
                })
                .catch(error => {
                    console.error('Error fetching user rating:', error);
                });
        }
    };

    return (
        <div className="HomePage">
            <Header username={username} onLogout={handleLogout}/>
            <h2 className="selected-game-label">{selectedGame}</h2>
            <div className="game-container">
                <div className="board-container">
                    {!gameStarted ? (
                        <Lobby
                            token={token}
                            onGameStart={() => setGameStarted(true)}
                            onSessionIdUpdate={(id) => setGameSessionId(id)}
                        />
                    ) : (
                        <Board gameSessionId={gameSessionId} gameUpdates={gameUpdates} username={username} onGameFinish={handleBackToLobby}/>
                    )}
                </div>
                <Scoreboard
                    game={selectedGame}
                    username={username}
                />
            </div>
            <div className="divider"/>
            <div className="rating-section">
                <div className="rating-row">
                    <div className="rating-cell">
                        <label className="rating-label">Your Rating:</label>
                    </div>
                    <div className="rating-cell">
                        <RatingSelector currentRating={userRating} game={selectedGame} onRate={handleRatingChanged}/>
                    </div>
                </div>
                <div className="rating-row">
                    <div className="rating-cell">
                        <label className="rating-label">Average Rating:</label>
                    </div>
                    <div className="rating-cell">
                        <RatingDisplay rating={averageRating}/>
                    </div>
                </div>
            </div>
            <div className="comments-section">
                <h2 className="comments-section-label">Comments</h2>
                <AddCommentForm
                    game={selectedGame}
                    onCommentAdded={handleCommentAdded}
                />
                <Comments
                    game={selectedGame}
                    commentsList={commentsList}
                />
            </div>
        </div>
    );
}

export default HomePage;
