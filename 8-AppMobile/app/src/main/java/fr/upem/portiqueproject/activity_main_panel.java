package fr.upem.portiqueproject;

import android.os.SystemClock;
import android.support.v7.app.AppCompatActivity;
import android.os.Bundle;
import android.view.View;
import android.widget.Button;
import android.widget.Chronometer;

public class activity_main_panel extends AppCompatActivity {
    private Button btnstart;
    private Button btnstop;
    long timeWhenStopped = 0;
    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_main_panel);

        final Chronometer chrono = (Chronometer) findViewById(R.id.chrono);
        btnstart=(Button)findViewById(R.id.btnstart);

        btnstart.setOnClickListener(new View.OnClickListener() {
            @Override
            public void onClick(View v) {
                chrono.setBase(SystemClock.elapsedRealtime() + timeWhenStopped);
                chrono.start();

            }
        });


        btnstop=(Button) findViewById(R.id.btnstop);

        btnstop.setOnClickListener(new View.OnClickListener() {
            @Override
            public void onClick(View v) {
                chrono.stop();
               /// chrono.setBase(SystemClock.elapsedRealtime());
                timeWhenStopped = 0;

            }

        });

    }

}
