using System;

using UIKit;
using Foundation;
using Fluffimax.Core;
using SDWebImage;


namespace Fluffimax.iOS
{
	public partial class ProfileViewController : UIViewController
	{
		public ProfileViewController () : base ("ProfileViewController", null)
		{
		}

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();
			// Perform any additional setup after loading the view, typically from a nib.
			SetImageBtn.TouchUpInside += (object sender, EventArgs e) => {
				MaybeChangeProfileImage ();
			};

			ChangeUsernameBtn.TouchUpInside += (object sender, EventArgs e) => {
				MaybeSaveChanges ();
			};

			ChangePasswordBtn.TouchUpInside += (object sender, EventArgs e) => {
				MaybeChangePassword ();
			};




		}

		private void MaybeChangeProfileImage() {

		}



		private void MaybeSaveChanges() {
			// change username?
			if (Game.CurrentPlayer.username != UsernameField.Text) {
				bool mustSetPassword = false;
				if (Game.CurrentPlayer.username == Game.CurrentPlayer.pwd) {
					// user has to set a password
					mustSetPassword = true;
				}
				// ok to just set the username
				Server.UpdateUsername (UsernameField.Text, (resultStr) => {
					if (string.IsNullOrEmpty (resultStr)) {
						// success
						InvokeOnMainThread (() => {
							Game.CurrentPlayer.username = UsernameField.Text;
							if (mustSetPassword)
								MaybeChangePassword(true);
							else {
								Game.SavePlayer(true);
								UpdateButtonStates ();
							}
						});

					} else {
						// something went wrong - show the message
						HomeViewController.ShowMessageBox ("Error", "Unable to change username.  Make sure your username is unique.  Try an email address for best results", "will do");
					}
				});
			}

			// change nickname?
			if (Game.CurrentPlayer.nickname != NicknameField.Text) {
				Server.UpdateNickname (NicknameField.Text, (resultStr) => {
					if (string.IsNullOrEmpty (resultStr)) {
						// success
						InvokeOnMainThread (() => {
							Game.CurrentPlayer.nickname = NicknameField.Text;
							Game.SavePlayer(true);
							UpdateButtonStates ();
						});
					} else {
						// something went wrong - show the message
						HomeViewController.ShowMessageBox ("Error", "Unable to change nickname.  Maybe try again later?", "umm.. ok...");
					}
				});

			}
		}
			

		private void MaybeChangePassword(bool forced = false) {
			
			string titleStr;

			if (forced)
				titleStr = "You must change your password when you first change your username.  Enter the new password now:";
			else
				titleStr = "Enter the new password now:";

			UIAlertView alert = new UIAlertView ();
			alert.Title = "Change password";
			alert.AddButton ("Change");
			if (!forced)
				alert.AddButton ("Cancel");
			alert.Message = titleStr;
			alert.AlertViewStyle = UIAlertViewStyle.PlainTextInput;
			alert.Clicked += (object s, UIButtonEventArgs ev) => {
				if (ev.ButtonIndex == 0) {
					string newPwd = alert.GetTextField (0).Text;
					Server.UpdatePassword(newPwd, (resultStr) => {
						if (string.IsNullOrEmpty (resultStr)) {
							// success
							InvokeOnMainThread (() => {
								Game.CurrentPlayer.pwd = newPwd;
								Game.SavePlayer(true);
								UpdateButtonStates ();
							});

						} else {
							// something went wrong - show the message
							HomeViewController.ShowMessageBox ("Error", "Unable to change username.  Make sure your username is unique.  Try an email address for best results", "will do");
						}
					});
				}
			};
			alert.Show ();
		}




		public override void ViewWillAppear (bool animated)
		{
			base.ViewWillAppear (animated);
			UpdateUIForUser ();
		}

		private void UpdateUIForUser() {
			// title
			string titleStr = "";
			if (string.IsNullOrEmpty (Game.CurrentPlayer.nickname)) {
				titleStr = "Profile for Unnamed Player";
			} else {
				titleStr = Game.CurrentPlayer.nickname + "'s Profile";
			}
			HeaderLabel.Text = titleStr;

			// profile image
			if (string.IsNullOrEmpty (Game.CurrentPlayer.userimage)) {
				UserProfileImage.Image = UIImage.FromBundle ("unknown_user");
			} else {
				UserProfileImage.SetImage (
					url: new NSUrl (Game.CurrentPlayer.userimage), 
					placeholder: UIImage.FromBundle ("unknown_user")
				);
			}

			// nickname
			NicknameField.Text = Game.CurrentPlayer.nickname;

			// username
			UsernameField.Text = Game.CurrentPlayer.username;

			if (Game.CurrentPlayer.username == Game.CurrentPlayer.pwd) {
				ChangePasswordBtn.Enabled = false;
			} else {
				ChangePasswordBtn.Enabled = true;
			}

			UpdateButtonStates ();
		}

		private void UpdateButtonStates() {
			bool changed = false;

			if (NicknameField.Text != Game.CurrentPlayer.nickname)
				changed = true;

			if (UsernameField.Text != Game.CurrentPlayer.username)
				changed = true;

			if (changed) {
				ChangeUsernameBtn.Enabled = true;
			} else {
				ChangeUsernameBtn.Enabled = false;
			}
		}

		public override void DidReceiveMemoryWarning ()
		{
			base.DidReceiveMemoryWarning ();
			// Release any cached data, images, etc that aren't in use.
		}

		protected HomeViewController RootController { 
			get {
				return (UIApplication.SharedApplication.Delegate as AppDelegate).RootController;
			} 
		}

		protected SidebarNavigation.SidebarController SidebarController { 
			get {
				return (UIApplication.SharedApplication.Delegate as AppDelegate).RootController.SidebarController;
			} 
		}

		// provide access to the sidebar controller to all inheriting controllers
		protected NavController NavController { 
			get {
				return (UIApplication.SharedApplication.Delegate as AppDelegate).RootController.NavController;
			} 
		}

		partial void nicknameChanged (UITextField sender)
		{
			UpdateButtonStates();
		}
	}
}


