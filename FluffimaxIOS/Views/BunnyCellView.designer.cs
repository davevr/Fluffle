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
    [Register ("BunnyCellView")]
    partial class BunnyCellView
    {
        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel BreedLabel { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIImageView BunnyImage { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIButton BuyBtn { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel EyeColorLabel { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel FurColorLabel { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel GenderLabel { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel PriceLabel { get; set; }

        void ReleaseDesignerOutlets ()
        {
            if (BreedLabel != null) {
                BreedLabel.Dispose ();
                BreedLabel = null;
            }

            if (BunnyImage != null) {
                BunnyImage.Dispose ();
                BunnyImage = null;
            }

            if (BuyBtn != null) {
                BuyBtn.Dispose ();
                BuyBtn = null;
            }

            if (EyeColorLabel != null) {
                EyeColorLabel.Dispose ();
                EyeColorLabel = null;
            }

            if (FurColorLabel != null) {
                FurColorLabel.Dispose ();
                FurColorLabel = null;
            }

            if (GenderLabel != null) {
                GenderLabel.Dispose ();
                GenderLabel = null;
            }

            if (PriceLabel != null) {
                PriceLabel.Dispose ();
                PriceLabel = null;
            }
        }
    }
}