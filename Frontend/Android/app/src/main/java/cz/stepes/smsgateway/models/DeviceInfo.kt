package cz.stepes.smsgateway.models

data class DeviceInfo(
    val vendor: String,
    val model: String,
    val osVersion: String
)