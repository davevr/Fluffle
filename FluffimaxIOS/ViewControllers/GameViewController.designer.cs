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
    [Register ("GameViewController")]
    partial class GameViewController
    {
        [Outlet]
        UIKit.UIView BunnyDetailView { get; set; }

        [Outlet]
        UIKit.UILabel BunnyNameLabel { get; set; }


        [Outlet]
        UIKit.UIButton BuyBunnyBtn { get; set; }


        [Outlet]
        UIKit.UIButton BuyCarrotsBtn { get; set; }


        [Outlet]
        UIKit.UILabel CarrotCount { get; set; }


        [Outlet]
        UIKit.UIImageView CarrotImg { get; set; }


        [Outlet]
        UIKit.NSLayoutConstraint CSCarrotHeight { get; set; }


        [Outlet]
        UIKit.NSLayoutConstraint CSCarrotWidth { get; set; }


        [Outlet]
        UIKit.NSLayoutConstraint CSCarrotX { get; set; }


        [Outlet]
        UIKit.NSLayoutConstraint CSCarrotY { get; set; }


        [Outlet]
        UIKit.UIButton FeedBunnyBtn { get; set; }


        [Outlet]
        UIKit.UIButton GiveBtn { get; set; }


        [Outlet]
        UIKit.UIImageView GrassField { get; set; }


        [Outlet]
        UIKit.UILabel ProgressCount { get; set; }


        [Outlet]
        UIKit.UIButton SellBunnyBtn { get; set; }


        [Outlet]
        UIKit.UILabel SizeCount { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel BunnyInfoLabel { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIButton CatchBtn { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIView PlayfieldView { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIProgressView ProgressIndicator { get; set; }

        void ReleaseDesignerOutlets ()
        {
            if (BunnyDetailView != null) {
                BunnyDetailView.Dispose ();
                BunnyDetailView = null;
            }

            if (BunnyInfoLabel != null) {
                BunnyInfoLabel.Dispose ();
                BunnyInfoLabel = null;
            }

            if (BunnyNameLabel != null) {
                BunnyNameLabel.Dispose ();
                BunnyNameLabel = null;
            }

            if (BuyBunnyBtn != null) {
                BuyBunnyBtn.Dispose ();
                BuyBunnyBtn = null;
            }

            if (BuyCarrotsBtn != null) {
                BuyCarrotsBtn.Dispose ();
                BuyCarrotsBtn = null;
            }

            if (CarrotImg != null) {
                CarrotImg.Dispose ();
                CarrotImg = null;
            }

            if (CatchBtn != null) {
                CatchBtn.Dispose ();
                CatchBtn = null;
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

            if (FeedBunnyBtn != null) {
                FeedBunnyBtn.Dispose ();
                FeedBunnyBtn = null;
            }

            if (GiveBtn != null) {
                GiveBtn.Dispose ();
                GiveBtn = null;
            }

            if (GrassField != null) {
                GrassField.Dispose ();
                GrassField = null;
            }

            if (PlayfieldView != null) {
                PlayfieldView.Dispose ();
                PlayfieldView = null;
            }

            if (ProgressIndicator != null) {
                ProgressIndicator.Dispose ();
                ProgressIndicator = null;
            }

            if (SellBunnyBtn != null) {
                SellBunnyBtn.Dispose ();
                SellBunnyBtn = null;
            }

            if (SizeCount != null) {
                SizeCount.Dispose ();
                SizeCount = null;
            }
        }
    }
}