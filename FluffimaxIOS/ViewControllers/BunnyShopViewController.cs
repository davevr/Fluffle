using System;
using Foundation;
using UIKit;
using Fluffimax.Core;

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
		private BunnyShopTableSource dataSource;
		private Bunny pendingBunny = null;
		private BuyDelegate buyDelegate;


		public BunnyShopViewController () : base ("BunnyShopViewController", null)
		{
			buyDelegate = new BuyDelegate();
			buyDelegate.ShopView = this;
		}

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();
			// Perform any additional setup after loading the view, typically from a nib.
			BunnySaleList.RegisterNibForCellReuse (UINib.FromName (BunnyCellView.Key, NSBundle.MainBundle), BunnyCellView.Key);
			dataSource = new BunnyShopTableSource ();
			BunnySaleList.DataSource = dataSource;
			BunnySaleList.RowHeight = 96;
			dataSource.ShopView = this;
			UpdateCarrotCount ();
		}

		private void UpdateCarrotCount() {
			BeginInvokeOnMainThread(() => {
				CarrotCountLabel.Text = String.Format("You have {0} carrots", Game.CurrentPlayer.carrotCount);
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
		}

		public void MaybeBuyBunny(Bunny theBuns) {
			if (Game.CurrentPlayer.carrotCount >= theBuns.Price) {
				pendingBunny = theBuns;
				UIAlertView confirmView = new UIAlertView ("Confirm Purchase", String.Format ("Are you sure you want to buy this bunny for {0} carrots?", theBuns.Price),
					                          buyDelegate, "never mind", new string[] {"Buy!"});
				confirmView.Show ();
			} else {
				UIAlertView denyView = new UIAlertView ("Purchase Declined", "Sorry, you do not have enough carrots to buy this bunny", null, "Oh well");
				denyView.Show ();
			}
		}

		public void PurchaseBunny() {
			if (pendingBunny != null) {
				if (Game.CurrentPlayer.BuyBunny (pendingBunny)) {
					Game.BunnyStore.Remove (pendingBunny);
					UpdateCarrotCount ();
					UIAlertView goodNews = new UIAlertView ("Purchase Accepted", "Enjoy your new cute bunny!", null, "Happiness!");
					goodNews.Show ();
					//BunnySaleList.ReloadData ();
					NavController.PopViewController(true);
				} else {
					UIAlertView denyView = new UIAlertView ("Purchase Declined", "Sorry, the purchase did not go through", null, "Oh well");
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


