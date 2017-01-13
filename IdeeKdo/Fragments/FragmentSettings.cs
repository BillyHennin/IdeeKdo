using Android.OS;
using Android.Views;
using IdeeKdo.Assets;

namespace IdeeKdo.Fragments
{
    public class FragmentSettings : FragmentMain
    {
        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            return InitView(Resource.String.SettingsTitle, this, Resource.Layout.FragmentLayoutSettings, inflater,
                container);
        }
    }
}