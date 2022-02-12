package cz.stepes.smsgateway.models

import com.beust.klaxon.Json

data class DeviceModel(
    @Json(name = "deviceId") val deviceId: String,
    @Json(name = "vendor") val vendor: String,
    @Json(name = "model") val model: String,
    @Json(name = "osVersion") val osVersion: String
)