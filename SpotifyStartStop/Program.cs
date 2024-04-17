using NAudio.CoreAudioApi;

namespace SpotifyStartStop;

class Program
{
    static async Task Main(string[] args)
    {


        MMDeviceEnumerator enumerator = new MMDeviceEnumerator();
        MMDevice auxDevice = enumerator.GetDevice("{0.0.0.00000000}.{4f3656f5-3e0a-4c06-b36c-7a8bb40e02b5}");
        bool isMePause = false;

        SpotifyController spotifyController = new();
        await spotifyController.AuthenticateUserAsync();
        if (spotifyController.spotify != null)
        {
            Timer timer = new Timer(async _ =>
            {
                var peakValue = auxDevice.AudioMeterInformation.PeakValues;

                Console.WriteLine($"Peak value: {peakValue[0]}");
                if (auxDevice.AudioMeterInformation.PeakValues[0] > 0.0)
                {
                    if (await spotifyController.IsSpotifyPlaying())
                    {
                        try
                        {
                            await spotifyController.spotify.Player.PausePlayback();
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex.Message);
                        }
                        isMePause = true;
                        await spotifyController.RefreshAccessTokenAsync();
                    }
                }
                else if (!await spotifyController.IsSpotifyPlaying() && isMePause)
                {
                    await spotifyController.spotify.Player.ResumePlayback();
                    isMePause = false;
                    await spotifyController.RefreshAccessTokenAsync();
                }

            }, null, 0, 1000);

            Console.ReadLine();

            timer.Dispose();
        }

    }
}