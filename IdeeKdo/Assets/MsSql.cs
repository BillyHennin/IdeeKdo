using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace IdeeKdo.Assets
{
    public class MsSql
    {
        private const string Source = "BILLY-HENNIN-PC";
        private const string Port = "1433";
        private const string Catalog = "XadiaOAuthTest";
        private const string UserId = "sa";
        private const string Password = "xadia85.db";

        private static readonly string DataBaseConnectionString =
            $"Data Source={Source},{Port};Initial Catalog={Catalog};Persist Security Info=True;User ID={UserId};Password={Password}";

        public readonly DataRow[] Result;

        public MsSql(string strQuery, Dictionary<string, object> param)
        {
            Result = GetDataRowsFromQuery(strQuery, param);
        }

        /// <summary>
        ///     Permet de modifier l'adresse email d'un utilisateur
        /// </summary>
        /// <param name="oEmail">Email de l'utilisateur qui va etre utiliser pour modifier son mot de passe</param>
        /// <param name="nMail">Nouvel email de l'utilisateur, penser à verifier s'il est valide</param>
        /// <returns>Retourne true si la mise à jour s'est effectuée correctement</returns>
        public static bool ChangeMail(string oEmail, string nMail)
            => UpdateUser("SP_Mail", new Dictionary<string, object> {{"OldEmail", oEmail}, {"NewEmail", nMail}});

        /// <summary>
        ///     Permet de modifier le mot de passe d'un utilisateur en passant son email actuel en parametre
        /// </summary>
        /// <param name="eMail">Email de l'utilisateur qui va etre utiliser pour modifier son mot de passe</param>
        /// <param name="pwd">Nouveau mot de passe de l'utilisateur</param>
        /// <returns>Retourne true si la mise à jour s'est effectuée correctement</returns>
        public static bool ChangePwd(string eMail, string pwd)
            => UpdateUser("SP_NewPassword", new Dictionary<string, object> {{"Email", eMail}, {"PWD", pwd}});

        private static bool UpdateUser(string strQuery, Dictionary<string, object> param)
        {
            var bRtr = true;
            try
            {
                DoSqlDataSet(strQuery, param);
            }
            catch
            {
                bRtr = false;
            }
            return bRtr;
        }

        private static DataRow[] GetDataRowsFromQuery(string strQuery, Dictionary<string, object> param)
        {
            try
            {
                var oDs = DoSqlDataSet(strQuery, param);
                return oDs.Tables.Count > 0 ? oDs.Tables[0].Select() : oDs.Tables.Add().Select();
            }
            catch
            {
                return null;
            }
        }

        private static DataSet DoSqlDataSet(string strQuery, Dictionary<string, object> param)
        {
            if (param == null)
                return null;
            var oDs = new DataSet();
            oDs.Clear();
            var cnx = new SqlConnection(DataBaseConnectionString);
            cnx.Open();
            var cmd = new SqlCommand
            {
                CommandText = strQuery,
                CommandType = CommandType.StoredProcedure,
                Connection = cnx,
                CommandTimeout = 300
            };
            foreach (var kvp in param)
                cmd.Parameters.AddWithValue(kvp.Key, kvp.Value ?? DBNull.Value);
            var da = new SqlDataAdapter {SelectCommand = cmd};
            da.Fill(oDs);
            da.Dispose();
            cmd.Dispose();
            cnx.Close();
            return oDs;
        }
    }
}