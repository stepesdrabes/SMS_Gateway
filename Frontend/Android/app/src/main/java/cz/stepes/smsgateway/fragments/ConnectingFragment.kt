package cz.stepes.smsgateway.fragments

import android.os.Bundle
import android.view.LayoutInflater
import android.view.View
import android.view.ViewGroup
import android.widget.Toast
import androidx.appcompat.app.AppCompatActivity
import androidx.fragment.app.Fragment
import cz.stepes.smsgateway.DeviceManager
import cz.stepes.smsgateway.R
import cz.stepes.smsgateway.models.DeviceModel
import cz.stepes.smsgateway.models.SessionToken
import cz.stepes.smsgateway.networking.HttpClient

class ConnectingFragment : Fragment() {

    private var token: SessionToken? = null

    override fun onCreateView(
        inflater: LayoutInflater, container: ViewGroup?,
        savedInstanceState: Bundle?
    ): View? {
        // Inflate the layout for this fragment
        return inflater.inflate(R.layout.fragment_connecting, container, false)
    }

    override fun onViewCreated(view: View, savedInstanceState: Bundle?) {
        super.onViewCreated(view, savedInstanceState)

        val transaction = parentFragmentManager.beginTransaction()

        // Tries to connect to the server
        try {
            registerDevice()

            transaction.replace(R.id.fragment_view, MainFragment(token!!))
        } catch (exception: Exception) {
            transaction.replace(R.id.fragment_view, CantConnectFragment())

            Toast.makeText(context, exception.toString(), Toast.LENGTH_LONG).show()
        } finally {
            transaction.addToBackStack(null)
            transaction.commit()
        }
    }

    /**
     * Registers the device on the server with HTTP requests
     */
    private fun registerDevice() {
        // Gets the device information for device registration
        val deviceId = DeviceManager.getDeviceId(activity as AppCompatActivity)
        val deviceInfo = DeviceManager.getDeviceInfo()

        val deviceModel =
            DeviceModel(deviceId, deviceInfo.vendor, deviceInfo.model, deviceInfo.osVersion)

        // Sends the post request with the device to the server
        token = HttpClient.postObject<SessionToken>(HttpClient.Endpoints.POST_DEVICE, deviceModel)

        if (token == null) {
            throw Exception("Device wasn't registered")
        }
    }
}