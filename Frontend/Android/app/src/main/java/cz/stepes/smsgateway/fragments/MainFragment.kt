package cz.stepes.smsgateway.fragments

import android.app.PendingIntent
import android.content.BroadcastReceiver
import android.content.Context
import android.content.Intent
import android.content.IntentFilter
import android.os.Bundle
import android.telephony.SmsManager
import android.view.LayoutInflater
import android.view.View
import android.view.ViewGroup
import android.widget.Toast
import androidx.appcompat.app.AppCompatActivity
import androidx.fragment.app.Fragment
import androidx.recyclerview.widget.LinearLayoutManager
import com.microsoft.signalr.HubConnectionState
import cz.stepes.smsgateway.MessagesAdapter
import cz.stepes.smsgateway.R
import cz.stepes.smsgateway.enums.MessageState
import cz.stepes.smsgateway.models.MessageModel
import cz.stepes.smsgateway.models.MessageUpdateModel
import cz.stepes.smsgateway.models.SessionToken
import cz.stepes.smsgateway.networking.HttpClient
import cz.stepes.smsgateway.networking.SignalRListener
import kotlinx.android.synthetic.main.fragment_main.*

class MainFragment(val token: SessionToken) : Fragment() {

    private lateinit var messagesAdapter: MessagesAdapter

    private lateinit var signalRListener: SignalRListener

    override fun onCreateView(
        inflater: LayoutInflater, container: ViewGroup?,
        savedInstanceState: Bundle?
    ): View? {
        // Inflate the layout for this fragment
        return inflater.inflate(R.layout.fragment_main, container, false)
    }

    override fun onViewCreated(view: View, savedInstanceState: Bundle?) {
        super.onViewCreated(view, savedInstanceState)

        signalRListener = SignalRListener(
            token.token,
            token.device.deviceId,
            { exception: Exception? -> onConnectionClosed(exception) },
            { messageId: String -> onSendMessage(messageId) }
        )

        initializeComponents()

        getMessages()
    }

    override fun onResume() {
        super.onResume()

        // Shows disconnected from the server, if the connection is no longer active on resume
        if (signalRListener.hubConnection.connectionState != HubConnectionState.CONNECTED) {
            val transaction = parentFragmentManager.beginTransaction()

            transaction.replace(R.id.fragment_view, CantConnectFragment())

            transaction.remove(this)
            transaction.commit()
        }
    }

    /**
     * Handler for disruption of SignalR connection
     */
    private fun onConnectionClosed(exception: Exception?) {
        val transaction = parentFragmentManager.beginTransaction()

        transaction.replace(R.id.fragment_view, CantConnectFragment())

        transaction.addToBackStack(null)
        transaction.commit()

        Toast.makeText(activity, "Připojení bylo ukončeno", Toast.LENGTH_LONG).show()
        onDestroy()
    }

    /**
     * Handler for SendMessage event sent from SignalR server
     */
    private fun onSendMessage(messageId: String) {
        activity?.runOnUiThread {
            try {
                // Gets message from the server by the message ID, return if the response null
                val message = HttpClient.httpGetObject<MessageModel>(
                    HttpClient.Endpoints.GET_MESSAGE,
                    "/" + token.device.deviceId + "/$messageId?token=" + token.token
                ) ?: return@runOnUiThread

                // Adds the message to the recycler view and smoothly scrolls to it
                messagesAdapter.addMessage(message)
                messagesAdapter.notifyItemInserted(0)

                // Sends the SMS message
                sendSmsMessage(message)

                messages_view.smoothScrollToPosition(0)

            } catch (exception: Exception) {
                Toast.makeText(activity, exception.stackTraceToString(), Toast.LENGTH_LONG).show()
            }
        }
    }

    /**
     * Initializes UI components
     */
    private fun initializeComponents() {
        messages_view.apply {
            layoutManager = LinearLayoutManager(context)
            messagesAdapter = MessagesAdapter()
            adapter = messagesAdapter
        }

        connection_id.text = signalRListener.hubConnection.connectionId
    }

    /**
     * Gets messages sent by the device
     */
    private fun getMessages() {
        // Gets list of messages and returns if the result is null
        val messages = HttpClient.httpGetArray<MessageModel>(
            HttpClient.Endpoints.GET_MESSAGES,
            "/" + token.device.deviceId + "?&token=" + token.token
        ) ?: return

        // Passes the list of messages to the recycler view
        messagesAdapter.submitList(messages as ArrayList<MessageModel>)
    }

    /**
     * Sends an SMS message to a certain phone number
     */
    private fun sendSmsMessage(message: MessageModel) {
        val sent = "SMS_SENT"

        val sentIntent =
            PendingIntent.getBroadcast(context, 69, Intent(sent), PendingIntent.FLAG_IMMUTABLE)

        activity?.registerReceiver(object : BroadcastReceiver() {
            override fun onReceive(context: Context, intent: Intent) {
                val endpoint =
                    HttpClient.Endpoints.PUT_MESSAGE + "/" + token.device.deviceId + "/" + message.messageId + "?token=" + token.token

                // Tries to update state of the message on the server
                when (resultCode) {
                    AppCompatActivity.RESULT_OK -> {
                        HttpClient.putObjectAsync(endpoint, MessageUpdateModel(1))

                        message.state = MessageState.SENT.value
                        messagesAdapter.notifyItemChanged(0)
                    }
                    else -> {
                        HttpClient.putObjectAsync(endpoint, MessageUpdateModel(2))

                        message.state = MessageState.FAILED.value
                        messagesAdapter.notifyItemChanged(0)
                    }
                }
            }
        }, IntentFilter(sent))

        val smsManager = SmsManager.getDefault() as SmsManager

        smsManager.sendTextMessage(message.recipient, null, message.content, sentIntent, null)
    }
}