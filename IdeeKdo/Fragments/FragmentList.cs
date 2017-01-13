using Android.OS;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using IdeeKdo.Assets;
using IdeeKdo.Assets.ToolBox;

namespace IdeeKdo.Fragments
{
    public class FragmentList : FragmentMain
    {
        private Button _btnNext;
        private Button _btnPrev;
        private int _iPage;
        private int _iTotalPages;
        private int _itotalUser;
        private LinearLayout _lnListBtn;
        private PhotoAlbumAdapter _mAdapter;
        private RecyclerView.LayoutManager _mLayoutManager;
        private PhotoAlbum _mPhotoAlbum;
        private RecyclerView _mRecyclerView;
        private TextView _paginaEditText;

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            var view = InitView(Resource.String.ListTitle, this, Resource.Layout.FragmentLayoutList, inflater, container);
            _lnListBtn = view.FindViewById<LinearLayout>(Resource.Id.linearLayoutListBtn);
            _mLayoutManager = new LinearLayoutManager(XTools.MainAct);
            _paginaEditText = view.FindViewById<TextView>(Resource.Id.PaginationText);
            _btnNext = view.FindViewById<Button>(Resource.Id.ListNextButton);
            _btnNext.Click += (s, e) =>
            {
                _iPage++;
                ChargeData();
            };
            _btnPrev = view.FindViewById<Button>(Resource.Id.ListPreviousButton);
            _btnPrev.Click += (s, e) =>
            {
                _iPage--;
                ChargeData();
            };
            Xui.SetVisibility(_btnPrev, false);
            _iPage = 1;
            ChargeData();
            return view;
        }

        private void UpdateBtnState()
        {
            Xui.SetVisibility(_lnListBtn, true);
            if (_iPage == 1)
            {
                Xui.SetVisibility(_btnPrev, false);
            }
            else if (_iPage == _iTotalPages)
            {
                Xui.SetVisibility(_btnNext, false);
            }
            else
            {
                Xui.SetVisibility(_btnPrev, true);
                Xui.SetVisibility(_btnNext, true);
            }
        }

        private void DoOnUiThread()
        {
            _mAdapter.MPhotoAlbum = _mPhotoAlbum;
            _mRecyclerView = XTools.MainAct.FindViewById<RecyclerView>(Resource.Id.recyclerViewList);
            _mRecyclerView.SetLayoutManager(_mLayoutManager);
            _mRecyclerView.SetAdapter(_mAdapter);
            _paginaEditText.Text = $"Page {_iPage}/{_iTotalPages} / Utilisateurs : {_itotalUser}";
            UpdateBtnState();
            _mPhotoAlbum = null;
            //_mAdapter = null;
        }

        private void ChargeData()
        {
            XTools.AwaitAction(
                obj =>
                    XPhotos.GetUsersInfos(ref _iPage, ref _iTotalPages, ref _itotalUser, ref _mPhotoAlbum,
                        act => DoOnUiThread(), ref _mAdapter));
        }
    }
}