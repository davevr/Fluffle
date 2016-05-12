using System;

using UIKit;
using Fluffimax.Core;

namespace Fluffimax.iOS
{
	public partial class CarrotShopViewController : UIViewController
	{
		public CarrotShopViewController () : base ("CarrotShopViewController", null)
		{
		}

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();
			// Perform any additional setup after loading the view, typically from a nib.

			BuyItem1Btn.TouchUpInside += (object sender, EventArgs e) => {
				BuyItem(1);
			};

			BuyItem2Btn.TouchUpInside += (object sender, EventArgs e) => {
				BuyItem(2);
			};

			BuyItem3Btn.TouchUpInside += (object sender, EventArgs e) => {
				BuyItem(3);
			};

			BuyItem4Btn.TouchUpInside += (object sender, EventArgs e) => {
				BuyItem(4);
			};

			BuyItem5Btn.TouchUpInside += (object sender, EventArgs e) => {
				BuyItem(5);
			};

			WatchAdBtn.TouchUpInside += (object sender, EventArgs e) => {
				BuyItem(-1);
			};

		}

		private void BuyItem(int whichItem) {
			int carrotCount = 0;

			switch (whichItem) {
			case 1:
				carrotCount = 100;
				break;
			case 2:
				carrotCount = 750;
				break;
			case 3:
				carrotCount = 2000;
				break;
			case 4:
				carrotCount = 20000;
				break;
			case 5:
				carrotCount = 100000;
				break;
			case -1:
				carrotCount = 10;
				break;
			}


			if (carrotCount > 0) {
				Game.CurrentPlayer.GiveCarrots (carrotCount);
				UpdateTextLabels ();
			}
		}

		private void UpdateTextLabels() {
			InvokeOnMainThread (() => {
				CurrentCarrotLabel.Text = String.Format("you have {0} carrots", Game.CurrentPlayer.carrotCount);
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
			UpdateTextLabels ();
		}

		protected UINavigationController NavController { 
			get {
				return (UIApplication.SharedApplication.Delegate as AppDelegate).NavController;
			} 
		}

	}
}


