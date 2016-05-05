using System;

using UIKit;

using Fluffimax.Core;

namespace Fluffimax.iOS
{
	public partial class HomeViewController : UIViewController
	{
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

			ResumeGame ();
		}

		public override void ViewWillAppear (bool animated)
		{
			base.ViewWillAppear (animated);
			NavController.NavigationBarHidden = true;
		}

		private void ResumeGame() {
			Game.InitBunnyStore();

			// load the player
			if (Game.LoadExistingPlayer ()) {
				StartBtn.SetTitle ("Resume", UIControlState.Normal);
			} else {
				// if no player, create one
				Game.InitGameForNewPlayer ();
				Console.WriteLine ("player has {0} bunnies", Game.CurrentPlayer.Bunnies.Count);
				Game.SavePlayer ();
				StartBtn.SetTitle ("Start", UIControlState.Normal);
			}
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


