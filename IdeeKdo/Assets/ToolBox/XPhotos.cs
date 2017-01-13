using System;
using System.Json;
using Android.Graphics;
using Android.Util;
using IdeeKdo.Activities;

namespace IdeeKdo.Assets.ToolBox
{
    public static class XPhotos
    {
        private const string Host = "reqres.in";

        public static void ShowPhotos(ref PhotoAlbum mPhotoAlbum, Action<object> actionUi,
            ref PhotoAlbumAdapter mAdapter)
        {
            if (mAdapter == null)
            {
                mAdapter = new PhotoAlbumAdapter(mPhotoAlbum);
                mAdapter.ItemClick += XTools.OnItemListClick;
            }
            Xui.ShowOnMainUi(obj => { actionUi.Invoke(null); });
        }


        public static void GetUsersInfos(ref int iPage, ref int iTotalPages, ref int iTotalUser,
            ref PhotoAlbum mPhotoAlbum, Action<object> actionUi,
            ref PhotoAlbumAdapter mAdapter)
        {
            var url = $"https://{Host}/api/users?page={iPage}";
            if (!XNetwork.PingRequest(Host))
            {
                XMessage.ShowError(Resource.String.ErrorServer);
                return;
            }
            try
            {
                var json = XNetwork.GetJsonFromWeb(url).Result;
                iTotalPages = json["total_pages"];
                iTotalUser = json["total"];
                iPage = json["page"];
                var listUser = json["data"];
                foreach (JsonValue user in listUser)
                {
                    int id;
                    XInputs.IsNumeric(user["id"].ToString().Trim(' ', '"'), out id);
                    var strCaption = $"{user["first_name"]} {user["last_name"]}".Trim(' ', '"').Replace("\" \"", " ");
                    var photo = new Photo
                    {
                        Id = id,
                        photoTitre = strCaption,
                        photoBitmap = XNetwork.GetImageBitmapFromUrl(user["avatar"])
                    };
                    if (mPhotoAlbum == null)
                    {
                        mPhotoAlbum = new PhotoAlbum();
                    }
                    //Verifie qu'il n'y a pas de doublons dans les photos recupérées
                    if (mPhotoAlbum.MPhotos.Find(t => t.Id == id) == null)
                    {
                        mPhotoAlbum.MPhotos.Add(photo);
                    }
                }
                ShowPhotos(ref mPhotoAlbum, actionUi, ref mAdapter);
            }
            catch
            {
                //Le serveur ne reponds pas / erreur inconnue
                XLog.Write(LogPriority.Info, XTools.GetRsrcStr(Resource.String.ErrorServer));
                XMessage.ShowError(Resource.String.ErrorServer);
            }
        }

        public static void GetSingleUser(ref string tvTitle, ref Bitmap ivUser, int userId, Action<object> actionUi)
        {
            var url = $"https://{Host}/api/users/{userId}";
            try
            {
                var json = XNetwork.GetJsonFromWeb(url).Result;
                var user = json["data"];
                tvTitle = $"{user["first_name"]} {user["last_name"]}".Trim(' ', '"').Replace("\" \"", " ");
                ivUser = XNetwork.GetImageBitmapFromUrl(user["avatar"]);
                Xui.ShowOnMainUi(obj => { actionUi.Invoke(null); });
            }
            catch (Exception e)
            {
                //Le serveur ne reponds pas / erreur inconnue
                XLog.Write(LogPriority.Info, e.Message);
                XMessage.ShowError(e.Message);
                XTools.AwaitAction(obj => XTools.ChangeFragment(Main.GetHome()));
            }
        }
    }
}