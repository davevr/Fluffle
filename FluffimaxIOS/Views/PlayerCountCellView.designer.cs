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
    [Register ("PlayerCountCellView")]
    partial class PlayerCountCellView
    {
        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel BunnyCount { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel JoinedDateLabel { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIImageView PlayerImg { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel PlayerName { get; set; }

        void ReleaseDesignerOutlets ()
        {
            if (BunnyCount != null) {
                BunnyCount.Dispose ();
                BunnyCount = null;
            }

            if (JoinedDateLabel != null) {
                JoinedDateLabel.Dispose ();
                JoinedDateLabel = null;
            }

            if (PlayerImg != null) {
                PlayerImg.Dispose ();
                PlayerImg = null;
            }

            if (PlayerName != null) {
                PlayerName.Dispose ();
                PlayerName = null;
            }
        }
    }
}