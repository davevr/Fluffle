using System;
using Foundation;
using UIKit;
using Fluffimax.Core;
using System.Collections.Generic;

namespace Fluffimax.iOS
{
	public class BuyDelegate : UIAlertViewDelegate {
		public BunnyShopViewController ShopView { get; set; }

		public override void Clicked(UIAlertView alertview, nint buttonIndex)
		{
			if (buttonIndex == 1)
				ShopView.PurchaseBunny ();

		}
	}

	public partial class BunnyShopViewController : UIViewController
	{
		private BunnyShopCollectionSource dataSource;
		private BunnyShopCollectionDelegate theDelegate;
		private Bunny pendingBunny = null;
		private BuyDelegate buyDelegate;
		private List<Bunny> shopList;
		public List<Bunny> CurrentFilterSet;
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


		public BunnyShopViewController () : base ("BunnyShopViewController", null)
		{
			buyDelegate = new BuyDelegate();
			buyDelegate.ShopView = this;
		}

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();
			UIBarButtonItem menuBtn = new UIBarButtonItem("back_btn".Localize(), UIBarButtonItemStyle.Bordered, null);
			this.NavigationItem.BackBarButtonItem = menuBtn;

			// Perform any additional setup after loading the view, typically from a nib.
			AdoptionCollection.RegisterNibForCell(UINib.FromName(AdoptionViewCell.Key, NSBundle.MainBundle), AdoptionViewCell.Key);
			this.AutomaticallyAdjustsScrollViewInsets = false;
			dataSource = new BunnyShopCollectionSource ();
			theDelegate = new BunnyShopCollectionDelegate();
			theDelegate.Controller = this;

			AdoptionCollection.Source = dataSource;
			AdoptionCollection.Delegate = theDelegate;
			AdoptionCollection.BackgroundColor = UIColor.White;

			var flow = AdoptionCollection.CollectionViewLayout as UICollectionViewFlowLayout;
			flow.SectionInset = new UIEdgeInsets(0, 8, 8, 16);


			FilterPanel.ClipsToBounds = true;

			dataSource.ShopView = this;
			this.Title = "Adoption_Agency".Localize();
			UpdateCarrotCount ();
			View.LayoutIfNeeded();

			FilterBtn.TouchUpInside += (sender, e) => { ToggleFilterPanel(); };
			BreedBtn.TouchUpInside += HandleBreedChoice;
			GenderBtn.TouchUpInside += HandleGenderChoice;
			SizeBtn.TouchUpInside += HandleSizeChoice;
			FurBtn.TouchUpInside += HandleFurChoice;
			EyesBtn.TouchUpInside += HandleEyeChoice;
			PriceBtn.TouchUpInside += HandlePriceChoice;
		}


		void HandleBreedChoice(object sender, EventArgs e)
		{
			UIActionSheet sheet = new UIActionSheet("Choose_Breed".Localize(), null, "cancel_btn".Localize(), null, breedChoices.ToArray());

			sheet.Clicked += (theSender, theEvent) =>
			{
				var index = (int)theEvent.ButtonIndex;
				if (index < breedChoices.Count)
				{
					curBreed = breedChoices[index];
					UpdateFilters();
					RefreshListView();
				}
			};

			sheet.ShowInView(View);
		}

		void HandleGenderChoice(object sender, EventArgs e)
		{
			UIActionSheet sheet = new UIActionSheet("Choose_Gender".Localize(), null, "cancel_btn".Localize(), null, genderChoices.ToArray());

			sheet.Clicked += (theSender, theEvent) =>
			{
				var index = (int)theEvent.ButtonIndex;
				if (index < genderChoices.Count)
				{
					curGender = genderChoices[index];
					UpdateFilters();
					RefreshListView();
				}
			};

			sheet.ShowInView(View);
		}

