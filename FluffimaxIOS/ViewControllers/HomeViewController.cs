using System;

using UIKit;
using System.Collections.Generic;

using Fluffimax.Core;

namespace Fluffimax.iOS
{
	public partial class HomeViewController : UIViewController
	{
		private string RewardString = null;
		public SidebarNavigation.SidebarController SidebarController { get; private set; }

		private GameViewController _gameVC = null;
		private BunnyCamViewController _bunnyVC = null;
		private CarrotShopViewController _carrotVC = null;
		private AboutViewController _aboutVC = null;
		private ProfileViewController _profileVC = null;
		private LeaderboardViewController _boardsVC = null;

		public HomeViewController () : base (null, null)
		{
		}

		public NavController NavController { get; set;}

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();

			RewardString = null;

			NavController = new NavController();




			SidebarController = new SidebarNavigation.SidebarController(this, NavController, new SideMenuController());
			SidebarController.MenuWidth = 220;
			SidebarController.ReopenOnRotate = false;
			SidebarController.MenuLocation = SidebarNavigation.SidebarController.MenuLocations.Left;

			//NavController.PushViewController (new InitialLoadViewController (), false);
			ResumeGame ();
		}

		private void FinishLoad() {
			InvokeOnMainThread (() => {
				NavController.PushViewController (new GameViewController() , true);
			});
		}



		public override void ViewWillAppear (bool animated)
		{
			base.ViewWillAppear (animated);
			NavController.NavigationBarHidden = false;
			if (!string.IsNullOrEmpty(RewardString))
				ShowReward();

		}

		private void ShowReward() {

			UIAlertView alert = new UIAlertView ("Welcome Back", RewardString, null, "Great!");
			alert.Show ();

			RewardString = null;
		}

		private void ResumeGame() {
			SpriteManager.Initialize();
			Server.InitServer();
			Game.InitBunnyStore();
			Game.InitGrowthChart ();

			// load the player
			Game.LoadExistingPlayer ((curPlayer) => {
				if (curPlayer != null) {
					InvokeOnMainThread (() => {
						//StartBtn.SetTitle ("Resume", UIControlState.Normal);
						RewardString = Game.MaybeRewardPlayer ();
						FinishLoad();
					});
				} else {
					// if no player, create one
					Game.InitGameForNewPlayer ((newPlayer) => {
						Game.SavePlayer (true);
						InvokeOnMainThread (() => {
							//StartBtn.SetTitle ("Start", UIControlState.Normal);
							FinishLoad();
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


	}
}


