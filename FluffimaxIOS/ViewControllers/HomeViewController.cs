using System;

using UIKit;

using Fluffimax.Core;

namespace Fluffimax
{
	public partial class HomeViewController : UIViewController
	{
		public HomeViewController () : base ("HomeViewController", null)
		{
		}

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();
			// Perform any additional setup after loading the view, typically from a nib.
			StartBtn.TouchUpInside += (object sender, EventArgs e) => {
				NavController.PushViewController(new GameViewController(), true);
			};

			ResumeGame ();
		}

		private void ResumeGame() {
			Game.InitBunnyStore();

			// load the player
			if (!Game.LoadExistingPlayer ()) {
				// if no player, create one
				Game.InitGameForNewPlayer ();
				Game.SavePlayer ();
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


