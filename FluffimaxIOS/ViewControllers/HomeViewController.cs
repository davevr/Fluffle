using System;

using UIKit;

using Fluffimax.Core;

namespace Fluffimax.iOS
{
	public partial class HomeViewController : UIViewController
	{
		private string RewardString = null;

		public HomeViewController () : base ("HomeViewController", null)
		{
		}

		public override void ViewDidLoad ()
		{
			NavController.NavigationBarHidden = true;
			base.ViewDidLoad ();
			// Perform any additional setup after loading the view, typically from a nib.
			StartBtn.TouchUpInside += (object sender, EventArgs e) => {
				NavController.PushViewController(new GameViewController(), true);
			};

			AboutBtn.TouchUpInside += (object sender, EventArgs e) => {
				NavController.PushViewController(new AboutViewController(), true);
			};

			BunnyCamBtn.TouchUpInside += (object sender, EventArgs e) => {
				NavController.PushViewController(new BunnyCamViewController(), true);
			};

			MoreBunnyBtn.TouchUpInside += (object sender, EventArgs e) => {
				NavController.PushViewController(new CarrotShopViewController(), true);
			};
			RewardString = null;
			ResumeGame ();
		}

		public override void ViewWillAppear (bool animated)
		{
			base.ViewWillAppear (animated);
			NavController.NavigationBarHidden = true;
			if (!string.IsNullOrEmpty(RewardString))
				ShowReward();
		}

		private void ShowReward() {

			UIAlertView alert = new UIAlertView ("Welcome Back", RewardString, null, "Great!");
			alert.Show ();

			RewardString = null;
		}

		private void ResumeGame() {
			Server.InitServer();
			Game.InitBunnyStore();

			// load the player
			Game.LoadExistingPlayer ((curPlayer) => {
				if (curPlayer != null) {
					StartBtn.SetTitle ("Resume", UIControlState.Normal);
					RewardString = Game.MaybeRewardPlayer ();
				} else {
					// if no player, create one
					Game.InitGameForNewPlayer ((newPlayer) => {
						Game.SavePlayer (true);
						InvokeOnMainThread (() => {
							StartBtn.SetTitle ("Start", UIControlState.Normal);
						});
					});
				}
			});
		}

		public override void DidReceiveMemoryWarning ()
		{
			base.DidReceiveMemoryWarning ();
			// Release any cached data, images, etc that aren't in use.
		}

		protected UINavigationController NavController { 
			get {
				return (UIApplication.SharedApplication.Delegate as AppDelegate).NavController;
			} 
		}
	}
}


