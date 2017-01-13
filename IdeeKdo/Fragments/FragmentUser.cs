using System;
using Android.Graphics;
using Android.OS;
using Android.Util;
using Android.Views;
using Android.Widget;
using IdeeKdo.Activities;
using IdeeKdo.Assets;
using IdeeKdo.Assets.ToolBox;

namespace IdeeKdo.Fragments
{
    public class FragmentUser : FragmentMain
    {
        private LinearLayout _layoutMain;
        private int _userId;
        private Bitmap _userImage;
        private ImageView _userImageView;
        private string _userName;
        private TextView _userTextView;

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            var view = InitView(Resource.String.UserTitle, this, Resource.Layout.FragmentLayoutUser, inflater, container);
            try
            {
                _userId = Arguments.GetInt("UserId");
                //Set layout
                _layoutMain = view.FindViewById<LinearLayout>(Resource.Id.LineMainLayoutUser);
                //Set TitleText
                _userTextView = view.FindViewById<TextView>(Resource.Id.UserInfoText);
                //Set Image
                _userImageView = view.FindViewById<ImageView>(Resource.Id.UserImageView);
                XTools.AwaitAction(
                    act =>
                        XPhotos.GetSingleUser(ref _userName, ref _userImage, _userId, obj => InitPage()));
            }
            catch (Exception e)
            {
                XLog.Write(LogPriority.Error, e.Message);
                XTools.AwaitAction(obj => XTools.ChangeFragment(Main.GetHome()));
            }
            return view;
        }

        private void InitPage()
        {
            Xui.ToggleVisibility(_layoutMain);
            SetStrTitle(_userName);
            XTools.MainAct.SetTitle(this);
            _userTextView.Text = _userName;
            _userImageView.SetImageBitmap(_userImage);
        }
    }
}