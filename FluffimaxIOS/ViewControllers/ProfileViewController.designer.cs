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
		UIButton ChangePasswordBtn { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UIButton ChangeUsernameBtn { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UILabel HeaderLabel { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UITextField NicknameField { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UIButton SetImageBtn { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UITextField UsernameField { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UIImageView UserProfileImage { get; set; }

		[Action ("nicknameChanged:")]
		[GeneratedCode ("iOS Designer", "1.0")]
		partial void nicknameChanged (UITextField sender);

		void ReleaseDesignerOutlets ()
		{
			if (ChangePasswordBtn != null) {
				ChangePasswordBtn.Dispose ();
				ChangePasswordBtn = null;
			}
			if (ChangeUsernameBtn != null) {
				ChangeUsernameBtn.Dispose ();
				ChangeUsernameBtn = null;
			}
			if (HeaderLabel != null) {
				HeaderLabel.Dispose ();
				HeaderLabel = null;
			}
			if (NicknameField != null) {
				NicknameField.Dispose ();
				NicknameField = null;
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
