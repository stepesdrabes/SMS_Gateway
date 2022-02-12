package cz.stepes.smsgateway.fragments

import android.os.Bundle
import androidx.fragment.app.Fragment
import android.view.LayoutInflater
import android.view.View
import android.view.ViewGroup
import cz.stepes.smsgateway.R
import kotlinx.android.synthetic.main.fragment_cant_connect.*

class CantConnectFragment : Fragment() {

    override fun onCreateView(
        inflater: LayoutInflater, container: ViewGroup?,
        savedInstanceState: Bundle?
    ): View? {
        // Inflate the layout for this fragment
        return inflater.inflate(R.layout.fragment_cant_connect, container, false)
    }

    override fun onViewCreated(view: View, savedInstanceState: Bundle?) {
        super.onViewCreated(view, savedInstanceState)

        // Gets called when the retry button is pressed, tries to reconnect to the server
        reconnect.setOnClickListener {
            val transaction = parentFragmentManager.beginTransaction()

            transaction.replace(R.id.fragment_view, ConnectingFragment())

            transaction.addToBackStack(null)
            transaction.commit()

            onDestroy()
        }
    }
}