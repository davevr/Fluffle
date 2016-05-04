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

namespace Fluffimax
{
	[Register ("GameViewController")]
	partial class GameViewController
	{
		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UIButton BuyBunnyBtn { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UIButton BuyCarrotsBtn { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UILabel CarrotCount { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UIImageView CarrotImg { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		NSLayoutConstraint CSCarrotHeight { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		NSLayoutConstraint CSCarrotWidth { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		NSLayoutConstraint CSCarrotX { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		NSLayoutConstraint CSCarrotY { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UILabel ProgressCount { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UILabel SizeCount { get; set; }

		void ReleaseDesignerOutlets ()
		{
			if (BuyBunnyBtn != null) {
				BuyBunnyBtn.Dispose ();
				BuyBunnyBtn = null;
			}
			if (BuyCarrotsBtn != null) {
				BuyCarrotsBtn.Dispose ();
				BuyCarrotsBtn = null;
			}
			if (CarrotCount != null) {
				CarrotCount.Dispose ();
				CarrotCount = null;
			}
			if (CarrotImg != null) {
				CarrotImg.Dispose ();
				CarrotImg = null;
			}
			if (CSCarrotHeight != null) {
				CSCarrotHeight.Dispose ();
				CSCarrotHeight = null;
			}
			if (CSCarrotWidth != null) {
				CSCarrotWidth.Dispose ();
				CSCarrotWidth = null;
			}
			if (CSCarrotX != null) {
				CSCarrotX.Dispose ();
				CSCarrotX = null;
			}
			if (CSCarrotY != null) {
				CSCarrotY.Dispose ();
				CSCarrotY = null;
			}
			if (ProgressCount != null) {
				ProgressCount.Dispose ();
				ProgressCount = null;
			}
			if (SizeCount != null) {
				SizeCount.Dispose ();
				SizeCount = null;
			}
		}
	}
}
