using System;
using System.Json;
using System.Threading.Tasks;
using Android.App;
using Xamarin.Auth;

namespace IdeeKdo.Assets.Tools
{
    public static class XAuth
    {
        private static readonly TaskScheduler UiScheduler = TaskScheduler.FromCurrentSynchronizationContext();

        private static void DoOAuth(string clientId, string scope, string authUrl, string redirectUrl, Activity activity)
        {
            var auth = new OAuth2Authenticator(clientId, scope, new Uri(authUrl), new Uri(redirectUrl));
            // If authorization succeeds or is canceled, .Completed will be fired.
            auth.Completed += (s, ee) =>
            {
                if (!ee.IsAuthenticated)
                {
                    XMessage.ShowNotification("Erreur", "L'authentification a echouée", activity);
                    return;
                }

                // Now that we're logged in, make a OAuth2 request to get the user's info.
                var request = new OAuth2Request("GET", new Uri("https://graph.facebook.com/me"), null, ee.Account);
                request.GetResponseAsync().ContinueWith(t =>
                {
                    var builder = new AlertDialog.Builder(activity);
                    if (t.IsFaulted)
                    {
                        builder.SetTitle("Error");
                        builder.SetMessage(t.Exception.Flatten().InnerException?.ToString());
                    }
                    else if (t.IsCanceled)
                    {
                        builder.SetTitle("Task Canceled");
                    }
                    else
                    {
                        var obj = JsonValue.Parse(t.Result.GetResponseText());

                        builder.SetTitle("Logged in");
                        builder.SetMessage("Name: " + obj["name"]);
                    }

                    builder.SetPositiveButton("Ok", (o, e) => { });
                    builder.Create().Show();
                }, UiScheduler);
            };
            activity.StartActivity(auth.GetUI(activity));
        }

        public static void GoogleAuth(Activity activity)
        {
            DoOAuth("962936376748-v87jg2b9eg39j5bcmqj79t7ilhlb8829.apps.googleusercontent.com",
                "https://www.googleapis.com/auth/userinfo.email",
                "https://accounts.google.com/o/oauth2/auth", "https://www.googleapis.com/plus/v1/people/me", activity);
            //http://www.appliedcodelog.com/2015/08/login-by-google-account-integration-for.html
        }

        public static void FacebookAuth(Activity activity)
        {
            DoOAuth("337371049934482", string.Empty, "https://m.facebook.com/dialog/oauth/",
                "http://www.facebook.com/connect/login_success.html", activity);
        }
    }
}