package cz.stepes.smsgateway.networking

import com.microsoft.signalr.HubConnection
import com.microsoft.signalr.HubConnectionBuilder
import com.microsoft.signalr.HubConnectionState

class SignalRListener(token: String, deviceId: String, private val onClosed: (Exception?) -> Unit, private val onSendMessage: (String) -> Unit) {

    var hubConnection: HubConnection = HubConnectionBuilder.create(HttpClient.Endpoints.HUB_URL)
        .withHeader("TOKEN", token)
        .withHeader("DEVICE_ID", deviceId).build()

    private var connected: Boolean = false

    init {
        // Registers listeners
        registerOnClosed()
        onSendMessage()

        // Connects to SignalR
        connect()
    }

    /**
     * Connect to the server
     */
    private fun connect() {
        hubConnection.start().blockingAwait()

        if (hubConnection.connectionState == HubConnectionState.CONNECTED) {
            connected = true
        }
    }

    /**
     * Registers handler for disconnecting from the server
     */
    private fun registerOnClosed() {
        hubConnection.onClosed { exception: Exception? ->
            onClosed(exception)

            connected = false
        }
    }

    /**
     * Registers SendMessage handler
     */
    private fun onSendMessage() {
        hubConnection.on("SendMessage", { messageId: String ->
            onSendMessage(messageId)
        }, String::class.java)
    }
}