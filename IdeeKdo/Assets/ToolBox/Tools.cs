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
        ///     Délégué utilisé pour mettre à jour les données d'un utilisateur
        /// </summary>
        /// <param name="newValue">Nouvelle valeur</param>
        /// <returns>Retour un objet de type Task</returns>
        public delegate Task<bool> ChangeUserData(string newValue);

        /// <summary>
        ///     Activitée principale de l'application, celle qui sera appelée lorsqu'aucune activitée ne sera explicitement
        ///     utilisée
        /// </summary>
        public static Main MainAct;

        /// <summary>
        ///     Objet UserProfile qui sera utilisé dans tout le cycle de vie de l'application
        /// </summary>
        public static UserProfile User;

        /// <summary>
        ///     Methode utilisée dans les pages contenant des vues de type RecyclerView
        ///     Recupere l'id de l'utilisateur pour pouvoirs afficher une nouvelle page
        /// </summary>
        /// <param name="sender">objet</param>
        /// <param name="photo">photo sur laquelle l'utilisateur a cliqué</param>
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
        ///     Affiche un message d'attente pendant tout le temps ou l'action passée en parametre s'effectue
        /// </summary>
        /// <param name="action">Action à effectuer</param>
        /// <param name="activity">Activitée en cours</param>
        public static async void AwaitAction(Func<Task> action, Activity activity = null)
            => await PrivateAwaitAction(action, activity);

        /// <summary>
        ///     Affiche un message d'attente pendant tout le temps ou l'action passée en parametre s'effectue
        /// </summary>
        /// <param name="action">Action à effectuer</param>
        /// <param name="activity">Activitée en cours</param>
        public static async void AwaitAction(Action<object> action, Activity activity = null)
            => await PrivateAwaitAction(action, activity);

        /// <summary>
        ///     Affiche un message d'attente pendant tout le temps ou l'action passée en parametre s'effectue
        /// </summary>
        /// <param name="action">Action à effectuer</param>
        /// <param name="activity">Activitée en cours</param>
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
        ///     Determine si l'activitée passée en parametre est null ou non, si c'est le cas, l'activitée Main est retournée
        /// </summary>
        /// <param name="activity">nullable</param>
        /// <returns>Retourne l'Activitée en cours</returns>
        public static Activity GetActivity(Activity activity = null) => activity ?? MainAct;


        /// <summary>
        ///     Permet de recuperer les variables stockéees et partagées
        ///     Peut être utiliser pour stocker un profil utilisateur, des options, ...
        /// </summary>
        /// <returns>Retourne les variables stockées.</returns>
        public static ISharedPreferences GetSharedPreferences()
            => Application.Context.GetSharedPreferences("Xadia", FileCreationMode.Private);

        /// <summary>
        ///     Obtiens le string qui correspond à la l'id envoyé en parametre
        /// </summary>
        /// <param name="idMessage">Resource.String qui correspond au message</param>
        /// <param name="atvi">Activitée en cours</param>
        /// <returns>Retourne le string qui correspond à la valeur de "idMessage".</returns>
        public static string GetRsrcStr(int idMessage, Activity atvi = null)
            => GetActivity(atvi).Resources.GetString(idMessage);

        /// <summary>
        ///     Permet de changer le fragment qui est affiché dans l'activité Main
        /// </summary>
        /// <param name="frg">Fragment qui va être affiché</param>
        /// <param name="bAddToBackStack">Ajouter le fragment au back stack ou non (mettre false pour le premier fragment)</param>
        public static void ChangeFragment(FragmentMain frg, bool bAddToBackStack = true)
        {
            //Si le fragment n'est pas initialisé, ne rien faire.
            if (frg == null)
            {
                return;
            }
            //L'Activitée Main peut changer en cas de rotation de l'écran
            //Changer le Fragment qui apparait en fond et l'afficher
            Xui.HideKeyBoard();
            FragmentTransaction trans;
            using (trans = MainAct.SupportFragmentManager.BeginTransaction())
            {
                //Si le fragment par defaut est ajouté au backStack et qu'un utilisateur utilise 
                //le bouton "retour" en bas de son ecran (bouton natif Android), alors
                //un ecran blanc sans vue sera présenté à l'utilisateur
                if (bAddToBackStack)
                {
                    trans.AddToBackStack(null);
                }
                trans.Replace(Resource.Id.HomeFrameLayout, frg).Commit();
            }
        }

        /// <summary>
        ///     Execute une mise à jour SQL et affiche un message en cas de succès
        /// </summary>
        /// <param name="update">Méthode qui sera appelée en cas de succès</param>
        /// <param name="strSql">Requete SQL</param>
        /// <param name="idSuccessText">Identifiant de la ressource qui sera affichée en cas de succès</param>
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