import React, { useEffect, useRef, useState } from 'react';
import { connectToGame } from '../../api/ws/socket';
import gsAxios from "../../api/http/axiosConfig";
import './Lobby.css';

const Lobby = ({ token, onGameStart, onSessionIdUpdate, initialSessionId }) => {
    const [sessionId, setSessionId] = useState(initialSessionId);
    const [inputSessionId, setInputSessionId] = useState('');
    const [players, setPlayers] = useState([]);
    const [isReadyToStart, setIsReadyToStart] = useState(false);
    const [copied, setCopied] = useState(false);
    const isConnectingRef = useRef(false);
    const connectionRef = useRef(null);

    const subscribeToLobby = (sessionId) => {
        connectToGame(token, sessionId, (msg) => {
            console.log('Received BoardUpdated message:', msg);
        }, (err) => {
            console.error("SignalR error:", err);
        }, () => {
            console.log('Received GameCanceled message');
            console.warn("Game was canceled.");
        },
            (lobbyDto) => {
            console.log("Received LobbyUpdated message:", lobbyDto);
            if (lobbyDto.players) setPlayers(lobbyDto.players);
            setIsReadyToStart(lobbyDto.readyToStart);
            if (lobbyDto.started) {
                console.log('Game started:', lobbyDto);
                onGameStart();
            }
        }).then(conn => {
            connectionRef.current = conn;
        });
    };

    useEffect(() => {
        if (!initialSessionId) return;

        gsAxios.get(`/lobby/${initialSessionId}`)
            .then(res => {
                const dto = res.data;
                setPlayers(dto.players);
                setIsReadyToStart(dto.readyToStart);
                setSessionId(dto.sessionId);
                onSessionIdUpdate(dto.sessionId);
                subscribeToLobby(dto.sessionId);
            })
            .catch(() => {
                console.warn("Failed to restore lobby.");
            });
    }, [initialSessionId]);

    const handleCreateLobby = async () => {
        if (isConnectingRef.current) return;
        isConnectingRef.current = true;

        try {
            const res = await gsAxios.post('/lobby');
            const dto = res.data;

            setSessionId(dto.sessionId);
            setPlayers(dto.players);
            setIsReadyToStart(dto.readyToStart);
            setInputSessionId('');
            onSessionIdUpdate(dto.sessionId);

            subscribeToLobby(dto.sessionId);
            console.log(`Lobby created with session ID: ${dto.sessionId}`);
        } catch (err) {
            console.error('Error creating lobby:', err);
        } finally {
            isConnectingRef.current = false;
        }
    };

    const handleJoinLobby = async () => {
        if (!inputSessionId.trim() || isConnectingRef.current) return;
        isConnectingRef.current = true;

        try {
            const res = await gsAxios.post(`/lobby/join/${inputSessionId}`);
            const dto = res.data;

            setSessionId(dto.sessionId);
            setPlayers(dto.players);
            setIsReadyToStart(dto.readyToStart);
            setInputSessionId('');
            onSessionIdUpdate(dto.sessionId);

            subscribeToLobby(dto.sessionId);
        } catch {
            alert("Lobby not found or join failed.");
        } finally {
            isConnectingRef.current = false;
        }
    };

    const handleStartGame = async () => {
        try {
            await gsAxios.post(`/lobby/start/${sessionId}`);
            onGameStart();
            console.log('Game started');
        } catch (err) {
            console.error('Error starting game:', err);
        }
    };

    const handleLeaveLobby = async () => {
        try {
            await gsAxios.post(`/lobby/leave/${sessionId}`);

            if (connectionRef.current) {
                await connectionRef.current.stop();
                connectionRef.current = null;
            }

            setSessionId('');
            setPlayers([]);
            setIsReadyToStart(false);
            setInputSessionId('');
        } catch (err) {
            console.error('Error leaving lobby:', err);
        }
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
