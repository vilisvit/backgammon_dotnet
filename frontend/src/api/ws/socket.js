import * as signalR from '@microsoft/signalr';

let connection;

export const connectToGame = async (token, sessionId, onBoardUpdate, onError, onGameCancel) => {
    connection = new signalR.HubConnectionBuilder()
        .withUrl(`http://localhost:8080/hubs/backgammon?sessionId=${sessionId}`, {
            accessTokenFactory: () => token
        })
        .withAutomaticReconnect()
        .build();

    connection.on("GameUpdated", onBoardUpdate);
    connection.on("Error", onError);
    connection.on("GameCanceled", onGameCancel);

    try {
        await connection.start();
        console.log("SignalR connected");
    } catch (err) {
        console.error("SignalR connection error: ", err);
    }
};

export const sendRollDice = async (sessionId) => {
    await connection.invoke("RollDice", sessionId);
};

export const sendPointClick = async (sessionId, pointId) => {
    await connection.invoke("OnClick", sessionId, pointId);
};

export const sendOffBoardClick = async (sessionId) => {
    await connection.invoke("OnOffBoardClick", sessionId);
};
