﻿using System;

using UIKit;

namespace Fluffimax.iOS
{
	public partial class BunnyCamViewController : UIViewController
	{
		public BunnyCamViewController () : base ("BunnyCamViewController", null)
		{
		}

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();
			// Perform any additional setup after loading the view, typically from a nib.
			this.Title = "Bunny_Cam".Localize();
			UIBarButtonItem menuBtn = new UIBarButtonItem(UIImage.FromBundle("menu-48"), UIBarButtonItemStyle.Plain, null);
			this.NavigationItem.SetLeftBarButtonItem(menuBtn, false);

			menuBtn.Clicked += (object sender, EventArgs e) =>
			{
				SidebarController.ToggleMenu();
			};
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


