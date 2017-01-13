using Android.OS;
using Android.Views;
using Android.Widget;
using IdeeKdo.Assets;
using IdeeKdo.Assets.ToolBox;

namespace IdeeKdo.Fragments
{
    public class FragmentChangeMail : FragmentMain
    {
        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            var view = InitView(Resource.String.ChangeMail, this, Resource.Layout.FragmentLayoutMail, inflater,
                container);
            view.FindViewById<Button>(Resource.Id.ChangeMailButtonFragment).Click += (s, a) => { ChangeMail(view); };
            return view;
        }

        private static void ChangeMail(View view)
        {
            Xui.HideKeyBoard();
            var newMail = view.FindViewById<EditText>(Resource.Id.NewEmailText);
            var newMailConfirm = view.FindViewById<EditText>(Resource.Id.NewEmailConfirmText);
            if (XInputs.CheckStringInputs(newMail, newMailConfirm))
            {
                if (XInputs.ValidEmail(newMail.Text))
                {
                    if (XInputs.CheckEqualsInput(newMail, newMailConfirm))
                    {
                        XTools.AwaitAction(
                            obj =>
                                XTools.AwaitSql(XTools.User.ChangeMail, newMail.Text,
                                    Resource.String.SuccessEmail));
                        newMail.Text = string.Empty;
                        newMailConfirm.Text = string.Empty;
                    }
                    else
                    {
                        XInputs.SetEditTextError(newMailConfirm, Resource.String.ErrorNewMail);
                    }
                }
                else
                {
                    XInputs.SetEditTextError(newMail, Resource.String.ErrorMail);
                }
            }
            else
            {
                XMessage.ShowError(Resource.String.ErrorEmpty);
            }
        }
    }
}