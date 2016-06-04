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
	[Register ("BunnySizeCellView")]
	partial class BunnySizeCellView
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
		UILabel ProgressLabel { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UILabel SizeLabel { get; set; }

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
			if (ProgressLabel != null) {
				ProgressLabel.Dispose ();
				ProgressLabel = null;
			}
			if (SizeLabel != null) {
				SizeLabel.Dispose ();
				SizeLabel = null;
			}
		}
	}
}
