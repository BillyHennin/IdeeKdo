using System;
using System.Collections.Generic;
using Android.Graphics;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;

namespace IdeeKdo.Assets
{
    public class Photo
    {
        public int Id;
        // Retourne le Bitmap de la photo
        public Bitmap photoBitmap;
        // Retourne le titre de la photo:
        public string photoTitre;
    }


    public class PhotoViewHolder : RecyclerView.ViewHolder
    {
        public new int Position;

        public PhotoViewHolder(View itemView, Action<int> listener, int? id = null) : base(itemView)
        {
            Image = itemView.FindViewById<ImageView>(Resource.Id.imageView);
            Position = id ?? 0;
            Image.Clickable = true;
            Image.Click += (s, e) => listener(Position);
            Caption = itemView.FindViewById<TextView>(Resource.Id.textView);
        }

        public ImageView Image { get; }
        public TextView Caption { get; }
    }


    // Photo album: holds image resource IDs and caption:
    public class PhotoAlbum
    {
        // Built-in photo collection - this could be replaced with
        // a photo database:
        // Array of photos that make up the album:
        public readonly List<Photo> MPhotos;

        // Create an instance copy of the built-in photo list and
        // create the random number generator:
        public PhotoAlbum()
        {
            MPhotos = new List<Photo>();
        }

        public int NumPhotos => MPhotos.Count;

        public Photo this[int i] => MPhotos[i];
    }

    public class PhotoAlbumAdapter : RecyclerView.Adapter
    {
        public PhotoAlbum MPhotoAlbum;

        public PhotoAlbumAdapter(PhotoAlbum photoAlbum)
        {
            MPhotoAlbum = photoAlbum;
        }

        // Return the number of photos available in the photo album:
        public override int ItemCount => MPhotoAlbum.NumPhotos;
        public event EventHandler<Photo> ItemClick;

        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            // Inflate the CardView for the photo:
            var itemView = LayoutInflater.From(parent.Context).Inflate(Resource.Layout.ListPhotoCardView, parent, false);
            // Create a ViewHolder to find and hold these view references, and 
            // register OnClick with the view holder:
            return new PhotoViewHolder(itemView, OnClick);
        }

        // Fill in the contents of the photo card (invoked by the layout manager):
        public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            var vh = holder as PhotoViewHolder;
            // Set the ImageView and TextView in this ViewHolder's CardView 
            // from this position in the photo album:
            if (vh == null)
                return;
            vh.Image.SetImageBitmap(MPhotoAlbum[position].photoBitmap);
            vh.Position = position;
            vh.Caption.Text = MPhotoAlbum[position].photoTitre;
        }

        // Raise an event when the item-click takes place:
        private void OnClick(int id) => ItemClick?.Invoke(this, MPhotoAlbum[id]);
    }
}