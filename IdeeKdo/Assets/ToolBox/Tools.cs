using System;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.OS;
using IdeeKdo.Activities;
using IdeeKdo.Fragments;
using FragmentTransaction = Android.Support.V4.App.FragmentTransaction;

namespace IdeeKdo.Assets.ToolBox
{
    /// <summary>
    ///     Classe static regroupant un certain nombre de methode utiles mais inclassable
    /// </summary>
    public static class XTools
    {
        /// <summary>
        ///     D�l�gu� utilis� pour mettre � jour les donn�es d'un utilisateur
        /// </summary>
        /// <param name="newValue">Nouvelle valeur</param>
        /// <returns>Retour un objet de type Task</returns>
        public delegate Task<bool> ChangeUserData(string newValue);

        /// <summary>
        ///     Activit�e principale de l'application, celle qui sera appel�e lorsqu'aucune activit�e ne sera explicitement
        ///     utilis�e
        /// </summary>
        public static Main MainAct;

        /// <summary>
        ///     Objet UserProfile qui sera utilis� dans tout le cycle de vie de l'application
        /// </summary>
        public static UserProfile User;

        /// <summary>
        ///     Methode utilis�e dans les pages contenant des vues de type RecyclerView
        ///     Recupere l'id de l'utilisateur pour pouvoirs afficher une nouvelle page
        /// </summary>
        /// <param name="sender">objet</param>
        /// <param name="photo">photo sur laquelle l'utilisateur a cliqu�</param>
        public static void OnItemListClick(object sender, Photo photo)
        {
            XMessage.ShowQuestion("Ouvrir ce profile ?", $"Voulez-vous ouvrir le profil de {photo.photoTitre} ?",
                ay => NewBundle(photo), null);
        }

        private static void NewBundle(Photo photo)
        {
            var bundle = new Bundle();
            bundle.PutInt("UserId", photo.Id);
            ChangeFragment(new FragmentUser {Arguments = bundle});
        }

        /// <summary>
        ///     Affiche un message d'attente pendant tout le temps ou l'action pass�e en parametre s'effectue
        /// </summary>
        /// <param name="action">Action � effectuer</param>
        /// <param name="activity">Activit�e en cours</param>
        public static async void AwaitAction(Func<Task> action, Activity activity = null)
            => await PrivateAwaitAction(action, activity);

        /// <summary>
        ///     Affiche un message d'attente pendant tout le temps ou l'action pass�e en parametre s'effectue
        /// </summary>
        /// <param name="action">Action � effectuer</param>
        /// <param name="activity">Activit�e en cours</param>
        public static async void AwaitAction(Action<object> action, Activity activity = null)
            => await PrivateAwaitAction(action, activity);

        /// <summary>
        ///     Affiche un message d'attente pendant tout le temps ou l'action pass�e en parametre s'effectue
        /// </summary>
        /// <param name="action">Action � effectuer</param>
        /// <param name="activity">Activit�e en cours</param>
        /// <returns>Retourne un objet Task</returns>
        private static async Task PrivateAwaitAction(object action, Activity activity)
        {
            var progressDialog = Xui.ShowWaitProgressDialog(activity);
            //Determiner le type d'execution en fonction du type de l'objet action
            if (action.GetType() == typeof(Action<object>))
            {
                await Task.Run(delegate { ((Action<object>) action).Invoke(null); });
            }
            else if (action.GetType() == typeof(Func<Task>))
            {
                await Task.Run(async delegate { await ((Func<Task>) action)(); });
            }
            progressDialog.Dismiss();
        }

        /// <summary>
        ///     Determine si l'activit�e pass�e en parametre est null ou non, si c'est le cas, l'activit�e Main est retourn�e
        /// </summary>
        /// <param name="activity">nullable</param>
        /// <returns>Retourne l'Activit�e en cours</returns>
        public static Activity GetActivity(Activity activity = null) => activity ?? MainAct;


        /// <summary>
        ///     Permet de recuperer les variables stock�ees et partag�es
        ///     Peut �tre utiliser pour stocker un profil utilisateur, des options, ...
        /// </summary>
        /// <returns>Retourne les variables stock�es.</returns>
        public static ISharedPreferences GetSharedPreferences()
            => Application.Context.GetSharedPreferences("Xadia", FileCreationMode.Private);

        /// <summary>
        ///     Obtiens le string qui correspond � la l'id envoy� en parametre
        /// </summary>
        /// <param name="idMessage">Resource.String qui correspond au message</param>
        /// <param name="atvi">Activit�e en cours</param>
        /// <returns>Retourne le string qui correspond � la valeur de "idMessage".</returns>
        public static string GetRsrcStr(int idMessage, Activity atvi = null)
            => GetActivity(atvi).Resources.GetString(idMessage);

        /// <summary>
        ///     Permet de changer le fragment qui est affich� dans l'activit� Main
        /// </summary>
        /// <param name="frg">Fragment qui va �tre affich�</param>
        /// <param name="bAddToBackStack">Ajouter le fragment au back stack ou non (mettre false pour le premier fragment)</param>
        public static void ChangeFragment(FragmentMain frg, bool bAddToBackStack = true)
        {
            //Si le fragment n'est pas initialis�, ne rien faire.
            if (frg == null)
            {
                return;
            }
            //L'Activit�e Main peut changer en cas de rotation de l'�cran
            //Changer le Fragment qui apparait en fond et l'afficher
            Xui.HideKeyBoard();
            FragmentTransaction trans;
            using (trans = MainAct.SupportFragmentManager.BeginTransaction())
            {
                //Si le fragment par defaut est ajout� au backStack et qu'un utilisateur utilise 
                //le bouton "retour" en bas de son ecran (bouton natif Android), alors
                //un ecran blanc sans vue sera pr�sent� � l'utilisateur
                if (bAddToBackStack)
                {
                    trans.AddToBackStack(null);
                }
                trans.Replace(Resource.Id.HomeFrameLayout, frg).Commit();
            }
        }

        /// <summary>
        ///     Execute une mise � jour SQL et affiche un message en cas de succ�s
        /// </summary>
        /// <param name="update">M�thode qui sera appel�e en cas de succ�s</param>
        /// <param name="strSql">Requete SQL</param>
        /// <param name="idSuccessText">Identifiant de la ressource qui sera affich�e en cas de succ�s</param>
        public static async void AwaitSql(ChangeUserData update, string strSql, int idSuccessText)
        {
            if (!await update(strSql))
            {
                return;
            }
            ChangeFragment(Main.GetHome());
            Xui.SetSnackMessage(idSuccessText);
        }
    }
}