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
	[Register ("BunnySpreadCellView")]
	partial class BunnySpreadCellView
	{
		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UIImageView BunnyImg { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UILabel BunnyName { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UIImageView PlayerImg { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UILabel PlayerName { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UILabel SpreadLabel { get; set; }

		void ReleaseDesignerOutlets ()
		{
			if (BunnyImg != null) {
				BunnyImg.Dispose ();
				BunnyImg = null;
			}
			if (BunnyName != null) {
				BunnyName.Dispose ();
				BunnyName = null;
			}
			if (PlayerImg != null) {
				PlayerImg.Dispose ();
				PlayerImg = null;
			}
			if (PlayerName != null) {
				PlayerName.Dispose ();
				PlayerName = null;
			}
			if (SpreadLabel != null) {
				SpreadLabel.Dispose ();
				SpreadLabel = null;
			}
		}
	}
}
