using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Android.App;
using Android.Content;
using Android.Util;
using IdeeKdo.Activities;
using IdeeKdo.Assets.Tools;

namespace IdeeKdo.Assets
{
    public static class Authentication
    {
        public static void OAuth(string sProvider, Activity atvi)
        {
            switch (sProvider)
            {
                case "Facebook":
                    XAuth.FacebookAuth(atvi);
                    break;
                case "Twitter":
                    break;
                default:
                    XAuth.GoogleAuth(atvi);
                    break;
            }
        }

        public static void SimpleAuth(Dictionary<string, object> dUser, Activity atvi)
        {
            var strSql = "SP_IdentificationEmail";
            var idErreur = Resource.String.ErrorInfos;
            if (dUser.Count != 2)
            {
                strSql = "SP_Register";
                idErreur = Resource.String.ErrorCreateAccount;
            }
            DoSql(dUser, atvi, strSql, idErreur);
        }

        private static void DoSql(Dictionary<string, object> dUser, Activity atvi, string strQuery, int idError)
        {
            try
            {
                var oDs = new MsSql(strQuery, dUser);
                var oDr = oDs.Result;
                CreateSession(oDr[0], atvi, ConnectType.Sql);
            }
            catch
            {
                XMessage.ShowError(idError, atvi);
            }
        }

        private static void CreateSession(DataRow drCompte, Activity atvi, ConnectType authType)
        {
            try
            {
                var dict = drCompte.Table.Columns.Cast<DataColumn>()
                    .ToDictionary(col => col.ColumnName, col => drCompte[col].ToString());
                dict.Add("AuthType", authType.ToString());
                XTools.User = new UserProfile(dict);
                UserProfile.Auth = true;
                atvi.StartActivity(new Intent(Application.Context, typeof(Main)));
                atvi.Finish();
            }
            catch (Exception e)
            {
                XLog.Write(LogPriority.Error, e.Message);
                XMessage.ShowError(e.Message, atvi);
            }
        }
    }
}