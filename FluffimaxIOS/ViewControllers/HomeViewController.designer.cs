// WARNING
//
// This file has been generated automatically by Xamarin Studio from the outlets and
// actions declared in your storyboard file.
// Manual changes to this file will not be maintained.
//
using Foundation;
using System;
using System.CodeDom.Compiler;
using UIKit;

namespace Fluffimax.iOS
{
	[Register ("HomeViewController")]
	partial class HomeViewController
	{
		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UIButton AboutBtn { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UIButton BunnyCamBtn { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UIButton MoreBunnyBtn { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UIButton StartBtn { get; set; }

		void ReleaseDesignerOutlets ()
		{
			if (AboutBtn != null) {
				AboutBtn.Dispose ();
				AboutBtn = null;
			}
			if (BunnyCamBtn != null) {
				BunnyCamBtn.Dispose ();
				BunnyCamBtn = null;
			}
			if (MoreBunnyBtn != null) {
				MoreBunnyBtn.Dispose ();
				MoreBunnyBtn = null;
			}
			if (StartBtn != null) {
				StartBtn.Dispose ();
				StartBtn = null;
			}
		}
	}
}
