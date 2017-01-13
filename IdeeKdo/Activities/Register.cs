using System;
using System.Collections.Generic;
using Android.App;
using Android.OS;
using Android.Support.V7.App;
using Android.Widget;
using IdeeKdo.Assets;
using IdeeKdo.Assets.Tools;

namespace IdeeKdo.Activities
{
    [Activity(Label = "@string/Register", Theme = "@style/MyTheme")]
    public class Register : AppCompatActivity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.ActivityLayoutRegister);
            FindViewById<Button>(Resource.Id.ValidateRegisterButton).Click += RegisterCompte;
        }

        private void RegisterCompte(object sender, EventArgs args)
        {
            if (!XNetwork.CheckNetwork(this))
                return;
            var isValid = true;
            var strNom = FindViewById<EditText>(Resource.Id.Nom).Text.Trim();
            var strPrenom = FindViewById<EditText>(Resource.Id.Prenom).Text.Trim();
            var editTextEmail = FindViewById<EditText>(Resource.Id.Email);
            var strEmail = editTextEmail.Text.Trim();
            var editTextpassword = FindViewById<EditText>(Resource.Id.Password);
            var strPassword = editTextpassword.Text.Trim();
            var strPasswordConfirm = FindViewById<EditText>(Resource.Id.PasswordConfirm).Text.Trim();
            if (XInputs.CheckStringInputs(strNom, strPrenom, strEmail, strPassword, strPasswordConfirm))
            {
                if (!XInputs.CheckStringInputs(strPassword, strPasswordConfirm))
                {
                    XInputs.SetEditTextError(editTextpassword, Resource.String.ErrorPassword, this);
                    isValid = false;
                }
                if (!XInputs.ValidEmail(strEmail))
                {
                    XInputs.SetEditTextError(editTextEmail, Resource.String.ErrorMail, this);
                    isValid = false;
                }
                if (!isValid)
                    return;
                var dUser = new Dictionary<string, object>
                {
                    {"Mail", strEmail},
                    {"Password", strPassword},
                    {"Nom", strNom},
                    {"Prenom", strPrenom}
                };
                Authentication.SimpleAuth(dUser, this);
            }
            else
            {
                XMessage.ShowError(Resource.String.ErrorMail, this);
            }
        }
    }
}