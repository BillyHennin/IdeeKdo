using Android.Support.V4.App;
using Android.Views;
using IdeeKdo.Assets.ToolBox;

namespace IdeeKdo.Assets
{
    /// <summary>
    ///     Classe mêre de tous les fragments de l'application.
    /// </summary>
    public class FragmentMain : Fragment
    {
        public int IdTitle;
        public string StrTitle;

        /// <summary>
        ///     Initialisation du fragment.
        ///     C'est cette methode qu'il faut appeler lors de la création d'un fragment.
        /// </summary>
        /// <param name="iTitle">Resource titre par default</param>
        /// <param name="fragment">Fragment enfant</param>
        /// <param name="iLayout">Layout à afficher</param>
        /// <param name="inflater"></param>
        /// <param name="container"></param>
        /// <returns>Retourne la vue générée</returns>
        protected View InitView(int iTitle, FragmentMain fragment, int iLayout, LayoutInflater inflater,
            ViewGroup container)
        {
            IdTitle = iTitle;
            StrTitle = XTools.GetRsrcStr(iTitle);
            XTools.MainAct.SetTitle(fragment);
            return inflater.Inflate(iLayout, container, false);
        }

        /// <summary>
        ///     Permet de mettre à jour le titre de la bar d'action dynamiquement si aucune ressource n'est disponnible
        /// </summary>
        /// <param name="str">Chaine de caractere qui sera affichée en tant que titre</param>
        protected void SetStrTitle(string str) => StrTitle = str;
    }
}