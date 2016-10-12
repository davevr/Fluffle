using Foundation;
using UIKit;
using HockeyApp;
using Flurry.Analytics;
using System.Collections.Generic;
using SidebarNavigation;
using System;

namespace Fluffimax.iOS
{
	// The UIApplicationDelegate for the application. This class is responsible for launching the
	// User Interface of the application, as well as listening (and optionally responding) to application events from iOS.
	[Register("AppDelegate")]
	public class AppDelegate : UIApplicationDelegate
	{
		// class-level declarations

		public override UIWindow Window
		{
			get;
			set;
		}

		public HomeViewController RootController
		{
			get;
			set;
		}

		public static bool IsMini {
			get
			{
				return UIScreen.MainScreen.Bounds.Height == 480;
			}
		}

		public override bool FinishedLaunching (UIApplication application, NSDictionary launchOptions)
		{
			FlurryAgent.StartSession("B9MSG4BV56C6NG3YGRKW");
			FlurryAgent.SetEventLoggingEnabled (true);

			var manager = BITHockeyManager.SharedHockeyManager;
			manager.Configure("ab076f11566a4a6c94f870ca7f143ef5");
			manager.DisableUpdateManager = false;
			manager.EnableStoreUpdateManager = true;
			manager.StartManager();

			// Override point for customization after application launch.
			// If not required for your application you can safely delete this method
			Window = new UIWindow(UIScreen.MainScreen.Bounds);

			/*
			// If you have defined a root view controller, set it here:
			NavController = new UINavigationController(new HomeViewController());
			*/

			RootController = new HomeViewController();
			Window.RootViewController = RootController ;

			Window.MakeKeyAndVisible();


			/*
			if (Flurry.Ads.FlurryAds.IsAdReady ("Fluffle Test Ad")) {
				Flurry.Ads.FlurryAds.DisplayAd ("Fluffle Test Ad", UITableViewHeaderFooterView, theViewController);

			} else {
				Flurry.Ads.FlurryAds.FetchAndDisplayAd ("space", UITableViewHeaderFooterView, theViewController, Flurry.Ads.AdSize.Fullscreen);
			}
*/
			return true;

		}


		public override void OnResignActivation (UIApplication application)
		{
			// Invoked when the application is about to move from active to inactive state.
			// This can occur for certain types of temporary interruptions (such as an incoming phone call or SMS message) 
			// or when the user quits the application and it begins the transition to the background state.
			// Games should use this method to pause the game.
		}

		public override void DidEnterBackground (UIApplication application)
		{
			// Use this method to release shared resources, save user data, invalidate timers and store the application state.
			// If your application supports background exection this method is called instead of WillTerminate when the user quits.
			Fluffimax.Core.Game.SavePlayer();
		}

		public override void WillEnterForeground (UIApplication application)
		{
			// Called as part of the transiton from background to active state.
			// Here you can undo many of the changes made on entering the background.
		}

		public override void OnActivated (UIApplication application)
		{
			// Restart any tasks that were paused (or not yet started) while the application was inactive. 
			// If the application was previously in the background, optionally refresh the user interface.
		}

		public override void WillTerminate (UIApplication application)
		{
			// Called when the application is about to terminate. Save data, if needed. See also DidEnterBackground.
		}
	}
}


