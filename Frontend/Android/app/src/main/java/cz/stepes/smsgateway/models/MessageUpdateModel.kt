package cz.stepes.smsgateway.models

import com.beust.klaxon.Json

data class MessageUpdateModel(
    @Json(name = "state") val state: Int
)