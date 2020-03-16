package fr.upem.portiqueproject;

import android.app.ProgressDialog;
import android.content.Context;
import android.net.ConnectivityManager;
import android.net.NetworkInfo;
import android.os.AsyncTask;
import android.os.SystemClock;
import android.support.v7.app.AppCompatActivity;
import android.os.Bundle;
import android.util.Log;
import android.view.View;
import android.widget.Button;
import android.widget.Chronometer;
import android.widget.Toast;

import com.example.httpservice.getService;

import org.json.JSONArray;
import org.json.JSONException;
import org.json.JSONObject;

import java.io.BufferedInputStream;
import java.io.IOException;
import java.io.InputStream;
import java.io.OutputStream;
import java.net.HttpURLConnection;
import java.net.MalformedURLException;
import java.net.ProtocolException;
import java.net.URL;
import okhttp3.OkHttpClient;
import okhttp3.Request;
import okhttp3.Response;


public class activity_main_panel extends AppCompatActivity {
    private String TAG = MainActivity.class.getSimpleName();
    private static String url2 = "https://www.google.com";
    private Button btnstart;
    private Button btnstop;
    long timeWhenStopped = 0;

    private getService GetService;
    private InputStream in;
    private OutputStream out;
    private int responseCode = 0;

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_main_panel);

        final Chronometer chrono = (Chronometer) findViewById(R.id.chrono);
        btnstart=(Button)findViewById(R.id.btnstart);
        btnstop=(Button) findViewById(R.id.btnstop);


        btnstart.setOnClickListener(new View.OnClickListener() {

            @Override
            public void onClick(View v) {

                new HttpTask().execute("http://192.168.43.199:8080/poissons"); // Send HTTP request

                    chrono.setBase(SystemClock.elapsedRealtime() + timeWhenStopped);
                    chrono.start();


            }
        });

        btnstop.setOnClickListener(new View.OnClickListener() {
            @Override
            public void onClick(View v) {

                    new HttpTask().execute("http://192.168.43.199:8080/poissons"); // Send HTTP request

                    chrono.stop();
                    timeWhenStopped = 0;

            }

        });

    }

    private class HttpTask extends AsyncTask<String, Void, String> {
        @Override
        protected String doInBackground(String... strURLs) {
            HttpURLConnection urlConnection = null;
            String response = null;
            try{

                URL url = new URL(strURLs[0]);
                urlConnection = (HttpURLConnection) url.openConnection();
                urlConnection.setRequestMethod("GET");

                responseCode = urlConnection .getResponseCode();
                if (responseCode == HttpURLConnection.HTTP_OK) {  // 200
                    return "OK (" + responseCode + ")";
                } else {
                    return "Fail (" + responseCode + ")";
                }

            }
            catch (Exception e){
                return"Erreur URL connexion " + e;
            } finally {
                if(urlConnection != null ) urlConnection.disconnect();
            }
        }

        // Displays the result of the AsyncTask.
        // The String result is passed from doInBackground().
        @Override
        protected void onPostExecute(String result) {
            super.onPostExecute(result);
            Log.e(TAG, "Reponse: " + result);

        }
    }


    private boolean isConnected() {
        ConnectivityManager connectivityManager =
                (ConnectivityManager) getSystemService(Context.CONNECTIVITY_SERVICE);
        NetworkInfo networkInfo = connectivityManager.getActiveNetworkInfo();
        return networkInfo != null && networkInfo.isConnected();
    }

}
