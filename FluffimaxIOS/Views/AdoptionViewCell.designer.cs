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
    [Register ("AdoptionViewCell")]
    partial class AdoptionViewCell
    {
        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIImageView BunnyImage { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel BunnyInfo { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel BunnyName { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel BunnyPrice { get; set; }

        void ReleaseDesignerOutlets ()
        {
            if (BunnyImage != null) {
                BunnyImage.Dispose ();
                BunnyImage = null;
            }

            if (BunnyInfo != null) {
                BunnyInfo.Dispose ();
                BunnyInfo = null;
            }

            if (BunnyName != null) {
                BunnyName.Dispose ();
                BunnyName = null;
            }

            if (BunnyPrice != null) {
                BunnyPrice.Dispose ();
                BunnyPrice = null;
            }
        }
    }
}