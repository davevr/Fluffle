
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
	public class LBMostSharesFragment : Android.Support.V4.App.Fragment
	{
		private ListView leaderList;
		private MostSharesAdapter adapter;

		public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
		{
			View theView = inflater.Inflate(Resource.Layout.LBMostSharesLayout, container, false);

			var headList = theView.FindViewById<TextView>(Resource.Id.title);
			headList.SetTypeface(MainActivity.bodyFace, Android.Graphics.TypefaceStyle.Bold);

			var subtitle = theView.FindViewById<TextView>(Resource.Id.subtitle);
			subtitle.SetTypeface(MainActivity.bodyFace, Android.Graphics.TypefaceStyle.Normal);

			leaderList = theView.FindViewById<ListView>(Resource.Id.theListView);

			adapter = new MostSharesAdapter(this.Activity);

			RefreshListView();

			return theView;
		}

		public void Update()
		{

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

	public class MostSharesAdapter : BaseAdapter<Player>
	{
		public List<Player> itemList;
		Activity context;

		public MostSharesAdapter(Activity context) : base()
		{
			this.context = context;
			itemList = new List<Player>();
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
				view = context.LayoutInflater.Inflate(Resource.Layout.LBCellMostShares, null);
			}



			return view;
		}
	}
}

