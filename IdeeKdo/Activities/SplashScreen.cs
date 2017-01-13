using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.Support.V7.App;
using IdeeKdo.Assets;
using IdeeKdo.Assets.ToolBox;

namespace IdeeKdo.Activities
{
    [Activity(Theme = "@style/MyTheme.Splash", MainLauncher = true, NoHistory = true)]
    public class SplashScreen : AppCompatActivity
    {
        protected override void OnResume()
        {
            base.OnResume();
            var startupWork = new Task(async () =>
            {
                for (var i = 0; i < 100; i++)
                {
                    await Task.Delay(10);
                }
            });
            startupWork.ContinueWith(t =>
            {
                StartActivity(new Intent(Application.Context,
                    XNetwork.CheckNetwork(this) && UserProfile.Auth
                        ? typeof(Main)
                        : typeof(Login)));
                Finish();
            }, TaskScheduler.FromCurrentSynchronizationContext());
            startupWork.Start();
        }
    }
}