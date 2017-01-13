using Android.OS;
using Android.Views;
using Android.Widget;
using IdeeKdo.Assets;
using IdeeKdo.Assets.ToolBox;

namespace IdeeKdo.Fragments
{
    public class FragmentChangePwd : FragmentMain
    {
        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            var view = InitView(Resource.String.ChangePWD, this, Resource.Layout.FragmentLayoutPassword, inflater,
                container);
            view.FindViewById<Button>(Resource.Id.ChangePasswordButtonFragment).Click += (s, a) => { ChangePwd(view); };
            return view;
        }

        private static void ChangePwd(View view)
        {
            Xui.HideKeyBoard();
            var newPassword = view.FindViewById<EditText>(Resource.Id.NewPasswordText);
            var newPasswordConfirm = view.FindViewById<EditText>(Resource.Id.PasswordConfirm);
            if (XInputs.CheckStringInputs(newPassword, newPasswordConfirm))
            {
                if (XInputs.CheckEqualsInput(newPassword, newPasswordConfirm))
                {
                    XTools.AwaitAction(
                        obj =>
                            XTools.AwaitSql(XTools.User.ChangePwd, newPassword.Text,
                                Resource.String.SuccessPassword));
                    newPassword.Text = string.Empty;
                    newPasswordConfirm.Text = string.Empty;
                }
                else
                {
                    XInputs.SetEditTextError(newPasswordConfirm, Resource.String.ErrorPassword);
                }
            }
            else
            {
                XMessage.ShowError(Resource.String.ErrorEmpty);
            }
        }
    }
}