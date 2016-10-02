
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.Graphics;
using Android.Support.V7.Widget;
using Android.Support.V7.App;
using Android.Support.V4.View;
using Android.Support.V4.App;
using Android.Support.V4.Widget;
using com.refractored;
using Android.Text;
using Android.Text.Style;
using Android.Content.PM;


namespace Fluffle.AndroidApp
{
	[Activity(Label = "LeaderboardFragment")]
	public class LeaderboardFragment : Android.Support.V4.App.Fragment, ViewPager.IOnPageChangeListener
	{
		public MainActivity MainPage { get; set; }
		private Android.Support.V7.Widget.Toolbar toolbar = null;
		public static LBLargestBunnyFragment largestFragment;
		public static LBMostBunniesFragment mostFragment;
		public static LBMostSharesFragment sharesFragment;
		public static LBMostSpreadFragment spreadFragment;
		private PagerSlidingTabStrip tabs;

		private ViewPager pager;

		// the page adapter
		public class LeaderboardPageAdapter : FragmentPagerAdapter, ICustomTabProvider
		{
			private string[] Titles = { "most bunnies", "most shares", "largest bunny", "furthest spread", };
			Android.Support.V7.App.AppCompatActivity activity;

			public LeaderboardPageAdapter(Android.Support.V4.App.FragmentManager fm, Android.Support.V7.App.AppCompatActivity theActivity)
				: base(fm)
			{
				activity = theActivity;
			}

			public override Java.Lang.ICharSequence GetPageTitleFormatted(int position)
			{
				return new Java.Lang.String(Titles[position]);
			}

			public override int Count
			{
				get
				{
					return Titles.Length;
				}
			}



			public View GetCustomTabView(ViewGroup parent, int position)
			{
				var tabView = activity.LayoutInflater.Inflate(Resource.Layout.NotifyTabView, null);
				var counter = tabView.FindViewById<TextView>(Resource.Id.counter);
				var title = tabView.FindViewById<TextView>(Resource.Id.tab_title);
				title.SetTypeface(MainActivity.bodyFace, TypefaceStyle.Normal);
				title.Text = Titles[position];
				counter.Visibility = ViewStates.Gone;
				return tabView;
			}

			public override Android.Support.V4.App.Fragment GetItem(int position)
			{
				Android.Support.V4.App.Fragment theItem = null;
				switch (position)
				{
					case 0:
						theItem = LeaderboardFragment.mostFragment;
						break;

					case 1:
						theItem = LeaderboardFragment.sharesFragment;
						break;

					case 2:
						theItem = LeaderboardFragment.largestFragment;
						break;

					case 3:
						theItem = LeaderboardFragment.spreadFragment;
						break;




				}
				return theItem;
			}
		}

		public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
		{
			// Use this to return your custom view for this Fragment
			// return inflater.Inflate(Resource.Layout.YourFragment, container, false);

			base.OnCreateView(inflater, container, savedInstanceState);

			var view = inflater.Inflate(Resource.Layout.LeaderboardLayout, container, false);


			mostFragment = new LBMostBunniesFragment();
			sharesFragment = new LBMostSharesFragment();
			largestFragment = new LBLargestBunnyFragment();
			spreadFragment = new LBMostSpreadFragment();

			pager = view.FindViewById<ViewPager>(Resource.Id.post_pager);
			pager.Adapter = new LeaderboardPageAdapter(MainPage.SupportFragmentManager, MainPage);
			pager.AddOnPageChangeListener(this);
			pager.OffscreenPageLimit = 2;
			tabs = view.FindViewById<PagerSlidingTabStrip>(Resource.Id.tabs);
			tabs.SetViewPager(pager);
			tabs.IndicatorColor = Resources.GetColor(Resource.Color.Fluffle_green);
			//tabs.TabTextColor = Resources.GetColorStateList(Resource.Color.tabTextColor);
			//tabs.TabTextColorSelected = Resources.GetColorStateList(Resource.Color.tabTextColor);
			tabs.IndicatorHeight = Resources.GetDimensionPixelSize(Resource.Dimension.tab_indicator_height);
			tabs.UnderlineColor = Resources.GetColor(Resource.Color.Fluffle_green);
			tabs.TabPaddingLeftRight = Resources.GetDimensionPixelSize(Resource.Dimension.tab_padding);
			tabs.OnPageChangeListener = this;
			//tabs.ShouldExpand = true;

			tabs.SetTabTextColor(Color.White);
			pager.CurrentItem = 0;


			return view;
		}

		public void OnPageScrolled(int position, float positionOffset, int positionOffsetPixels)
		{

		}

		public void OnPageScrollStateChanged(int state)
		{

		}

		public void OnPageSelected(int position)
		{
			switch (position)
			{
				case 0:
					// most bunnies
					mostFragment.Update();
					break;

				case 1:
					//shares
					sharesFragment.Update();
					break;

				case 2:
					// largest bunny
					largestFragment.Update();
					break;

				case 3:
					// spread
					spreadFragment.Update();
					break;


			}
		}

		public void Update()
		{

		}
	}
}

