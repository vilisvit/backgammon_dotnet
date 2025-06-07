import React, {useEffect, useRef, useState} from 'react';
import {connectSocket, subscribe} from '../../api/ws/socket';
import gsAxios from "../../api/http/axiosConfig";
import './Lobby.css';

const Lobby = ({ token, onGameStart, onSessionIdUpdate, unsubscribeRef, initialSessionId }) => {
    const [sessionId, setSessionId] = useState(initialSessionId);
    const [inputSessionId, setInputSessionId] = useState('');
    const [players, setPlayers] = useState([]);
    const [isReadyToStart, setIsReadyToStart] = useState(false);
    const [copied, setCopied] = useState(false);

    const isConnectingRef = useRef(false);
    const stompClientRef = useRef(null);
    const localUnsubscribeRef = useRef(null);

    const subscribeToLobby = (sessionId) => {
        if (localUnsubscribeRef.current) {
            localUnsubscribeRef.current();
            localUnsubscribeRef.current = null;
        }

        localUnsubscribeRef.current = subscribe(`/topic/lobby/${sessionId}`, (msg) => {
            try {
                if (msg.players) setPlayers(msg.players);
                setIsReadyToStart(msg.readyToStart);
                if (msg.started) {
                    console.log('Game started:', msg);
                    onGameStart();
                }
            } catch (e) {
                console.error('Failed to parse lobby message:', msg);
            }
        });

        if (unsubscribeRef.current) {
            unsubscribeRef.current = localUnsubscribeRef.current;
        }
    };

    useEffect(() => {
        if (!initialSessionId) return;

        gsAxios.get(`/lobby/${initialSessionId}`)
            .then(res => {
                const dto = res.data;
                setPlayers(dto.players);
                setIsReadyToStart(dto.readyToStart);
                subscribeToLobby(dto.sessionId);
            })
            .catch(() => {
                console.warn("Failed to restore lobby.");
            });
    }, [initialSessionId]);


    const handleCreateLobby = () => {
        if (isConnectingRef.current) return;
        isConnectingRef.current = true;

        connectSocket(token, (client) => {
            stompClientRef.current = client;

            gsAxios.post('/lobby', {})
                .then((res) => {
                    const dto = res.data;
                    setSessionId(dto.sessionId);
                    onSessionIdUpdate(dto.sessionId);
                    subscribeToLobby(dto.sessionId);

                    setPlayers(dto.players);
                    setIsReadyToStart(dto.readyToStart);
                    setInputSessionId('');
                    console.log(`Lobby created with session ID: ${dto.sessionId}`);
                })
                .catch((err) => {
                    console.error('Error creating lobby:', err);
                })
                .finally(() => {
                    isConnectingRef.current = false;
                });

        }, (error) => {
            console.error('WebSocket connection error:', error);
            isConnectingRef.current = false;
        });
    };

    const handleJoinLobby = () => {
        if (!inputSessionId.trim() || isConnectingRef.current) return;
        isConnectingRef.current = true;

        connectSocket(token, (client) => {
            stompClientRef.current = client;

            gsAxios.post(`/lobby/join/${inputSessionId}`, {})
                .then((res) => {
                    const dto = res.data;
                    subscribeToLobby(dto.sessionId);
                    setSessionId(dto.sessionId);
                    onSessionIdUpdate(dto.sessionId);

                    setPlayers(dto.players);
                    setIsReadyToStart(dto.readyToStart);
                    setInputSessionId('');
                })
                .catch(() => {
                    alert("Lobby not found or join failed.");
                })
                .finally(() => {
                    isConnectingRef.current = false;
                });

        }, (error) => {
            console.error('WebSocket connection error:', error);
            isConnectingRef.current = false;
        });
    };

    const handleStartGame = () => {
        gsAxios.post(`/lobby/start/${sessionId}`, {})
            .then(() => {
                onGameStart();
                console.log('Game started');
            })
            .catch((err) => {
                console.error('Error starting game:', err);
            });
    };

    const handleLeaveLobby = () => {

        gsAxios.post(`/lobby/leave/${sessionId}`, {})
            .then(() => {
                if (localUnsubscribeRef.current) {
                    localUnsubscribeRef.current();
                    localUnsubscribeRef.current = null;
                }

                setSessionId('');
                setPlayers([]);
                setIsReadyToStart(false);
                setInputSessionId('');
                if (unsubscribeRef.current) {
                    unsubscribeRef.current = null;
                }
            })
            .catch((err) => {
                console.error('Error leaving lobby:', err);
            }
        );
    };

    return (
        <div className="lobby-container">
            <h3 className="lobby-header">Lobby</h3>
            <div className="lobby-actions">
                <button className="create-lobby-button" onClick={handleCreateLobby} disabled={!!sessionId}>Create</button>
                <input className="lobby-input"
                    placeholder="Join session ID"
                    value={inputSessionId}
                    onChange={(e) => setInputSessionId(e.target.value)}
                    disabled={!!sessionId}
                />
                <button className="join-lobby-button" onClick={handleJoinLobby} disabled={!!sessionId}>Join</button>
            </div>

            {sessionId && (
                <div className="lobby-info">
                    <div className="session-id-container">
                        <strong>Session ID:</strong>
                        <span className="session-id-text">{sessionId}</span>
                        <button className="copy-button" onClick={() => {
                            navigator.clipboard.writeText(sessionId);
                            setCopied(true);
                            setTimeout(() => setCopied(false), 2000);
                        }}>
                            {copied ? "Copied!" : "Copy"}
                        </button>
                    </div>

                    <h4>Players:</h4>
                    <div className="lobby-player-list">
                        {players.map((p, i) => (
                            <div className="lobby-player-frame" key={i}>
                                {p}
                            </div>
                        ))}
                    </div>

                    <div className="lobby-status">
                        {isReadyToStart
                            ? "✅ Ready to start!"
                            : "⏳ Waiting for players..."}
                    </div>

                    {players.length === 2 && (
                        <button className="lobby-start-button" onClick={handleStartGame}>
                            Start Game
                        </button>
                    )}

                    <button className="leave-lobby-button" onClick={handleLeaveLobby}>
                        Leave Lobby
                    </button>
                </div>
            )}
        </div>
    );
};

export default Lobby;