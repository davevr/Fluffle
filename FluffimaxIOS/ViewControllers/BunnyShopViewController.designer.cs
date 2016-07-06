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
    [Register ("BunnyShopViewController")]
    partial class BunnyShopViewController
    {
        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UITableView BunnySaleList { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel CarrotCountLabel { get; set; }

        void ReleaseDesignerOutlets ()
        {
            if (BunnySaleList != null) {
                BunnySaleList.Dispose ();
                BunnySaleList = null;
            }

            if (CarrotCountLabel != null) {
                CarrotCountLabel.Dispose ();
                CarrotCountLabel = null;
            }
        }
    }
}