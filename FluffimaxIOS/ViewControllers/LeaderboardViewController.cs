using System;

using UIKit;

namespace Fluffimax.iOS
{
	public partial class LeaderboardViewController : UITabBarController
	{
		UIViewController bunnyCountVC, playerShareVC, bunnySizeVC, bunnySpreadVC;
		public LeaderboardViewController () : base (null, null)
		{
			bunnyCountVC = new LBPlayerBunnyCountVC();
			bunnyCountVC.TabBarItem = new UITabBarItem ("most bunnies", UIImage.FromBundle ("about-48"), 0);

			playerShareVC = new LBPlayerShareVC();
			playerShareVC.TabBarItem = new UITabBarItem ("most shares", UIImage.FromBundle ("about-48"), 1);

			bunnySizeVC = new LBBunnySizeVC();
			bunnySizeVC.TabBarItem = new UITabBarItem ("biggest buns", UIImage.FromBundle ("about-48"), 2);

			bunnySpreadVC = new LBBunnySpreadVC();
			bunnySpreadVC.TabBarItem = new UITabBarItem ("spread", UIImage.FromBundle ("about-48"), 3);


			var tabs = new UIViewController[] {
				bunnyCountVC, playerShareVC, bunnySizeVC, bunnySpreadVC
			};

			ViewControllers = tabs;
			this.Title = "Fluffle Leaderboards";
			UIBarButtonItem menuBtn = new UIBarButtonItem (UIImage.FromBundle ("menu-48"), UIBarButtonItemStyle.Plain, null);
			this.NavigationItem.SetLeftBarButtonItem (menuBtn, false);

			menuBtn.Clicked += (object sender, EventArgs e) => 
			{
				SidebarController.ToggleMenu();
			};

		}

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();
			// Perform any additional setup after loading the view, typically from a nib.
		}

		public override void ViewWillAppear (bool animated)
		{
			base.ViewWillAppear (animated);
			NavController.NavigationBarHidden = false;
		}

		public override void DidReceiveMemoryWarning ()
		{
			base.DidReceiveMemoryWarning ();
			// Release any cached data, images, etc that aren't in use.
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


