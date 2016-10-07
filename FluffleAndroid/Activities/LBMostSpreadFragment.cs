
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;

using Fluffimax.Core;

namespace Fluffle.AndroidApp
{
	public class LBMostSpreadFragment : Android.Support.V4.App.Fragment
	{
		private ListView leaderList;
		private MostSpreadAdapter adapter;

		public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
		{
			View theView = inflater.Inflate(Resource.Layout.LBMostSpreadLayout, container, false);

			var headList = theView.FindViewById<TextView>(Resource.Id.title);
			headList.SetTypeface(MainActivity.bodyFace, Android.Graphics.TypefaceStyle.Bold);

			var subtitle = theView.FindViewById<TextView>(Resource.Id.subtitle);
			subtitle.SetTypeface(MainActivity.bodyFace, Android.Graphics.TypefaceStyle.Normal);

			leaderList = theView.FindViewById<ListView>(Resource.Id.theListView);

            Update();

            return theView;
		}

        public void Update()
        {
            Server.GetBunnySizeLB((theList) =>
            {
                Activity.RunOnUiThread(() =>
                {
                    adapter = new MostSpreadAdapter(this.Activity, theList);
                    leaderList.Adapter = adapter;
                    RefreshListView();
                });

            });
        }

        private void RefreshListView()
        {
            if (this.View != null)
            {
                Activity.RunOnUiThread(() =>
                {

                    adapter.NotifyDataSetChanged();

                    leaderList.InvalidateViews();
                });
            }
        }



    }

	public class MostSpreadAdapter : BaseAdapter<Bunny>
	{
		public List<Bunny> itemList;
		Activity context;

		public MostSpreadAdapter(Activity context, List<Bunny> theList) : base()
        {
            this.context = context;
            itemList = theList;
        }

        public override long GetItemId(int position)
		{
			return position;
		}
		public override Bunny this[int position]
		{
			get { return itemList[position]; }
		}
		public override int Count
		{
			get { return itemList.Count; }
		}

		public override View GetView(int position, View convertView, ViewGroup parent)
		{
			View view = convertView; // re-use an existing view, if one is available
			if (view == null)
			{
				view = context.LayoutInflater.Inflate(Resource.Layout.LBCellMostSpread, null);
			}

            var bunImage = view.FindViewById<ImageView>(Resource.Id.bunnyImage);
            var bunName = view.FindViewById<TextView>(Resource.Id.bunnyNameText);
            var playerImage = view.FindViewById<ImageView>(Resource.Id.playerImage);
            var playerName = view.FindViewById<TextView>(Resource.Id.playerNameText);
            var shares = view.FindViewById<TextView>(Resource.Id.bunnySharesText);

            if (convertView == null)
            {
                bunName.SetTypeface(MainActivity.bodyFace, Android.Graphics.TypefaceStyle.Normal);
                shares.SetTypeface(MainActivity.bodyFace, Android.Graphics.TypefaceStyle.Normal);
            }

            Bunny curBuns = itemList[position];

            Koush.UrlImageViewHelper.SetUrlDrawable(bunImage, curBuns.GetProfileImage(), Resource.Drawable.baseicon);


            string bunnyURL = curBuns.GetProfileImage();

            bunName.Text = string.IsNullOrEmpty(curBuns.BunnyName) ? "Unnamed bunny" : curBuns.BunnyName;

            shares.Text = Bunny.SizeString(curBuns.BunnySize);

            if (curBuns.CurrentOwner == 0)
            {
                // no owner
                playerName.Text = "no owner";
                playerImage.Visibility = ViewStates.Invisible;
            }
            else
            {
                playerName.Text = string.IsNullOrEmpty(curBuns.CurrentOwnerName) ? "unknown" : curBuns.CurrentOwnerName;
                playerImage.Visibility = ViewStates.Visible;
                Koush.UrlImageViewHelper.SetUrlDrawable(playerImage, curBuns.CurrentOwnerImg, Resource.Drawable.unknown_user);

            }

            return view;
		}
	}
}

