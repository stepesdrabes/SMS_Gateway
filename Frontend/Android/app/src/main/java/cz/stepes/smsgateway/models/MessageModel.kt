package cz.stepes.smsgateway.models

import com.beust.klaxon.Json

data class MessageModel(
    @Json(name = "messageId") val messageId: Long,
    @Json(name = "deviceId") val deviceId: String,
    @Json(name = "recipient") val recipient: String,
    @Json(name = "content") val content: String,
    @Json(name = "state") var state: Int,
    @Json(name = "sentAt") val sentAt: Long,
    @Json(name = "proceededAt") val proceededAt: Long?
)