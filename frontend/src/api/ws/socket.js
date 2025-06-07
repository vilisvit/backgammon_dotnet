import SockJS from 'sockjs-client';
import { Client } from '@stomp/stompjs';

let stompClient = null;

export const connectSocket = (token, onConnected, onError) => {
    const socket = new SockJS('http://localhost:8080/ws');
    stompClient = new Client({
        webSocketFactory: () => socket,
        connectHeaders: {
            Authorization: `Bearer ${token}`,
        },
        onConnect: () => {
            console.log('Connected to WebSocket');
            onConnected?.();
        },
        onStompError: (frame) => {
            console.error('STOMP error', frame);
            onError?.(frame);
        },
        debug: (str) => {
            console.log(str);
        },
        reconnectDelay: 5000,
    });

    stompClient.activate();
};

export const sendMessage = (destination, payload) => {
    if (stompClient && stompClient.connected) {
        stompClient.publish({
            destination,
            body: JSON.stringify(payload),
        });
    } else {
        console.warn('WebSocket is not connected');
    }
};

export const subscribe = (destination, callback) => {
    if (stompClient && stompClient.connected) {
        const subscription = stompClient.subscribe(destination, (message) => {
            const body = JSON.parse(message.body);
            callback(body);
        });

        return () => subscription.unsubscribe();
    } else {
        console.warn('WebSocket is not connected');

        return () => {};
    }
};

export const disconnectSocket = () => {
    if (stompClient) {
        stompClient.deactivate();
        stompClient = null;
        console.log('Disconnected from WebSocket');
    }
};