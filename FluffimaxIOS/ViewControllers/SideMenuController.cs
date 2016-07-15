using System;

using UIKit;

namespace Fluffimax.iOS
{
	public partial class SideMenuController : UIViewController
	{
		public SideMenuController () : base ("SideMenuController", null)
		{
		}

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();
			// Perform any additional setup after loading the view, typically from a nib.

			GameBtn.TouchUpInside += (sender, e) => {
				NavController.PopToRootViewController(false);
				SidebarController.CloseMenu();
			};


			ProfileBtn.TouchUpInside += (sender, e) => {
				NavController.PushViewController(new ProfileViewController(), false);
				SidebarController.CloseMenu();
			};

			BoardsBtn.TouchUpInside += (sender, e) => {
				NavController.PushViewController(new LeaderboardViewController(), false);
				SidebarController.CloseMenu();
			};

			CamBtn.TouchUpInside += (sender, e) => {
				NavController.PushViewController(new BunnyCamViewController(), false);
				SidebarController.CloseMenu();
			};

			AboutBtn.TouchUpInside += (sender, e) => {
				NavController.PushViewController(new AboutViewController(), false);
				SidebarController.CloseMenu();
			};
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


