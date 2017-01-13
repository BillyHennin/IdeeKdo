using System.Json;
using Android.OS;
using Android.Util;
using Android.Views;
using Android.Widget;
using IdeeKdo.Assets;
using IdeeKdo.Assets.Tools;

namespace IdeeKdo.Fragments
{
    public class FragmentWeather : FragmentMain
    {
        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            var view = InitView(Resource.String.WeatherTitle, this, Resource.Layout.FragmentLayoutWeather, inflater,
                container);
            Xui.ToggleVisibility(view.FindViewById<LinearLayout>(Resource.Id.InvisibleWeatherLayout));
            view.FindViewById<Button>(Resource.Id.getWeatherButton).Click +=
                (s, a) => { XTools.AwaitAction(obj => ParseAndDisplay(view)); };
            return view;
        }

        private static async void ParseAndDisplay(View view)
        {
            Xui.HideKeyBoard();
            var hasError = false;
            var latitude = view.FindViewById<EditText>(Resource.Id.latText);
            var strLatitude = latitude.Text.Trim();
            var longitude = view.FindViewById<EditText>(Resource.Id.longText);
            var strLongitude = longitude.Text.Trim();
            if (!XInputs.IsNumeric(strLatitude))
            {
                XInputs.SetEditTextError(latitude, Resource.String.ErrorInput);
                hasError = true;
            }
            if (!XInputs.IsNumeric(strLongitude))
            {
                XInputs.SetEditTextError(longitude, Resource.String.ErrorInput);
                hasError = true;
            }
            if (hasError)
                return;
            const string host = "api.geonames.org";
            var url = $"http://{host}/findNearByWeatherJSON?lat={strLatitude}&lng={strLongitude}&username=xadia";
            if (!XNetwork.PingRequest(host))
                return;
            try
            {
                var json = await XNetwork.GetJsonFromWeb(url);
                var resultatsMeteo = json["weatherObservation"];
                Xui.ShowOnMainUi(obj =>
                {
                    Xui.ToggleVisibility(view.FindViewById<LinearLayout>(Resource.Id.InvisibleWeatherLayout));
                    view.FindViewById<TextView>(Resource.Id.locationText).Text = resultatsMeteo["stationName"];
                    view.FindViewById<TextView>(Resource.Id.tempText).Text = $"{resultatsMeteo["temperature"]:F1} ° C";
                    view.FindViewById<TextView>(Resource.Id.humidText).Text = $"{resultatsMeteo["humidity"]} %";
                    view.FindViewById<TextView>(Resource.Id.condText).Text =
                        $"{GetJsonValue(resultatsMeteo, "clouds")} {GetJsonValue(resultatsMeteo, "weatherCondition")}";
                });
            }
            catch
            {
                //Le serveur ne reponds pas / erreur inconnue
                XLog.Write(LogPriority.Info, XTools.GetRsrcStr(Resource.String.ErrorServer));
                XMessage.ShowError(Resource.String.ErrorServer);
            }
        }

        private static string GetJsonValue(JsonValue jsonResultat, string key)
            => jsonResultat[key].ToString().Equals("\"n/a\"") ? string.Empty : jsonResultat[key].ToString();
    }
}