		void HandleSizeChoice(object sender, EventArgs e)
		{
			UIActionSheet sheet = new UIActionSheet("Choose_Size".Localize(), null, "cancel_btn".Localize(), null, sizeChoices.ToArray());

			sheet.Clicked += (theSender, theEvent) =>
			{
				var index = (int)theEvent.ButtonIndex;
				if (index < sizeChoices.Count)
				{
					curSize = sizeChoices[index];
					UpdateFilters();
					RefreshListView();
				}
			};

			sheet.ShowInView(View);
		}

		void HandleFurChoice(object sender, EventArgs e)
		{
			UIActionSheet sheet = new UIActionSheet("Choose_Fur".Localize(), null, "cancel_btn".Localize(), null, furChoices.ToArray());

			sheet.Clicked += (theSender, theEvent) =>
			{
				var index = (int)theEvent.ButtonIndex;
				if (index < furChoices.Count)
				{
					curFur = furChoices[index];
					UpdateFilters();
					RefreshListView();
				}
			};

			sheet.ShowInView(View);
		}

		void HandleEyeChoice(object sender, EventArgs e)
		{
			UIActionSheet sheet = new UIActionSheet("Chooice_Eyes".Localize(), null, "cancel_btn".Localize(), null, eyeChoices.ToArray());

			sheet.Clicked += (theSender, theEvent) =>
			{
				var index = (int)theEvent.ButtonIndex;
				if (index < eyeChoices.Count)
				{
					curEye = eyeChoices[index];
					UpdateFilters();
					RefreshListView();
				}
			};

			sheet.ShowInView(View);
		}

		void HandlePriceChoice(object sender, EventArgs e)
		{
			UIActionSheet sheet = new UIActionSheet("Chooce_Price".Localize(), null, "cancel_btn".Localize(), null, priceChoices.ToArray());

			sheet.Clicked += (theSender, theEvent) =>
			{
				var index = (int)theEvent.ButtonIndex;
				if (index < priceChoices.Count)
				{
					curPrice = priceChoices[index];
					UpdateFilters();
					RefreshListView();
				}
			};

			sheet.ShowInView(View);
		}


		private void ToggleFilterPanel()
		{
			if (FilterPanel.Hidden)
			{
				FilterPanel.Layer.Opacity = 0;
				FilterPanel.Hidden = false;
				FilterPanelHeight.Constant = 0;
				UIView.Animate(1, () =>
				{
					FilterPanelHeight.Constant = 180;
					FilterPanel.Layer.Opacity = 1;
					View.LayoutIfNeeded();
				}, () =>
				{
					InvokeOnMainThread(() =>
					{
						FilterPanelHeight.Constant = 180;
						FilterPanel.Layer.Opacity = 1;
					});
				});
			}
			else {
				UIView.Animate(1, () =>
				{
					FilterPanelHeight.Constant = 0;
					FilterPanel.Layer.Opacity = 0;
					View.LayoutIfNeeded();
				}, () =>
				{
					InvokeOnMainThread(() =>
					{
						FilterPanelHeight.Constant = 0;
						FilterPanel.Layer.Opacity = 0;
						FilterPanel.Hidden = true;
					});
				});
			}
		}

		private void UpdateCarrotCount() {
			BeginInvokeOnMainThread(() => {
				CarrotCountLabel.Text = String.Format("carrot_count".Localize(), Game.CurrentPlayer.carrotCount);
			});
		}

		public override void DidReceiveMemoryWarning ()
		{
			base.DidReceiveMemoryWarning ();
			// Release any cached data, images, etc that aren't in use.
		}

		public override void ViewWillAppear (bool animated)
		{
			base.ViewWillAppear (animated);
			NavController.NavigationBarHidden = false;
			UpdateFilters();
			UpdateStoreList();

			var flow = AdoptionCollection.CollectionViewLayout as UICollectionViewFlowLayout;
			flow.SectionInset = new UIEdgeInsets(0, 8, 8, 8);

		}

