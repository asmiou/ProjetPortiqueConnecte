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

import java.io.BufferedInputStream;
import java.io.IOException;
import java.io.InputStream;
import java.io.OutputStream;
import java.net.HttpURLConnection;
import java.net.URL;
import okhttp3.OkHttpClient;
import okhttp3.Request;
import okhttp3.Response;


public class activity_main_panel extends AppCompatActivity {
    private Button btnstart;
    private Button btnstop;
    long timeWhenStopped = 0;

    private getService GetService;
    private InputStream in;
    private OutputStream out;

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

                if (!isConnected()) {
                    Toast.makeText(activity_main_panel.this, "Aucune connexion à internet", Toast.LENGTH_SHORT).show();
                    return;
                }

                new FetchTask().execute("https://www.google.com");
                chrono.setBase(SystemClock.elapsedRealtime() + timeWhenStopped);
                chrono.start();

            }
        });

        btnstop.setOnClickListener(new View.OnClickListener() {
            @Override
            public void onClick(View v) {

                if (!isConnected()) {
                    Toast.makeText(activity_main_panel.this, "Aucune connexion à internet", Toast.LENGTH_SHORT).show();
                    return;
                }

                new FetchTask().execute("https://www.facebook.com");
                chrono.stop();
                timeWhenStopped = 0;





            }

        });

    }
    private class FetchTask extends AsyncTask<String, Void, String> {

        @Override
        protected void onPreExecute() {
            super.onPreExecute();
        }

        @Override
        protected String doInBackground(String... strings) {
            OkHttpClient client = new OkHttpClient();
            String stringUrl = strings[0];
            Request request = new Request.Builder().url(stringUrl).build();
            try {
                Response response = client.newCall(request).execute();
                return response.body().string();
            } catch (IOException e) {
                return null;
            }
        }

        @Override
        protected void onPostExecute(String s) {
            super.onPostExecute(s);
            if (s == null) {
                Toast.makeText(activity_main_panel.this, "Link error ", Toast.LENGTH_SHORT).show();
            } else {
                Toast.makeText(activity_main_panel.this, "Connexion Bien Etabli", Toast.LENGTH_SHORT).show();
            }
        }
    }

    private boolean isConnected() {
        ConnectivityManager connectivityManager =
                (ConnectivityManager) getSystemService(Context.CONNECTIVITY_SERVICE);
        NetworkInfo networkInfo = connectivityManager.getActiveNetworkInfo();
        return networkInfo != null && networkInfo.isConnected();
    }

}
