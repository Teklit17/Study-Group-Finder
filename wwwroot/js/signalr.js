const connection = new signalR.HubConnectionBuilder()
    .withUrl("/StudyHub")
    .withAutomaticReconnect()
    .build();

connection.start()
    .then(() => console.log("Connected to SignalR Hub"))
    .catch(err => console.error("Error connecting to SignalR Hub:", err));

// Handle real-time message notifications
connection.on("ReceiveMessage", (senderId, content) => {
    alert(`New message from ${senderId}: ${content}`);
});

export default connection;
