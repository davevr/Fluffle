using System;

using UIKit;
using Foundation;
using System.Collections.Generic;
using CoreGraphics;
using CoreAnimation;

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

			var prefs = NSUserDefaults.StandardUserDefaults;
			skipTutorial = prefs.BoolForKey("skipTutorials");

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

		private static bool forceTutorials = false;
		public static bool skipTutorial = false;


		public static bool ShowTutorialStep(string keyName, string messageStrKey)
		{
			bool shown = false;
			if (!skipTutorial)
			{
				var prefs = NSUserDefaults.StandardUserDefaults;

				bool didStep = false;

				if (prefs != null)
					didStep = prefs.BoolForKey(keyName);

				if (forceTutorials || !didStep)
				{
					shown = true;
					(UIApplication.SharedApplication.Delegate as AppDelegate).RootController.InvokeOnMainThread(() =>
					{
						UISwitch newbox = new UISwitch(new CGRect(10, 0, 40, 10));
						newbox.Transform = CGAffineTransform.MakeScale(0.75f, 0.75f);
						UILabel newLabel = new UILabel(new CGRect(65, 5, 200, 20));
						newbox.On = false;
						newLabel.Text = "skip_tutorials".Localize();
						newLabel.Font = newLabel.Font.WithSize(12);

						UIView newView = new UIView(new CGRect(0, 0, 300, 40));
						newView.AddSubview(newbox);
						newView.AddSubview(newLabel);

						UIAlertView alert = new UIAlertView();
						alert.Title = "tutorial_title".Localize();
						alert.AddButton("ok_btn".Localize());
						alert.Message = messageStrKey.Localize();
						alert.AlertViewStyle = UIAlertViewStyle.Default;
						alert.SetValueForKey(newView, new NSString("accessoryView"));
						alert.Clicked += (object s, UIButtonEventArgs ev) =>
						{
							prefs.SetBool(true, keyName);
							if (newbox.On)
							{
								skipTutorial = true;
								prefs.SetBool(true, "skipTutorial");
							}

							prefs.Synchronize();
						};

						alert.Show();

					});
				}
			}
			return shown;
		}


	}
}


