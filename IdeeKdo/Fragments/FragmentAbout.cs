using Android.OS;
using Android.Views;
using Android.Widget;
using IdeeKdo.Assets;
using IdeeKdo.Assets.ToolBox;

namespace IdeeKdo.Fragments
{
    public class FragmentAbout : FragmentMain
    {
        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            var view = InitView(Resource.String.AboutTitle, this, Resource.Layout.FragmentLayoutAbout, inflater,
                container);
            if (XLog.XadiaLogs != null && !XLog.XadiaLogs.Count.Equals(0))
            {
                view.FindViewById<TextView>(Resource.Id.AboutText).Text = XLog.GetLogs();
            }
            return view;
        }
    }
}