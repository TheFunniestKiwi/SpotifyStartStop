using SpotifyAPI.Web.Auth;
using SpotifyAPI.Web;
using System.Net;
using static System.Formats.Asn1.AsnWriter;


namespace SpotifyStartStop
{
    public class SpotifyController
    {
        private static string _clientId = "1874be34159348c09311335348c609dc"; 
        private static string _clientSecret = "78a74b7d52ad4a4da264996f3df67d04"; 
        private static Uri _redirectUri = new Uri("http://localhost:8888/callback"); 
        private static string refreshCode = "";
        public SpotifyClient? spotify;

        public SpotifyController()
        {
            refreshCode = StorageController.LoadToken();
        }

        public async Task AuthenticateUserAsync()
        {
            if (refreshCode == "")
            {


                var (verifier, challenge) = PKCEUtil.GenerateCodes();

                var loginRequest = new LoginRequest(_redirectUri, _clientId, LoginRequest.ResponseType.Code)
                {
                    CodeChallenge = challenge,
                    CodeChallengeMethod = "S256",
                    Scope = new[] { Scopes.UserModifyPlaybackState, Scopes.UserReadPlaybackState }
                };

                var uri = loginRequest.ToUri();
                BrowserUtil.Open(uri);

                using var httpListener = new HttpListener();
                httpListener.Prefixes.Add(_redirectUri.GetLeftPart(UriPartial.Authority) + "/");
                httpListener.Start();

                var context = await httpListener.GetContextAsync();
                HttpListenerResponse response = context.Response;


                string code = context.Request.QueryString.Get("code");

                if (string.IsNullOrEmpty(code))
                {
                    Console.WriteLine("Authorization failed.");
                    return;
                }

                var tokenResponse = await new OAuthClient().RequestToken(new PKCETokenRequest(_clientId, code, _redirectUri, verifier));
                Console.WriteLine(tokenResponse.RefreshToken);
                refreshCode = tokenResponse.RefreshToken;
                Console.WriteLine(tokenResponse.ExpiresIn);
                var authenticator = new PKCEAuthenticator(_clientId, tokenResponse);

                var config = SpotifyClientConfig.CreateDefault()
                    .WithAuthenticator(authenticator);

                spotify = new SpotifyClient(config);
            }
            else
            {
                await RefreshAccessTokenAsync();
            }
        }

        public async Task RefreshAccessTokenAsync()
        {
            var response = await new OAuthClient().RequestToken(
                new AuthorizationCodeRefreshRequest(_clientId, _clientSecret, refreshCode));
            refreshCode = response.RefreshToken;
            StorageController.SaveToken(refreshCode);
            var config = SpotifyClientConfig.CreateDefault().WithToken(response.AccessToken);

            spotify = new SpotifyClient(config);
        }

        public async Task<bool> IsSpotifyPlaying()
        {
            try
            {
                var playbackState = await spotify.Player.GetCurrentlyPlaying(new PlayerCurrentlyPlayingRequest());
                return playbackState != null && playbackState.IsPlaying;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error occurred: {ex.Message}");
                return false;
            }
        }

    }
}
