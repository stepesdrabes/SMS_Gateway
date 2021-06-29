package cz.stepes.smsgateway.networking

import com.beust.klaxon.Klaxon
import okhttp3.*
import okhttp3.MediaType.Companion.toMediaType
import okhttp3.RequestBody.Companion.toRequestBody
import java.io.IOException

class HttpClient {

    object Endpoints {
        const val URL = "http://88.103.3.15:5000/api/"
        const val HUB_URL = "http://88.103.3.15:5000/connectionHub"
        const val POST_DEVICE = "devices"
        const val GET_MESSAGES = "messages"
        const val GET_MESSAGE = "message"
        const val PUT_MESSAGE = "message"
    }

    companion object {

        val client = OkHttpClient()

        val JSON: MediaType = "application/json; charset=utf-8".toMediaType()

        /**
         * Gets a generic object from a URL endpoint
         */
        inline fun <reified T> httpGetObject(
            endpoint: String,
            parameters: String
        ): T? {
            // Creates request to an EndPoint with specified parameters
            val request = Request.Builder()
                .url(Endpoints.URL + endpoint + parameters)
                .build()

            client.newCall(request).execute().use { response ->
                if (!response.isSuccessful) {
                    throw IOException("Unexpected code $response")
                }

                // Returns generic object parsed from the JSON
                return Klaxon().parse<T>(response.body!!.string())
            }
        }

        /**
         * Gets list of generic objects from a URL endpoint
         */
        inline fun <reified T> httpGetArray(
            endpoint: String,
            parameters: String
        ): List<T>? {
            // Creates request to an EndPoint with specified parameters
            val request = Request.Builder()
                .url(Endpoints.URL + endpoint + parameters)
                .build()

            client.newCall(request).execute().use { response ->
                if (!response.isSuccessful) {
                    throw IOException("Unexpected code $response")
                }

                // Returns generic object parsed from the JSON
                return Klaxon().parseArray(response.body!!.string())
            }
        }

        /**
         * Sends POST request to a certain URL endpoint with certain data
         */
        inline fun <reified T> postObject(
            endpoint: String,
            instance: Any
        ): T? {
            // Converts the object to JSON string
            val json = Klaxon().toJsonString(instance)

            val body: RequestBody = json.toRequestBody(JSON)

            val request: Request = Request.Builder()
                .url(Endpoints.URL + endpoint)
                .post(body)
                .build()

            client.newCall(request).execute().use { response ->
                if (!response.isSuccessful) {
                    throw IOException("Unexpected code $response")
                }

                // Returns generic object parsed from the JSON
                return Klaxon().parse<T>(response.body!!.string())
            }
        }

        /**
         * Sends PUT request to a certain URL endpoint with certain data
         */
        fun putObjectAsync(
            endpoint: String,
            instance: Any
        ) {
            // Converts the object to JSON string
            val json = Klaxon().toJsonString(instance)

            val body: RequestBody = json.toRequestBody(JSON)

            val request: Request = Request.Builder()
                .url(Endpoints.URL + endpoint)
                .put(body)
                .build()

            // Creates a new request and sends it to a queue
            client.newCall(request).enqueue(object : Callback {
                override fun onFailure(call: Call, e: IOException) {}

                override fun onResponse(call: Call, response: Response) {
                    response.use {
                        if (!response.isSuccessful) {
                            throw IOException("Unexpected code $response")
                        }
                    }
                }
            })
        }
    }
}