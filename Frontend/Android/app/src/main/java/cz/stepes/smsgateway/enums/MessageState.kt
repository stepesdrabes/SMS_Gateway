package cz.stepes.smsgateway.enums

enum class MessageState(val value: Int, val message: String) {
    WAITING(0, "Odesílání"),
    SENT(1, "Odesláno"),
    FAILED(2, "Chyba");

    companion object {
        private val map = values().associateBy(MessageState::value)

        /**
         * Gets the message state from a number
         */
        fun fromInt(type: Int) = map[type]
    }
}