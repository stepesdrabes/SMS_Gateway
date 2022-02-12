package cz.stepes.smsgateway.models

import com.beust.klaxon.Json

data class SessionToken(
    @Json(name = "token") val token: String,
    @Json(name = "device") val device: DeviceModel
)