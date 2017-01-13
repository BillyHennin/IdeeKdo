using System;
using System.Collections.Generic;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Support.V7.App;
using Android.Widget;
using IdeeKdo.Assets;
using IdeeKdo.Assets.Tools;

namespace IdeeKdo.Activities
{
    [Activity(Label = "@string/LoginTitle", Theme = "@style/MyTheme")]
    public class Login : AppCompatActivity
    {
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            SetContentView(Resource.Layout.ActivityLayoutAuth);
            XNetwork.CheckNetwork(this);
            FindViewById<Button>(Resource.Id.AuthButton).Click += SimpleAuthButtonClick;
            FindViewById<Button>(Resource.Id.RegisterButton).Click +=
                (s, a) => { StartActivity(new Intent(Application.Context, typeof(Register))); };
            FindViewById<Button>(Resource.Id.FacebookAuthButton).Click +=
                (s, a) => { Authentication.OAuth("Facebook", this); };
            FindViewById<Button>(Resource.Id.GoogleAuthButton).Click +=
                (s, a) => { Authentication.OAuth("Google", this); };
            FindViewById<Button>(Resource.Id.TwitterAuthButton).Click +=
                (s, a) => { Authentication.OAuth("Twitter", this); };
        }

        private void SimpleAuthButtonClick(object sender, EventArgs args)
        {
            if (!XNetwork.CheckNetwork(this))
                return;
            var email = FindViewById<EditText>(Resource.Id.EmailText);
            var strEmail = email.Text.Trim();
            var strPassword = FindViewById<EditText>(Resource.Id.PasswordText).Text.Trim();
            if (XInputs.CheckStringInputs(strEmail, strPassword))
                if (XInputs.ValidEmail(strEmail))
                {
                    var dUser = new Dictionary<string, object> {{"Mail", email.Text}, {"Pwd", strPassword}};
                    XTools.AwaitAction(obj => Authentication.SimpleAuth(dUser, this), this);
                }
                else
                {
                    XInputs.SetEditTextError(email, Resource.String.ErrorMail, this);
                }
            else
                XMessage.ShowError(Resource.String.ErrorEmpty, this);
        }
    }
}