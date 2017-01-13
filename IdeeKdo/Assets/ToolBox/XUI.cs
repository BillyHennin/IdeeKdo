using System;
using Android.App;
using Android.Support.Design.Widget;
using Android.Views;
using Android.Views.InputMethods;

namespace IdeeKdo.Assets.ToolBox
{
    /// <summary>
    ///     Classe static qui regroupe l'ensemble des fonctions relative à l'interface general (Sauf message d'alerte)
    /// </summary>
    public static class Xui
    {
        /// <summary>
        ///     Affiche un message dans un bandeau se trouvant en bas de l'ecran
        /// </summary>
        /// <param name="idMessage">identifiant de la ressource texte</param>
        /// <param name="view">Vue actuelle</param>
        public static void SetSnackMessage(int idMessage, View view = null)
            => SetSnackMessage(XTools.GetRsrcStr(idMessage), view);

        /// <summary>
        ///     Affiche un message dans un bandeau se trouvant en bas de l'ecran
        /// </summary>
        /// <param name="strMessage">Mesage texte</param>
        /// <param name="view">Vue actuelle</param>
        public static void SetSnackMessage(string strMessage, View view = null)
            =>
                Snackbar.Make(view ?? XTools.MainAct.DrawerLayout, strMessage, Snackbar.LengthLong)
                    .SetAction("Ok", o => { })
                    .Show();

        /// <summary>
        ///     Permet de cacher le clavier virtuel lors d'un changement de fragment.
        ///     Si le clavier est deja caché, la fonction de fera rien.
        /// </summary>
        public static void HideKeyBoard()
        {
            try
            {
                var inputManager = (InputMethodManager) XTools.MainAct.GetSystemService("input_method");
                inputManager.HideSoftInputFromWindow(XTools.MainAct.CurrentFocus.WindowToken,
                    HideSoftInputFlags.NotAlways);
            }
            catch
            {
                //Le clavier n'etait pas visible et ne peut donc pas être caché
            }
        }

        /// <summary>
        ///     Affiche un message d'attente dans une pop up
        /// </summary>
        /// <param name="activity">Activitée en cours</param>
        /// <returns>Objet ProgressDialog initialisé et affiché</returns>
        public static ProgressDialog ShowWaitProgressDialog(Activity activity = null)
        {
            var atvi = XTools.GetActivity(activity);
            var mDialog = new ProgressDialog(atvi);
            mDialog.SetTitle(XTools.GetRsrcStr(Resource.String.Patienter, atvi));
            mDialog.SetMessage(XTools.GetRsrcStr(Resource.String.Verification, atvi));
            mDialog.SetCancelable(false);
            mDialog.Show();
            return mDialog;
        }

        /// <summary>
        ///     Fonction à utiliser lors d'utilisation de multithread.
        ///     Permet de modifier l'interface en forçant l'utilisation du Thread d'affichage
        /// </summary>
        /// <param name="action">Actions ou methode à effectuer</param>
        /// <param name="activity">Activitée voulu</param>
        public static void ShowOnMainUi(Action<object> action, Activity activity = null)
            => XTools.GetActivity(activity).RunOnUiThread(() => { action.Invoke(null); });

        /// <summary>
        ///     Change l'etat de visibilité de la vue.
        /// </summary>
        /// <param name="view"></param>
        public static void ToggleVisibility(View view) => SetVisibility(view, view.Visibility == ViewStates.Invisible);

        public static void SetVisibility(View view, bool bToggle)
            => view.Visibility = bToggle ? ViewStates.Visible : ViewStates.Invisible;
    }
}