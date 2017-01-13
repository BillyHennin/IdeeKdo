using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using IdeeKdo.Activities;
using IdeeKdo.Assets.ToolBox;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace IdeeKdo.Assets
{
    public enum ConnectType
    {
        Sql = 1,
        Google = 2,
        Twitter = 3,
        Facebook = 4
    }

    public class UserProfile
    {
        public UserProfile(string mail, string nom, string prenom)
        {
            Mail = mail;
            Nom = nom;
            Prenom = prenom;
            NomComplet = $"{Nom} {Prenom}";
        }

        public UserProfile(IReadOnlyDictionary<string, string> dictionary)
        {
            Mail = dictionary["Mail"];
            Nom = dictionary["Nom"];
            Prenom = dictionary["Prenom"];
            NomComplet = $"{dictionary["Nom"]} {dictionary["Prenom"]}";
            AuthType = (ConnectType) Enum.Parse(typeof(ConnectType), dictionary["AuthType"]);
        }

        public string Mail { get; private set; }
        public string Nom { get; }
        public string Prenom { get; }
        public string NomComplet { get; private set; }
        public ConnectType AuthType { get; }


        public static bool Auth
        {
            get
            {
                var strAuth = XTools.GetSharedPreferences().GetString("user", null);
                var isAuthenticated = false;
                if (string.IsNullOrEmpty(strAuth) || strAuth.Equals("null"))
                {
                    return false;
                }
                try
                {
                    var strJson = JObject.Parse(strAuth);
                    if (strJson != null)
                    {
                        XTools.User =
                            new UserProfile(JsonConvert.DeserializeObject<Dictionary<string, string>>(strJson.ToString()));
                        isAuthenticated = true;
                    }
                }
                catch
                {
                    isAuthenticated = false;
                }
                return isAuthenticated;
            }
            set
            {
                XTools.GetSharedPreferences()
                    .Edit()
                    .PutString("user", value ? JsonConvert.SerializeObject(XTools.User) : string.Empty)
                    .Commit();
            }
        }

        private string SetNewMail(string newMail)
        {
            Mail = newMail;
            return newMail;
        }

        public async Task<bool> ChangeMail(string newMail)
            => await UpdateUserData(MsSql.ChangeMail, Mail, SetNewMail(newMail));

        public async Task<bool> ChangePwd(string newPwd) => await UpdateUserData(MsSql.ChangePwd, Mail, newPwd);

        private async Task<bool> UpdateUserData(UpdateData updateData, string strEmailAsId, string strNewValue)
        {
            var success = await Task.FromResult(updateData(strEmailAsId, strNewValue));
            if (success)
            {
                Auth = true;
            }
            return success;
        }

        public void Deconnexion(Activity atvi)
        {
            //Detruire les infos de l'utilisateur
            XTools.User = null;
            Auth = false;
            //Rediriger
            atvi.StartActivity(new Intent(Application.Context, typeof(Login)));
            atvi.Finish();
        }

        private delegate bool UpdateData(string strEmailAsId, string strNewValue);
    }
}