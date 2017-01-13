using System;
using System.Linq;
using System.Text.RegularExpressions;
using Android.App;
using Android.Widget;

namespace IdeeKdo.Assets.Tools
{
    /// <summary>
    ///     Classe static qui s'occupe de gere les champs de saisie.
    /// </summary>
    public static class XInputs
    {
        private const string RegexEmail =
            @"\A(?:[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?)\Z";

        /// <summary>
        ///     Verifie si la chaine passée en parametre est un email valide
        /// </summary>
        /// <param name="strEmail">Email à tester</param>
        /// <returns>Retourne true si l'email est valide.</returns>
        public static bool ValidEmail(string strEmail) => Regex.IsMatch(strEmail, RegexEmail, RegexOptions.IgnoreCase);

        /// <summary>
        ///     Recupere un nombre inconnu de chaines de caracteres et renvoie 'true' si aucune d'entre elle n'est vide
        /// </summary>
        /// <param name="argStrings">nombre inconnu de chaine de caracteres</param>
        /// <returns>true si aucune chaine vide, false sinon.</returns>
        public static bool CheckStringInputs(params string[] argStrings)
            => argStrings.All(s => !string.IsNullOrWhiteSpace(s));

        /// <summary>
        ///     Recupere un nombre inconnu d'EditText et renvoie 'true' si aucune d'entre elle n'est vide
        /// </summary>
        /// <param name="argEditTexts">nombre inconnu d'EditText</param>
        /// <returns>true si aucun EditText vide, false sinon.</returns>
        public static bool CheckStringInputs(params EditText[] argEditTexts)
            => CheckStringInputs(Array.ConvertAll(argEditTexts, x => x.Text.Trim()));

        /// <summary>
        ///     Recupere un nombre inconnu de chaines de caracteres et renvoie 'true' si elles sont toutes identiques
        /// </summary>
        /// <param name="argStrings">nombre inconnu de chaine de caracteres</param>
        /// <returns>true si toutes les chaines sont identiques, false sinon.</returns>
        public static bool CheckEqualsInput(params string[] argStrings)
        {
            var firstItem = argStrings[0].Trim();
            return argStrings.Skip(1).All(s => string.Equals(firstItem, s.Trim()));
        }

        /// <summary>
        ///     Recupere un nombre inconnu d'EditText et renvoie 'true' si ils sont tous identiques
        /// </summary>
        /// <param name="argEditTexts">nombre inconnu de chaine de caracteres</param>
        /// <returns>true si tous les EditText sont identiques, false sinon.</returns>
        public static bool CheckEqualsInput(params EditText[] argEditTexts)
            => CheckEqualsInput(Array.ConvertAll(argEditTexts, x => x.Text.Trim()));

        public static bool IsNumeric(string strNumber)
        {
            double n;
            return IsNumeric(strNumber, out n);
        }

        public static bool IsNumeric(string strNumber, out double iNumber)
            => double.TryParse(strNumber.Replace(".", ","), out iNumber);

        public static bool IsNumeric(string strNumber, out int iNumber) => int.TryParse(strNumber, out iNumber);

        /// <summary>
        ///     Met le focus sur l'EditText + affiche un message d'erreur dessous ce dernier.
        /// </summary>
        /// <param name="argEditText">EditText qui sera focus et recevera un message dd'erreur si besoin</param>
        /// <param name="idMessage">Message d'erreur à afficher si besoin</param>
        /// <param name="atvi">Activitée en cours</param>
        public static void SetEditTextError(EditText argEditText, int idMessage, Activity atvi = null)
        {
            argEditText.SetError(XTools.GetRsrcStr(idMessage, atvi), null);
            argEditText.RequestFocus();
        }
    }
}