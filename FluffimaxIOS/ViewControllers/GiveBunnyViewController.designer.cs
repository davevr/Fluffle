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
    [Register ("GiveBunnyViewController")]
    partial class GiveBunnyViewController
    {
        [Outlet]
        UILabel BunnyInfoLabel { get; set; }


        [Outlet]
        UILabel BunnyNameLabel { get; set; }


        [Outlet]
        UIButton DoneBtn { get; set; }


        [Outlet]
        UILabel TitleLabel { get; set; }


        [Outlet]
        UIImageView TossImageView { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIImageView BunnyImage { get; set; }

        void ReleaseDesignerOutlets ()
        {
            if (BunnyImage != null) {
                BunnyImage.Dispose ();
                BunnyImage = null;
            }

            if (BunnyInfoLabel != null) {
                BunnyInfoLabel.Dispose ();
                BunnyInfoLabel = null;
            }

            if (BunnyNameLabel != null) {
                BunnyNameLabel.Dispose ();
                BunnyNameLabel = null;
            }

            if (DoneBtn != null) {
                DoneBtn.Dispose ();
                DoneBtn = null;
            }

            if (TitleLabel != null) {
                TitleLabel.Dispose ();
                TitleLabel = null;
            }

            if (TossImageView != null) {
                TossImageView.Dispose ();
                TossImageView = null;
            }
        }
    }
}