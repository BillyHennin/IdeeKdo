using Android.OS;
using Android.Views;
using Android.Widget;
using IdeeKdo.Assets;
using IdeeKdo.Assets.ToolBox;

namespace IdeeKdo.Fragments
{
    public class FragmentHome : FragmentMain
    {
        private FragmentChangeMail _fragmentChangeMail;
        private FragmentChangePwd _fragmentChangePwd;

        public FragmentHome()
        {
            FragmentPassword = new FragmentChangePwd();
            FragmentMail = new FragmentChangeMail();
        }

        private FragmentChangeMail FragmentMail
        {
            set
            {
                if (value != null)
                {
                    _fragmentChangeMail = value;
                }
            }
            get { return _fragmentChangeMail ?? new FragmentChangeMail(); }
        }

        private FragmentChangePwd FragmentPassword
        {
            set
            {
                if (value != null)
                {
                    _fragmentChangePwd = value;
                }
            }
            get { return _fragmentChangePwd ?? new FragmentChangePwd(); }
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            var user = XTools.User;
            var view = InitView(Resource.String.HomeTitle, this, Resource.Layout.FragmentLayoutHome, inflater, container);
            view.FindViewById<TextView>(Resource.Id.UtilisateurText).Text = user.NomComplet;
            view.FindViewById<TextView>(Resource.Id.EmailText).Text = user.Mail;
            view.FindViewById<Button>(Resource.Id.ChangeMailButton).Click +=
                (s, a) => { XTools.ChangeFragment(FragmentMail); };
            view.FindViewById<Button>(Resource.Id.ChangePWDButton).Click +=
                (s, a) => { XTools.ChangeFragment(FragmentPassword); };
            return view;
        }
    }
}