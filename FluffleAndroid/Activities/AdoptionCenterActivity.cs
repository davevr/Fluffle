
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Graphics;
using Android.Widget;

using Fluffimax.Core;

namespace Fluffle.AndroidApp
{
	[Activity(Label = "AdoptionCenterActivity", Icon = "@drawable/baseicon",
			 Theme = "@style/Theme.AppCompat.Light", ScreenOrientation = Android.Content.PM.ScreenOrientation.Portrait)]
	
	public class AdoptionCenterActivity : Activity
	{
		private TextView filterHeader;
		private LinearLayout filterDetails;
		private Button breedChoiceBtn;
		private Button sizeChoiceBtn;
		private Button genderChoiceBtn;
		private Button furChoiceBtn;
		private Button eyeChoiceBtn;
		private Button priceChoiceBtn;
		private GridView resultGrid;
		private BunnyListAdapter adapter;
		private Button filterBtn;

		private string curBreed = "any";
		private string curSize = "any";
		private string curGender = "any";
		private string curFur = "any";
		private string curEye = "any";
		private string curPrice = "any";

		private List<Bunny> storeList;

		protected override void OnCreate(Bundle savedInstanceState)
		{
			base.OnCreate(savedInstanceState);

			// Create your application here
			SetContentView(Resource.Layout.AdoptionCenterLayout);

			filterHeader = FindViewById<TextView>(Resource.Id.filterHeader);
			filterDetails = FindViewById<LinearLayout>(Resource.Id.FilterDetails);

			breedChoiceBtn = FindViewById<Button>(Resource.Id.breedChoiceBtn);

			sizeChoiceBtn = FindViewById<Button>(Resource.Id.sizeChoiceBtn);

			genderChoiceBtn = FindViewById<Button>(Resource.Id.genderChoiceBtn);

			furChoiceBtn = FindViewById<Button>(Resource.Id.furChoiceBtn);

			eyeChoiceBtn = FindViewById<Button>(Resource.Id.eyeChoiceBtn);

			priceChoiceBtn = FindViewById<Button>(Resource.Id.priceChoiceBtn);
			resultGrid = FindViewById<GridView>(Resource.Id.resultGrid);
			filterBtn = FindViewById<Button>(Resource.Id.filterBtn);

			filterHeader.Text = "checking for bunnies...";
			filterDetails.Visibility = ViewStates.Gone;
			filterBtn.Click += (sender, e) => { ToggleFilterArea(); };
			UpdateFilters();

		}

		protected override void OnPostResume()
		{
			base.OnPostResume();
			UpdateStoreList();
		}

		private void UpdateFilters()
		{
			breedChoiceBtn.Text = curBreed;
			genderChoiceBtn.Text = curGender;
			sizeChoiceBtn.Text = curSize;
			furChoiceBtn.Text = curFur;
			eyeChoiceBtn.Text = curEye;
			priceChoiceBtn.Text = curPrice;
		}

		private List<Bunny> FilterList()
		{
			List<Bunny> filterList = new List<Bunny>();

			foreach (Bunny curBuns in storeList)
			{
				bool addIt = true;
				if (curBreed != "any" && curBuns.BreedName != curBreed)
					addIt = false;
				else if (curSize != "any" && curBuns.BunnySize.ToString() != curSize)
					addIt = false;
				else if (curGender != "any")
				{
					// todo - check gender
					addIt = false;
				}
				else if (curFur != "any" && curBuns.FurColorName != curFur)
					addIt = false;
				else if (curEye != "any" && curBuns.EyeColorName != curEye)
					addIt = false;
				else if (curPrice != "any")
				{
					//todo - check pirce
					addIt = false;
				}
				if (addIt)
					filterList.Add(curBuns);
			}

			filterList.Sort((x, y) =>
			{
				return x.Price.CompareTo(y.Price);
			});

			return filterList;
		}

		private void ToggleFilterArea()
		{
			if (filterDetails.Visibility == ViewStates.Visible)
			{
				filterDetails.Visibility = ViewStates.Gone;
				filterBtn.Text = "Filter";
			}
			else {
				filterDetails.Visibility = ViewStates.Visible;
				filterBtn.Text = "Hide";
			}
		}

		private void UpdateStoreList()
		{
			Server.FetchStore((bunsList) =>
			{
				storeList = bunsList;
				RefreshListView();
			});
		}

		private void RefreshListView()
		{
			List<Bunny> bunsList = FilterList();
					
			if (adapter == null)
			{
				adapter = new BunnyListAdapter(this, bunsList);
			}

			RunOnUiThread(() =>
			{
				resultGrid.Adapter = adapter;
				filterHeader.Text = string.Format("{0} bunnies available", bunsList.Count);
				adapter.NotifyDataSetChanged();
				resultGrid.InvalidateViews();
			});
		}

	}

	public class BunnyListAdapter : BaseAdapter<Bunny>
	{
		public List<Bunny> allItems;
		AdoptionCenterActivity fragment;


		public BunnyListAdapter(AdoptionCenterActivity context, List<Bunny> theItems) : base()
		{
			this.fragment = context;
			this.allItems = theItems;
		}

		public override long GetItemId(int position)
		{
			return position;
		}
		public override Bunny this[int position]
		{
			get { return allItems[position]; }
		}
		public override int Count
		{
			get { return allItems.Count; }
		}

		public override View GetView(int position, View convertView, ViewGroup parent)
		{
			View view = convertView; // re-use an existing view, if one is available
			if (view == null)
			{
				view = fragment.LayoutInflater.Inflate(Resource.Layout.AdoptionCell, null);
			}
			var imageView = view.FindViewById<ImageView>(Resource.Id.bunnyImage);
			var nameView = view.FindViewById<TextView>(Resource.Id.bunnyNameLabel);
			var descView = view.FindViewById<TextView>(Resource.Id.bunnyDescLabel);
			var pricetag = view.FindViewById<TextView>(Resource.Id.priceTag);

			if (convertView == null)
			{
				// first time init
				nameView.SetTypeface(MainActivity.bodyFace, TypefaceStyle.Normal);
			}

			Bunny curBuns = allItems[position];
			string name = curBuns.BunnyName;
			if (string.IsNullOrEmpty(name))
				name = "new bunny";
			nameView.Text = name;
			descView.Text = curBuns.Description;
			pricetag.Text = string.Format("{0} c", curBuns.Price);
			Koush.UrlImageViewHelper.SetUrlDrawable(imageView, curBuns.GetProfileImage(), Resource.Drawable.baseicon);
			return view;
		}
	}
}

