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
    [Register ("ProfileViewController")]
    partial class ProfileViewController
    {
        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.NSLayoutConstraint BunnyTop { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIButton ChangePasswordBtn { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIButton ChangeUsernameBtn { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIScrollView MainScroll { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UITextField NicknameField { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.NSLayoutConstraint ProfileImageHeight { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.NSLayoutConstraint ProfileImageWidth { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.NSLayoutConstraint ScrollViewBtm { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIButton SetImageBtn { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UITextField UsernameField { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIImageView UserProfileImage { get; set; }

        [Action ("nicknameChanged:")]
        [GeneratedCode ("iOS Designer", "1.0")]
        partial void nicknameChanged (UIKit.UITextField sender);

        void ReleaseDesignerOutlets ()
        {
            if (BunnyTop != null) {
                BunnyTop.Dispose ();
                BunnyTop = null;
            }

            if (ChangePasswordBtn != null) {
                ChangePasswordBtn.Dispose ();
                ChangePasswordBtn = null;
            }

            if (ChangeUsernameBtn != null) {
                ChangeUsernameBtn.Dispose ();
                ChangeUsernameBtn = null;
            }

            if (MainScroll != null) {
                MainScroll.Dispose ();
                MainScroll = null;
            }

            if (NicknameField != null) {
                NicknameField.Dispose ();
                NicknameField = null;
            }

            if (ProfileImageHeight != null) {
                ProfileImageHeight.Dispose ();
                ProfileImageHeight = null;
            }

            if (ProfileImageWidth != null) {
                ProfileImageWidth.Dispose ();
                ProfileImageWidth = null;
            }

            if (ScrollViewBtm != null) {
                ScrollViewBtm.Dispose ();
                ScrollViewBtm = null;
            }

            if (SetImageBtn != null) {
                SetImageBtn.Dispose ();
                SetImageBtn = null;
            }

            if (UsernameField != null) {
                UsernameField.Dispose ();
                UsernameField = null;
            }

            if (UserProfileImage != null) {
                UserProfileImage.Dispose ();
                UserProfileImage = null;
            }
        }
    }
}