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
	[Register ("PlayerShareCellView")]
	partial class PlayerShareCellView
	{
		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UILabel JoinDateLabel { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UIImageView PlayerImg { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UILabel PlayerName { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UILabel ShareCount { get; set; }

		void ReleaseDesignerOutlets ()
		{
			if (JoinDateLabel != null) {
				JoinDateLabel.Dispose ();
				JoinDateLabel = null;
			}
			if (PlayerImg != null) {
				PlayerImg.Dispose ();
				PlayerImg = null;
			}
			if (PlayerName != null) {
				PlayerName.Dispose ();
				PlayerName = null;
			}
			if (ShareCount != null) {
				ShareCount.Dispose ();
				ShareCount = null;
			}
		}
	}
}
