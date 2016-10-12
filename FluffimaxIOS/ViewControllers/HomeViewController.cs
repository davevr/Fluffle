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


		public HomeViewController () : base (null, null)
		{
		}

		public NavController NavController { get; set;}

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();

			RewardString = null;

			NavController = new NavController();



			View.BackgroundColor = UIColor.Green;


			//NavController.PushViewController (new InitialLoadViewController (), false);
			ResumeGame ();
		}

		private void FinishLoad() {
			InvokeOnMainThread (() => {
				SidebarController = new SidebarNavigation.SidebarController(this, NavController, new SideMenuController());
				SidebarController.MenuWidth = 220;
				SidebarController.ReopenOnRotate = false;
				SidebarController.MenuLocation = SidebarNavigation.SidebarController.MenuLocations.Left;
				NavController.PushViewController (new GameViewController() , true);
			});
		}



		public override void ViewWillAppear (bool animated)
		{
			base.ViewWillAppear (animated);
			NavController.NavigationBarHidden = false;
			/*
			if (!string.IsNullOrEmpty(RewardString))
				ShowReward();
*/

		}

		private void ShowReward() {

			UIAlertView alert = new UIAlertView ("Welcome_Back_Msg".Localize(), RewardString, null, "Welcome_Back_Ok_Btn".Localize());
			alert.Show ();

			RewardString = null;
		}

		private void HandleNetworkChange(object sender, EventArgs args) {
			if (Reachability.IsNetworkAvailable ()) {
				Console.WriteLine ("network is now available");
			} else {
				Console.WriteLine ("lost network connection");
			}
		}

		private void ResumeGame() {
			Reachability.ReachabilityChanged += HandleNetworkChange;
			Server.InitServer ();
			if (Reachability.IsNetworkAvailable () || Server.IsLocal) {
				Server.IsAlive ((isAlive) => {
					if (isAlive) {
						Server.IsOnline = true;
						SpriteManager.Initialize ();
						Game.InitBunnyStore ();
						Game.InitGrowthChart ();

						// load the player
						Game.LoadExistingPlayer ((curPlayer) => {
							if (curPlayer != null) {
								InvokeOnMainThread (() => {
									//StartBtn.SetTitle ("Resume", UIControlState.Normal);
									RewardString = Game.MaybeRewardPlayer ();
									FinishLoad ();
								});
							} else {
								// if no player, create one
								Game.InitGameForNewPlayer ((newPlayer) => {
									Game.SavePlayer (true);
									InvokeOnMainThread (() => {
										//StartBtn.SetTitle ("Start", UIControlState.Normal);
										FinishLoad ();
									});
								});
							}
						});
					} else {
						Server.IsOnline = false;
						ShowMessageBox ("Error_Title".Localize(), "No_Fluffle_Cloud_Msg".Localize(), "Connection_Err_Btn".Localize());
					}
				});
			} else {
				Server.IsOnline = false;
				ShowMessageBox ("Error_Title".Localize(), "No_Network_Msg".Localize(), "Connection_Err_Btn".Localize());

			}
		}

		public override void DidReceiveMemoryWarning ()
		{
			base.DidReceiveMemoryWarning ();
			// Release any cached data, images, etc that aren't in use.
		}

		public static void ShowMessageBox(string titleStr, string msgStr, string btnMsg) {
			(UIApplication.SharedApplication.Delegate as AppDelegate).RootController.InvokeOnMainThread (() => {
				UIAlertView alert = new UIAlertView();
				alert.Title = titleStr;
				alert.AddButton(btnMsg);
				alert.Message = msgStr;
				alert.AlertViewStyle = UIAlertViewStyle.Default;
				alert.Clicked += (object s, UIButtonEventArgs ev) =>
				{

				};

				alert.Show ();
			});
		}
		private static bool forceTutorials = true;
		public static bool skipTutorial = false;


		public static bool ShowTutorialStep(string keyName, string messageStrKey)
		{
			bool shown = false;
			if (!skipTutorial)
			{
				// todo - figure out iOS prefs
				//ISharedPreferences prefs = PreferenceManager.GetDefaultSharedPreferences(activity);
				bool didStep = false;// prefs.GetBoolean(keyName, false);

				if (forceTutorials || !didStep)
				{
					shown = true;
					(UIApplication.SharedApplication.Delegate as AppDelegate).RootController.InvokeOnMainThread(() =>
					{
						UIButton newbox = new UIButton(UIButtonType.System);
						newbox.SetTitle("skip_tutorials".Localize(), UIControlState.Normal);

						UIAlertView alert = new UIAlertView();
						alert.Title = "tutorial_title".Localize();
						alert.AddButton("ok_btn".Localize());
						alert.Message = messageStrKey.Localize();
						alert.AlertViewStyle = UIAlertViewStyle.Default;
						alert.AddSubview(newbox);
						alert.Clicked += (object s, UIButtonEventArgs ev) =>
						{
							// todo - set the new prefs
							/*
							var editor = prefs.Edit();
							editor.PutBoolean(keyName, true);
							if (newBox.Checked)
							{
								skipTutorial = true;
								editor.PutBoolean("skipTutorial", true);
							}
							editor.Apply();
							*/
						};

						alert.Show();

					});
				}
			}
			return shown;
		}


	}
}


