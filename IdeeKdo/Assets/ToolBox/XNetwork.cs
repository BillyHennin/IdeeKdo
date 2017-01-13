using System.Json;
using System.Net.Http;
using System.Net.NetworkInformation;
using System.Threading.Tasks;
using Android.App;
using Android.Graphics;
using Android.Net;
using ModernHttpClient;
using Uri = System.Uri;

namespace IdeeKdo.Assets.ToolBox
{
    /// <summary>
    ///     Classe static regroupant toutes les methodes liées au reseau
    /// </summary>
    public static class XNetwork
    {
        /// <summary>
        ///     Permet de verifier que l'hote est joignable
        /// </summary>
        /// <param name="hostNameOrAddress">Nom de l'hote ou adress IP que l'on souhaite tester</param>
        /// <returns>Retourne true si l'hote est joinable</returns>
        public static bool PingRequest(string hostNameOrAddress)
        {
            bool bPing;
            try
            {
                bPing = new Ping().Send(hostNameOrAddress).Status == IPStatus.Success;
            }
            catch
            {
                bPing = false;
            }
            return bPing;
        }

        /// <summary>
        ///     Permet de verifier que l'hote est joignable et affiche un message le cas contraire
        /// </summary>
        /// <param name="hostNameOrAddress">Nom de l'hote ou adress IP que l'on souhaite tester</param>
        /// <param name="idMessage">Identifiant de la ressource qui sera affichée en cas d'erreur</param>
        /// <param name="activity">Activitée en cours</param>
        /// <returns>Retourne true si l'hote est joinable</returns>
        public static bool PingRequest(string hostNameOrAddress, int idMessage, Activity activity = null)
        {
            var bPing = PingRequest(hostNameOrAddress);
            if (!bPing)
            {
                XMessage.ShowError(idMessage, activity);
            }
            return bPing;
        }

        /// <summary>
        ///     Verifie si l'appareil est connecté à Internet.
        ///     Necessite la permission de l'appareil.
        ///     Affiche un message en cas d'erreur.
        /// </summary>
        /// <param name="activity">Activitée en cours</param>
        /// <returns>Retourne true si l'appareil est connecté à Internet.</returns>
        public static bool CheckNetwork(Activity activity = null)
        {
            var atvi = XTools.GetActivity(activity);
            var activeConnection = ((ConnectivityManager) atvi.GetSystemService("connectivity")).ActiveNetworkInfo;
            if (activeConnection != null && activeConnection.IsConnected)
            {
                return true;
            }
            XMessage.ShowError(Resource.String.ErrorNetwork, atvi);
            return false;
        }

        /// <summary>
        ///     Verifie si l'appareil peut pinger le serveur de base de données.
        /// </summary>
        /// <returns>Retourne true si l'appareil peut se connecter à la base de données</returns>
        public static bool CheckDatabase(Activity activity = null)
            => PingRequest(XTools.GetRsrcStr(Resource.String.DBA, activity), Resource.String.ErrorServer, activity);

        /// <summary>
        ///     Recupere une reponse Web et la transforme en objet Json
        /// </summary>
        /// <param name="url">String</param>
        /// <returns>Objet Json</returns>
        public static async Task<JsonValue> GetJsonFromWeb(string url)
        {
            var httpClient = new HttpClient(new NativeMessageHandler()) {BaseAddress = new Uri(url)};
            using (var response = httpClient.GetStreamAsync(new Uri(url)).Result)
            {
                return await Task.Run(() => JsonValue.Load(response));
            }
        }

        /// <summary>
        ///     Permet de recuperer un objet de type Bitmap via l'url d'une image
        /// </summary>
        /// <param name="url">url de l'image</param>
        /// <returns>Image du type Bitmap</returns>
        public static Bitmap GetImageBitmapFromUrl(string url)
        {
            Bitmap imageBitmap = null;
            using (var httpClient = new HttpClient(new NativeMessageHandler()))
            {
                //Todo : Check this;
                var imageBytes = httpClient.GetByteArrayAsync(url).Result;
                if (imageBytes != null && imageBytes.Length > 0)
                {
                    imageBitmap = BitmapFactory.DecodeByteArray(imageBytes, 0, imageBytes.Length);
                }
            }
            return imageBitmap;
        }
    }
}