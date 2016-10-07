
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
	public class LBMostBunniesFragment : Android.Support.V4.App.Fragment
	{
		private ListView leaderList;
		private MostBunniesAdapter adapter;

		public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
		{
			View theView = inflater.Inflate(Resource.Layout.LBMostBunniesLayout, container, false);

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
            Server.GetPlayerCountLB((theList) =>
            {
                Activity.RunOnUiThread(() =>
                {
                    adapter = new MostBunniesAdapter(this.Activity, theList);
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

    public class MostBunniesAdapter : BaseAdapter<Player>
	{
		public List<Player> itemList;
		Activity context;

		public MostBunniesAdapter(Activity context, List<Player> theList) : base()
		{
			this.context = context;
            itemList = theList;

        }
		public override long GetItemId(int position)
		{
			return position;
		}
		public override Player this[int position]
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
				view = context.LayoutInflater.Inflate(Resource.Layout.LBCellMostBunnies, null);
			}
            var playerImage = view.FindViewById<ImageView>(Resource.Id.profileImage);
            var playerName = view.FindViewById<TextView>(Resource.Id.playerNameText);
            var dateJoined = view.FindViewById<TextView>(Resource.Id.dateJoinedText);
            var bunCount = view.FindViewById<TextView>(Resource.Id.bunnyCountText);

            if (convertView == null)
            {
                playerName.SetTypeface(MainActivity.bodyFace, Android.Graphics.TypefaceStyle.Normal);
                bunCount.SetTypeface(MainActivity.bodyFace, Android.Graphics.TypefaceStyle.Normal);
            }
            Player curPlayer = itemList[position];

            playerName.Text = string.IsNullOrEmpty(curPlayer.nickname) ? "unknown" : curPlayer.nickname;
            dateJoined.Text = string.Format("joined {0}", curPlayer.creationDate.ToShortDateString());
            Koush.UrlImageViewHelper.SetUrlDrawable(playerImage, curPlayer.userimage, Resource.Drawable.unknown_user);

            bunCount.Text = curPlayer.totalBunnies.ToString();
            return view;
		}
	}
}

