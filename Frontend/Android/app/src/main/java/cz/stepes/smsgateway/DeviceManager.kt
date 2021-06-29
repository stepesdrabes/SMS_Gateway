package cz.stepes.smsgateway

import android.content.Context
import android.os.Build
import androidx.appcompat.app.AppCompatActivity
import cz.stepes.smsgateway.models.DeviceInfo
import java.util.*

class DeviceManager {

    companion object {

        /**
         * Gets the the information about the device
         */
        fun getDeviceInfo() = DeviceInfo(Build.MANUFACTURER, Build.MODEL, Build.VERSION.RELEASE)

        /**
         * Gets unique device ID
         */
        fun getDeviceId(activity: AppCompatActivity): String {
            return getPreferenceString(
                activity,
                activity.getString(R.string.pref_device_id),
                UUID.randomUUID().toString()
            )
        }

        /**
         * Gets device secret key
         */
        fun getSecretKeyId(activity: AppCompatActivity): String {
            return getPreferenceString(
                activity,
                activity.getString(R.string.pref_device_id),
                UUID.randomUUID().toString()
            )
        }

        private fun getPreferenceString(
            activity: AppCompatActivity,
            preference: String,
            default: String
        ): String {
            // Gets shared preferences for the app
            val sharedPreferences = activity.getPreferences(Context.MODE_PRIVATE)

            // Gets the desired data
            val data =
                sharedPreferences.getString(preference, null)

            if (data != null) {
                return data
            }

            val editor = sharedPreferences.edit()

            // Saves default value to shared preferences
            editor.putString(preference, default).apply()

            return default
        }
    }
}