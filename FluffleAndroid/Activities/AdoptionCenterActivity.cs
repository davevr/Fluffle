
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
using Android.Animation;

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

		private List<string> breedChoices;
		private List<string> sizeChoices;
		private List<string> genderChoices;
		private List<string> furChoices;
		private List<string> eyeChoices;
		private List<string> priceChoices;
        private Bunny pendingBunny = null;
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
			filterHeader.SetTypeface(MainActivity.bodyFace, TypefaceStyle.Normal);
			resultGrid.ItemClick += ResultGrid_ItemClick;

			filterHeader.Text = "checking for bunnies...";
			filterDetails.Visibility = ViewStates.Gone;
			filterBtn.Click += (sender, e) => { ToggleFilterArea(); };
			UpdateFilters();

			breedChoiceBtn.Click += HandleBreedChoice;
			genderChoiceBtn.Click += HandleGenderChoice;
			sizeChoiceBtn.Click += HandleSizeChoice;
			furChoiceBtn.Click += HandleFurChoice;
			eyeChoiceBtn.Click += HandleEyeChoice;
			priceChoiceBtn.Click += HandlePriceChoice;

		}

		void HandleBreedChoice(object sender, EventArgs e)
		{
			new Android.Support.V7.App.AlertDialog.Builder(this)
			   .SetItems(breedChoices.ToArray(), (theSender, theEvent) =>
			   {
				   RunOnUiThread(() =>
				   {
					   curBreed = breedChoices[theEvent.Which];
					   UpdateFilters();
					   RefreshListView();
				   });
			   })
				.SetTitle("choose breed")
				.SetCancelable(true)
				.Show();
		}

		void HandleGenderChoice(object sender, EventArgs e)
		{
			new Android.Support.V7.App.AlertDialog.Builder(this)
	           .SetItems(genderChoices.ToArray(), (theSender, theEvent) =>
			   {
				   RunOnUiThread(() =>
				   {
					   curGender = genderChoices[theEvent.Which];
					   UpdateFilters();
					   RefreshListView();
				});
				})
				.SetTitle("choose bunny gender")
				.SetCancelable(true)
				.Show();
		}

		void HandleSizeChoice(object sender, EventArgs e)
		{
			new Android.Support.V7.App.AlertDialog.Builder(this)
			   .SetItems(sizeChoices.ToArray(), (theSender, theEvent) =>
			   {
				   RunOnUiThread(() =>
				   {
					   curSize = sizeChoices[theEvent.Which];
					   UpdateFilters();
					   RefreshListView();
				   });
			   })
				.SetTitle("choose bunny size")
				.SetCancelable(true)
				.Show();
		}

		void HandleEyeChoice(object sender, EventArgs e)
		{
			new Android.Support.V7.App.AlertDialog.Builder(this)
			   .SetItems(eyeChoices.ToArray(), (theSender, theEvent) =>
			   {
				   RunOnUiThread(() =>
				   {
					   curEye = eyeChoices[theEvent.Which];
					   UpdateFilters();
					   RefreshListView();
				   });
			   })
				.SetTitle("choose eye color")
				.SetCancelable(true)
				.Show();
		}

		void HandleFurChoice(object sender, EventArgs e)
		{
			new Android.Support.V7.App.AlertDialog.Builder(this)
			   .SetItems(furChoices.ToArray(), (theSender, theEvent) =>
			   {
				   RunOnUiThread(() =>
				   {
					   curFur = furChoices[theEvent.Which];
					   UpdateFilters();
					   RefreshListView();
				   });
			   })
				.SetTitle("choose fur color")
				.SetCancelable(true)
				.Show();
		}


        void ResultGrid_ItemClick(object sender, AdapterView.ItemClickEventArgs e)
		{
			Bunny theBuns = adapter.allItems[e.Position];
            if (Game.CurrentPlayer.carrotCount >= theBuns.Price)
            {
                pendingBunny = theBuns;
                new Android.Support.V7.App.AlertDialog.Builder(this)
                       .SetTitle(Resource.String.Confirm_Adoption_Title.Localize())
                       .SetMessage(string.Format(Resource.String.Confirm_Adoption_Prompt.Localize(), theBuns.Price))
                       .SetCancelable(true)
                       .SetPositiveButton(Resource.String.Adoption_OK_Btn.Localize(), (ps, pe) => { HandleBuyBunny(theBuns.id); })
                       .SetNegativeButton(Resource.String.Adoption_Cancel_Btn.Localize(), (ns, ne) => { })
                       .Show();
            } else
            {
                new Android.Support.V7.App.AlertDialog.Builder(this)
                       .SetTitle(Resource.String.Adoption_Declined_Title.Localize())
                       .SetMessage(Resource.String.Adoption_Lack_Funds.Localize())
                       .SetCancelable(true)
                       .SetPositiveButton(Resource.String.Adoption_Lack_Funds_Confirm.Localize(), (ps, pe) => {  })
                       .Show();
            }
			                       
		}

		void HandleBuyBunny(long bunnyId)
		{
            if (pendingBunny != null)
            {
                if (Game.CurrentPlayer.BuyBunny(pendingBunny))
                {
                    Game.BunnyStore.Remove(pendingBunny);
                    UpdateCarrotCount();
                    pendingBunny = null;
                    new Android.Support.V7.App.AlertDialog.Builder(this)
                       .SetTitle(Resource.String.Adoption_Accepted_Title.Localize())
                       .SetMessage(Resource.String.Adoption_Worked.Localize())
                       .SetCancelable(false)
                       .SetPositiveButton(Resource.String.Adoption_Worked_Btn.Localize(), (ps, pe) => { Finish(); })
                       .Show();
                }
                else
                {
                    pendingBunny = null;
                    new Android.Support.V7.App.AlertDialog.Builder(this)
                       .SetTitle(Resource.String.Adoption_Declined_Title.Localize())
                       .SetMessage(Resource.String.Adoption_Failed.Localize())
                       .SetCancelable(false)
                       .SetPositiveButton(Resource.String.Adoption_Failed_Btn.Localize(), (ps, pe) => { })
                       .Show();
                }
            }
        }

        void UpdateCarrotCount()
        {
            RunOnUiThread(() =>
            {

            });
        }

		void HandlePriceChoice(object sender, EventArgs e)
		{
			new Android.Support.V7.App.AlertDialog.Builder(this)
			   .SetItems(priceChoices.ToArray(), (theSender, theEvent) =>
			   {
				   RunOnUiThread(() =>
				   {
					   curPrice = priceChoices[theEvent.Which];
					   UpdateFilters();
					   RefreshListView();
				   });
			   })
				.SetTitle("choose priec")
				.SetCancelable(true)
				.Show();
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
				else if (curSize != "any" && Bunny.SizeString(curBuns.BunnySize) != curSize)
					addIt = false;
				else if (curGender != "any")
				{
					string genderStr = curBuns.Female ? "female" : "male";
					if (genderStr != curGender)
						addIt = false;
				}
				else if (curFur != "any" && curBuns.FurColorName != curFur)
					addIt = false;
				else if (curEye != "any" && curBuns.EyeColorName != curEye)
					addIt = false;
				else if (curPrice != "any")
				{
					if (curPrice == "> 2000") {
						if (curBuns.Price < 2000)
							addIt = false;
					} else {
						var price = int.Parse(curPrice.Substring(2));
						if (curBuns.Price > price)
							addIt = false;
					}
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
				PopulateFilterChoices();
				RefreshListView();
			});
		}

		private void PopulateFilterChoices()
		{
			breedChoices = new List<string>();
			breedChoices.Add("any");
			sizeChoices = new List<string>();
			sizeChoices.Add("any");
			eyeChoices = new List<string>();
			eyeChoices.Add("any");
			genderChoices = new List<string>();
			genderChoices.Add("any");
			priceChoices = new List<string>();
			priceChoices.Add("any");
			furChoices = new List<string>();
			furChoices.Add("any");

			foreach (Bunny curBuns in storeList)
			{
				if (!breedChoices.Contains(curBuns.BreedName))
					breedChoices.Add(curBuns.BreedName);
				
				if (!sizeChoices.Contains(Bunny.SizeString(curBuns.BunnySize)))
					sizeChoices.Add(Bunny.SizeString(curBuns.BunnySize));

				if (!eyeChoices.Contains(curBuns.EyeColorName))
					eyeChoices.Add(curBuns.EyeColorName);

				if (!furChoices.Contains(curBuns.FurColorName))
					furChoices.Add(curBuns.FurColorName);
				if (curBuns.Female)
				{
					if (!genderChoices.Contains("female"))
						genderChoices.Add("female");
				}
				else {
					if (!genderChoices.Contains("male"))
						genderChoices.Add("male");
				}

				if ((curBuns.Price < 250) && !priceChoices.Contains("< 250"))
					priceChoices.Add("< 250");
				else if ((curBuns.Price < 500) && !priceChoices.Contains("< 500"))
					priceChoices.Add("< 500");
				else if ((curBuns.Price < 1000) && !priceChoices.Contains("< 1000"))
					priceChoices.Add("< 1000");
				else if ((curBuns.Price < 2000) && !priceChoices.Contains("< 2000"))
					priceChoices.Add("< 2000");
				else if ( !priceChoices.Contains("> 2000"))
					priceChoices.Add("> 2000");
			}

		}

		private void RefreshListView()
		{
			List<Bunny> bunsList = FilterList();
					
			adapter = new BunnyListAdapter(this, bunsList);

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

