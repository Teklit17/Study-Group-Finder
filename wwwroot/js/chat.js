// Function to scroll to the bottom of the chat
function scrollToBottom() {
    const chatBox = document.getElementById("chatBox");
    chatBox.scrollTop = chatBox.scrollHeight; // Set scrollTop to the container's full height
}

// Ensure no duplicate declarations of the connection variable
if (typeof connection === 'undefined') {
    const connection = new signalR.HubConnectionBuilder()
        .withUrl("/StudyHub") // Ensure this matches your backend SignalR Hub endpoint
        .withAutomaticReconnect() // Optional: Automatically reconnect if the connection is dropped
        .build();

    // Start SignalR connection
    connection.start()
        .then(() => console.log("Connected to SignalR Hub"))
        .catch(err => console.error("Error establishing SignalR connection:", err));

    // Handle real-time message updates
    connection.on("ReceiveMessage", (senderId, content) => {
        console.log(`Received message from ${senderId}: ${content}`);

        // Add the new message dynamically
        const messageList = document.getElementById("messageList");
        const newMessage = document.createElement("li");
        newMessage.classList.add("message");
        newMessage.classList.add(senderId === currentUserId ? "sent" : "received");
        newMessage.innerHTML = `<span class="message-content">${content}</span>`;
        messageList.appendChild(newMessage);

        // Scroll to the bottom of the chat when a new message is received
        scrollToBottom();
    });

    // Handle real-time notifications for new messages
    connection.on("NewMessageNotification", (senderId, content) => {
        console.log(`New message notification from ${senderId}: ${content}`);

        // Display a browser alert if the chat page is NOT open
//        if (!window.isChatPageOpen) {
  //          alert(`New message from ${senderId}: ${content}`);
    //    }
    });
}

// Send Message Functionality
async function sendMessage() {
    const content = document.getElementById("messageContent").value.trim();
    if (!content) {
        alert("Please enter a message!");
        return;
    }

    const message = {
        receiverID: receiverId, // Assuming receiverId is passed dynamically
        content: content
    };

    console.log("Sending message:", message);

    try {
        const response = await fetch('/Messages/SendMessage', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
                'Accept': 'application/json'
            },
            body: JSON.stringify(message)
        });

        if (response.ok) {
            console.log("Message sent successfully");
            document.getElementById("messageContent").value = '';
            // Add the sent message to the list
            const messageList = document.getElementById("messageList");
            const newMessage = document.createElement("li");
            newMessage.classList.add("message", "sent");
            newMessage.innerHTML = `<span class="message-content">${content}</span>`;
            messageList.appendChild(newMessage);

            // Scroll to the bottom of the chat after sending a message
            scrollToBottom();
        } else {
            const errorText = await response.text();
            console.error("Failed to send message:", response.status, errorText);
            alert("Error sending message");
        }
    } catch (err) {
        console.error("Fetch error:", err);
        alert("Error sending message");
    }
}

// Add event listener to the Send button
document.getElementById("sendMessageBtn").addEventListener("click", sendMessage);
// Add event listener for the Enter key
document.getElementById("messageContent").addEventListener("keydown", function (event) {
    if (event.key === "Enter" && !event.shiftKey) { // Check if Enter is pressed without Shift
        event.preventDefault(); // Prevent adding a new line in the textarea
        sendMessage(); // Trigger the sendMessage function
    }
});
// Set the chat page as closed when navigating away
window.addEventListener("beforeunload", () => {
    window.isChatPageOpen = false;
});

// Scroll to bottom on page load
document.addEventListener("DOMContentLoaded", () => {
    scrollToBottom();
});
