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
    [Register ("SideMenuController")]
    partial class SideMenuController
    {
        [Outlet]
        UIKit.UIImageView MenuImage { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIButton AboutBtn { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIButton BoardsBtn { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIButton CamBtn { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIButton GameBtn { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIButton ProfileBtn { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIButton ShopBtn { get; set; }

        void ReleaseDesignerOutlets ()
        {
            if (AboutBtn != null) {
                AboutBtn.Dispose ();
                AboutBtn = null;
            }

            if (BoardsBtn != null) {
                BoardsBtn.Dispose ();
                BoardsBtn = null;
            }

            if (CamBtn != null) {
                CamBtn.Dispose ();
                CamBtn = null;
            }

            if (GameBtn != null) {
                GameBtn.Dispose ();
                GameBtn = null;
            }

            if (MenuImage != null) {
                MenuImage.Dispose ();
                MenuImage = null;
            }

            if (ProfileBtn != null) {
                ProfileBtn.Dispose ();
                ProfileBtn = null;
            }

            if (ShopBtn != null) {
                ShopBtn.Dispose ();
                ShopBtn = null;
            }
        }
    }
}