package cz.stepes.smsgateway

import android.graphics.drawable.Drawable
import android.view.LayoutInflater
import android.view.View
import android.view.ViewGroup
import android.widget.ImageView
import android.widget.TextView
import androidx.appcompat.content.res.AppCompatResources
import androidx.recyclerview.widget.RecyclerView
import cz.stepes.smsgateway.enums.MessageState
import cz.stepes.smsgateway.models.MessageModel
import kotlinx.android.synthetic.main.message.view.*
import java.text.SimpleDateFormat
import java.util.*
import kotlin.collections.ArrayList

class MessagesAdapter : RecyclerView.Adapter<RecyclerView.ViewHolder>() {

    private var data: ArrayList<MessageModel> = ArrayList()

    override fun onCreateViewHolder(parent: ViewGroup, viewType: Int): RecyclerView.ViewHolder {
        return MessageViewHolder(
            LayoutInflater.from(parent.context).inflate(R.layout.message, parent, false)
        )
    }

    override fun onBindViewHolder(holder: RecyclerView.ViewHolder, position: Int) {
        when (holder) {
            is MessageViewHolder -> {
                val message = data[position]

                holder.bind(message)
            }
        }
    }

    override fun getItemCount() = data.size

    /**
     * Submits data to the recycler view
     */
    fun submitList(messagesList: ArrayList<MessageModel>) {
        data = messagesList
    }

    /**
     * Inserts message to the data list
     */
    fun addMessage(message: MessageModel) {
        data.add(0, message)
    }

    class MessageViewHolder constructor(itemView: View) : RecyclerView.ViewHolder(itemView) {

        private val date: TextView = itemView.message_date
        private val id: TextView = itemView.message_id
        private val stateIcon: ImageView = itemView.message_state_icon
        private val stateText: TextView = itemView.message_state_text
        private val recipient: TextView = itemView.message_recipient
        private val content: TextView = itemView.message_content

        fun bind(message: MessageModel) {
            val state = MessageState.fromInt(message.state)

            val formatter = SimpleDateFormat("dd.MM.yyyy HH:mm:ss", Locale.GERMAN);
            val sentAt = formatter.format(Date(message.sentAt))
            date.text = sentAt

            val color = getColor(itemView, message.state)
            val iconDrawable = getIconDrawable(itemView, message.state)

            stateText.setTextColor(color)
            stateIcon.setColorFilter(color)
            stateIcon.setImageDrawable(iconDrawable)
            id.text = message.messageId.toString()
            stateText.text = state?.message
            recipient.text = message.recipient
            content.text = message.content
        }

        private fun getColor(itemView: View, state: Int): Int {
            val context = itemView.context

            return when (state) {
                1 -> context.getColor(R.color.success_green)
                2 -> context.getColor(R.color.error_red)
                else -> context.getColor(R.color.warning_yellow)
            }
        }

        private fun getIconDrawable(itemView: View, state: Int): Drawable? {
            val context = itemView.context

            return when (state) {
                1 -> AppCompatResources.getDrawable(context, R.drawable.ic_baseline_check_24)
                2 -> AppCompatResources.getDrawable(context, R.drawable.ic_baseline_close_24)
                else -> AppCompatResources.getDrawable(
                    context,
                    R.drawable.ic_baseline_file_upload_24
                )
            }
        }
    }
}