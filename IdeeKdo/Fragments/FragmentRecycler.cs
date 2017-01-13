using System.Threading.Tasks;
using Android.OS;
using Android.Support.V4.Widget;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using IdeeKdo.Assets;
using IdeeKdo.Assets.ToolBox;
using IdeeKdo.Listener;

namespace IdeeKdo.Fragments
{
    public class FragmentRecycler : FragmentMain
    {
        private static PhotoAlbum _mPhotoAlbum;
        private static int _iPage;
        private int _iTotalPages;
        private int _iTotalUser;
        private PhotoAlbumAdapter _mAdapter;
        private RecyclerView.LayoutManager _mLayoutManager;
        private RecyclerView _mRecyclerView;
        private SwipeRefreshLayout _mSwipeRefreshLayout;
        private IParcelable _recyclerViewState;

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            var view = InitView(Resource.String.RecyclerTitle, this,
                Resource.Layout.FragmentLayoutRecycler, inflater,
                container);
            _mSwipeRefreshLayout = view.FindViewById<SwipeRefreshLayout>(Resource.Id.swipeRefreshLayout);
            _mSwipeRefreshLayout.Refresh += (s, e) =>
            {
                _iPage = 0;
                _mPhotoAlbum = null;
                XTools.AwaitAction(obj => ChargeData());
                _mSwipeRefreshLayout.Refreshing = false;
            };

            _mLayoutManager = new LinearLayoutManager(XTools.MainAct);
            var onScrollListener = new XamarinRecyclerViewOnScrollListener((LinearLayoutManager) _mLayoutManager);
            var prgBar = view.FindViewById<LinearLayout>(Resource.Id.linearLayoutRecylcerBot);
            onScrollListener.LoadMoreEvent += async (s, e) =>
            {
                Xui.ShowOnMainUi(delegate { Xui.ToggleVisibility(prgBar); });
                await Task.Run(delegate { ChargeData(); });
                Xui.ShowOnMainUi(delegate { Xui.ToggleVisibility(prgBar); });
            };
            _mRecyclerView = view.FindViewById<RecyclerView>(Resource.Id.recyclerView);
            _mRecyclerView.AddOnScrollListener(onScrollListener);
            _mRecyclerView.HasFixedSize = true;
            _mRecyclerView.SetLayoutManager(_mLayoutManager);
            //Si '_mPhotoAlbum' est null, alors la page est chargée pour la premiere fois et il faut donc initialiser le chargement
            if (_mPhotoAlbum == null)
            {
                XTools.AwaitAction(obj => ChargeData());
            }
            else
            {
                XTools.AwaitAction(obj => XPhotos.ShowPhotos(ref _mPhotoAlbum, action => DoOnUiThread(), ref _mAdapter));
            }
            return view;
        }

        private void DoOnUiThread()
        {
            //Mettre à jour la liste visible
            _mAdapter.MPhotoAlbum = _mPhotoAlbum;
            _mRecyclerView.SetAdapter(_mAdapter);
            //Reprendre la vue à l'endroit laissé (mettre à jour la liste sans cette ligne remet la vue à 0 et remonte tout).
            _mRecyclerView.GetLayoutManager().OnRestoreInstanceState(_recyclerViewState);
        }

        private void ChargeData()
        {
            //Sauvegarder l'etat de la vue (position exact, ....) pour reprendre au meme endroit une fois la liste mise à jour
            _recyclerViewState = _mRecyclerView.GetLayoutManager().OnSaveInstanceState();
            if (_iPage > _iTotalPages)
            {
                return;
            }
            _iPage++;
            XPhotos.GetUsersInfos(ref _iPage, ref _iTotalPages, ref _iTotalUser, ref _mPhotoAlbum, obj => DoOnUiThread(),
                ref _mAdapter);
        }
    }
}