		public override void ViewDidLayoutSubviews()
		{
			base.ViewDidLayoutSubviews();
			var flow = AdoptionCollection.CollectionViewLayout as UICollectionViewFlowLayout;
			int width = (int)AdoptionCollection.Bounds.Width;
			width -= 40;
			width /= 2;
			flow.ItemSize = new CoreGraphics.CGSize(width, flow.ItemSize.Height);
		}


		private void UpdateStoreList()
		{
			Server.FetchStore((bunsList) =>
			{
				shopList = bunsList;
				PopulateFilterChoices();
				RefreshListView();
			});
		}

		private void UpdateFilters()
		{
			BreedBtn.SetTitle(curBreed, UIControlState.Normal);
			GenderBtn.SetTitle(curGender, UIControlState.Normal);
			SizeBtn.SetTitle(curSize, UIControlState.Normal);
			FurBtn.SetTitle(curFur, UIControlState.Normal);
			EyesBtn.SetTitle(curEye, UIControlState.Normal);
			PriceBtn.SetTitle(curPrice, UIControlState.Normal);
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

			foreach (Bunny curBuns in shopList)
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
				else if (!priceChoices.Contains("> 2000"))
					priceChoices.Add("> 2000");
			}

		}

		private void RefreshListView()
		{
			CurrentFilterSet = FilterList();


			InvokeOnMainThread(() =>
			{
				dataSource.SetStoreList(CurrentFilterSet);
				AdoptionCollection.ReloadData();

				FilterResultText.Text = string.Format("{0} bunnies available", CurrentFilterSet.Count);

				HomeViewController.ShowTutorialStep("adoption_store_tutorial", "adoption_store_tutorial".Localize());

			});
		}

		private List<Bunny> FilterList()
		{
			List<Bunny> filterList = new List<Bunny>();

			foreach (Bunny curBuns in shopList)
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
					if (curPrice == "> 2000")
					{
						if (curBuns.Price < 2000)
							addIt = false;
					}
					else {
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

		public void MaybeBuyBunny(Bunny theBuns) {
			if (Game.CurrentPlayer.carrotCount >= theBuns.Price) {
				pendingBunny = theBuns;
				UIAlertView confirmView = new UIAlertView ("Confirm_Adoption_Title".Localize(), String.Format ("Confirm_Adoption_Prompt".Localize(), theBuns.Price),
				                                           buyDelegate, "Adoption_Cancel_Btn".Localize(), new string[] {"Adoption_OK_Btn".Localize()});
				confirmView.Show ();
			} else {
				UIAlertView denyView = new UIAlertView ("Adoption_Declined_Title".Localize(), "Adoption_Lack_Funds".Localize(), null, "Adoption_Lack_Funds_Confirm".Localize());
				denyView.Show ();
			}
		}

		public void PurchaseBunny() {
			if (pendingBunny != null) {
				if (Game.CurrentPlayer.BuyBunny (pendingBunny)) {
					Game.BunnyStore.Remove (pendingBunny);
					UpdateCarrotCount ();
					UIAlertView goodNews = new UIAlertView ("Adoption_Accepted_Title".Localize(), "Adoption_Worked".Localize(), null, "Adoption_Worked_Btn".Localize());
					goodNews.Show ();
					//BunnySaleList.ReloadData ();
					NavController.PopViewController(true);
				} else {
					UIAlertView denyView = new UIAlertView ("Adoption_Declined_Title".Localize(), "Adoption_Failed".Localize(), null, "Adoption_Failed_Btn".Localize());
					denyView.Show ();
				}
			}
		}

		protected HomeViewController RootController { 
			get {
				return (UIApplication.SharedApplication.Delegate as AppDelegate).RootController;
			} 
		}

		protected SidebarNavigation.SidebarController SidebarController { 
			get {
				return (UIApplication.SharedApplication.Delegate as AppDelegate).RootController.SidebarController;
			} 
		}

		// provide access to the sidebar controller to all inheriting controllers
		protected NavController NavController { 
			get {
				return (UIApplication.SharedApplication.Delegate as AppDelegate).RootController.NavController;
			} 
		}
	}
}


