# SpotifyStartStop

The goal of this application is to start or stop playback of spotify based on if something else is playing.
To work properly spotify client and other app that we check for sound playback need to be on two different virtual audio devices.

Application every second checks if audio device is outputting sound if it is the program sends API request for spotify to pause playback,
when audio device is not outputting sound there is another request for spotify to start playback.

One exception is when we manually stop playback of spotify then only manual start will unpause playback.

# Roadmap
Application is bare bones right now really made for one configuration. 
In future I hope to add GUI when we can select which audio device is sampled for sound with a list of available devices to eliminate need of looking for your device id.
