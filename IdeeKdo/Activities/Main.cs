using System.Net;
using Android.App;
using Android.Content.PM;
using Android.OS;
using Android.Support.Design.Widget;
using Android.Support.V4.Widget;
using Android.Support.V7.App;
using Android.Widget;
using IdeeKdo.Assets;
using IdeeKdo.Assets.Tools;
using IdeeKdo.Fragments;
using Toolbar = Android.Support.V7.Widget.Toolbar;

namespace IdeeKdo.Activities
{
    [Activity(Theme = "@style/MyTheme", NoHistory = true,
        ConfigurationChanges = ConfigChanges.Orientation | ConfigChanges.ScreenSize)]
    public class Main : AppCompatActivity
    {
        private static NavigationView _navigationView;
        private static FragmentHome _fragmentHome;
        private FragmentAbout _fragmentAbout;
        private FragmentList _fragmentList;
        private FragmentRecycler _fragmentRecycler;
        private FragmentSettings _fragmentSettings;
        private FragmentWeather _fragmentWeather;
        public DrawerLayout DrawerLayout;

        /// <summary>
        ///     Initialisation des fragments qui seront utilis�s dans tout le cycle de vie de l'application
        /// </summary>
        private void InitFragment()
        {
            _fragmentWeather = new FragmentWeather();
            _fragmentHome = GetHome();
            _fragmentSettings = new FragmentSettings();
            _fragmentAbout = new FragmentAbout();
            _fragmentRecycler = new FragmentRecycler();
            _fragmentList = new FragmentList();
        }

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            //Initialisation de quelques valeurs statics
            XTools.MainAct = this;
            ServicePointManager.ServerCertificateValidationCallback += delegate { return true; };
            InitFragment();
            SetContentView(Resource.Layout.ActivityLayoutMain);
            XTools.ChangeFragment(_fragmentHome, false);

            // Initialisation de la bar d'action
            var toolbar = FindViewById<Toolbar>(Resource.Id.action_bar);
            SetSupportActionBar(toolbar);
            SupportActionBar.SetDisplayHomeAsUpEnabled(false);
            SupportActionBar.SetDisplayShowHomeEnabled(true);

            // Ajout de la fonction 'NavigationView_NavigationItemSelected' � l'evenement 'NavigationItemSelected'.
            _navigationView = FindViewById<NavigationView>(Resource.Id.nav_view);
            _navigationView.NavigationItemSelected += NavigationView_NavigationItemSelected;
            _navigationView.GetHeaderView(0).FindViewById<TextView>(Resource.Id.txtViewTitle).Text =
                XTools.User.NomComplet;

            // Creation d'un bouton ActionBarDrawerToggle et ajout � la bar d'action.
            DrawerLayout = FindViewById<DrawerLayout>(Resource.Id.drawer_layout);
            var drawerToggle = new ActionBarDrawerToggle(this, DrawerLayout, toolbar, Resource.String.MenuOpened,
                Resource.String.MenuHome);
            DrawerLayout.AddDrawerListener(drawerToggle);
            drawerToggle.SyncState();
        }

        /// <summary>
        ///     Fonction appel�e lorsque l'utilisateur choisi un item dans la vue de navigation (Menu lateral gauche)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void NavigationView_NavigationItemSelected(object sender,
            NavigationView.NavigationItemSelectedEventArgs e)
        {
            switch (e.MenuItem.ItemId)
            {
                case Resource.Id.nav_home:
                    XTools.ChangeFragment(_fragmentHome);
                    break;
                case Resource.Id.nav_weather:
                    XTools.ChangeFragment(_fragmentWeather);
                    break;
                case Resource.Id.nav_recycler:
                    XTools.ChangeFragment(_fragmentRecycler);
                    break;
                case Resource.Id.nav_list:
                    XTools.ChangeFragment(_fragmentList);
                    break;
                case Resource.Id.nav_settings:
                    XAction.AddToStack(obj => XMessage.ShowMessage("Succès !", "Reprise d'action r�ussie"));
                    XAction.AddToStack(obj => Xui.SetSnackMessage(Resource.String.Patienter));
                    XAction.AddToStack(obj => XTools.ChangeFragment(_fragmentSettings));
                    XMessage.ShowNotification("Actions ajout�es au stack",
                        "Activez puis d�sactivez le mode avion pour essayer le syst�me de reprise d'action.");
                    XTools.ChangeFragment(_fragmentSettings);
                    break;
                case Resource.Id.nav_disconnect:
                    XMessage.ShowQuestion(Resource.String.MenuDisconnect, Resource.String.DialogDisconnect,
                        aYes => XTools.User.Deconnexion(this), null);
                    break;
                case Resource.Id.nav_about:
                    Xui.SetSnackMessage(Resource.String.ErrorMenuItem);
                    XTools.ChangeFragment(_fragmentAbout);
                    break;
                default:
                    Xui.SetSnackMessage(Resource.String.ErrorMenuItem);
                    break;
            }
            // Une fois la selection faites, on cache le menu
            DrawerLayout.CloseDrawers();
        }

        #region Set Values

        /// <summary>
        ///     Change le titre qui apparait en haute � gauche de l'application (Dans la bar d'action)
        /// </summary>
        /// <param name="fragment"></param>
        public void SetTitle(FragmentMain fragment)
        {
            SupportActionBar.Title = fragment.StrTitle;
            //Verification du fragment � afficher, si il apparait dans le menu lateral, alors la ligne correspondante est mise en avant
            var l = GetIndex(fragment);
            var i = l >= 0 ? l : 0;
            var menuITem = _navigationView.Menu.GetItem(i).SetChecked(true);
            //S'il le fragment n'est pas dans la liste, aucune ligne n'est mise en avant.
            if (l == -1)
                menuITem.SetChecked(false);
        }

        #endregion

        #region Get Values

        /// <summary>
        ///     Recuperation du fragment servant d'accueil, si aucun n'est disponnible, on utilise celui par defaut
        /// </summary>
        /// <returns>Fragment � d'accueil</returns>
        public static FragmentHome GetHome() => _fragmentHome ?? new FragmentHome();

        /// <summary>
        ///     Recuperation de l'index suivant le fragment pass� en parametre
        /// </summary>
        /// <param name="fragment">Fragment</param>
        /// <returns>Num�ro dans la liste (Menu lateral)</returns>
        private static int GetIndex(FragmentMain fragment)
        {
            int index;
            switch (fragment.IdTitle)
            {
                case Resource.String.HomeTitle:
                case Resource.String.ChangeMail:
                case Resource.String.ChangePWD:
                    index = 0;
                    break;
                case Resource.String.WeatherTitle:
                    index = 1;
                    break;
                case Resource.String.SettingsTitle:
                    index = 2;
                    break;
                case Resource.String.RecyclerTitle:
                    index = 3;
                    break;
                case Resource.String.ListTitle:
                    index = 4;
                    break;
                default:
                    index = -1;
                    break;
            }
            return index;
        }

        #endregion
    }
}