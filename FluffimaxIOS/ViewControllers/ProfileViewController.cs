using System;

using UIKit;
using Foundation;
using Fluffimax.Core;
using SDWebImage;


namespace Fluffimax.iOS
{
	public partial class ProfileViewController : UIViewController
	{

		string chooseFromText = "New_Image_Msg".Localize();
		string cancelText = "cancel_btn".Localize();
		string fromCameraText = "From_Camera_Msg".Localize();
		string fromGalleryText = "From_Gallery_Msg".Localize();
		string deleteCurrentPhotoText = "Delete_Current_Msg".Localize();
		private UIActivityIndicatorView progressIndicator;

		public ProfileViewController () : base ("ProfileViewController", null)
		{
			NSNotificationCenter.DefaultCenter.AddObserver(UIKeyboard.WillHideNotification, OnKeyboardNotification);
			NSNotificationCenter.DefaultCenter.AddObserver(UIKeyboard.WillShowNotification, OnKeyboardNotification);
		}

		private void HideKeyboard()
		{
			var activeView = KeyboardGetActiveView();
			if (activeView != null)
				activeView.ResignFirstResponder();
		}



		private void OnKeyboardNotification(NSNotification notification)
		{
			if (IsViewLoaded)
			{

				//Check if the keyboard is becoming visible
				bool visible = notification.Name == UIKeyboard.WillShowNotification;

				//Start an animation, using values from the keyboard
				UIView.BeginAnimations("AnimateForKeyboard");
				UIView.SetAnimationBeginsFromCurrentState(true);
				UIView.SetAnimationDuration(UIKeyboard.AnimationDurationFromNotification(notification));
				UIView.SetAnimationCurve((UIViewAnimationCurve)UIKeyboard.AnimationCurveFromNotification(notification));

				//Pass the notification, calculating keyboard height, etc.
				bool landscape = InterfaceOrientation == UIInterfaceOrientation.LandscapeLeft || InterfaceOrientation == UIInterfaceOrientation.LandscapeRight;
				if (visible)
				{
					var keyboardFrame = UIKeyboard.FrameEndFromNotification(notification);

					OnKeyboardChanged(visible, landscape ? keyboardFrame.Width : keyboardFrame.Height);
				}
				else {
					var keyboardFrame = UIKeyboard.FrameBeginFromNotification(notification);

					OnKeyboardChanged(visible, landscape ? keyboardFrame.Width : keyboardFrame.Height);
				}

				//Commit the animation
				UIView.CommitAnimations();
			}
		}

		protected virtual void OnKeyboardChanged(bool visible, nfloat keyboardHeight)
		{
			if (visible)
			{
				ScrollViewBtm.Constant = keyboardHeight;
			}
			else {
				ScrollViewBtm.Constant = 0;
			}
		}


		protected UIView KeyboardGetActiveView()
		{
			return FindFirstResponder(this.View);
		}

		private UIView FindFirstResponder(UIView view)
		{
			if (view.IsFirstResponder)
			{
				return view;
			}
			foreach (UIView subView in view.Subviews)
			{
				var firstResponder = FindFirstResponder(subView);
				if (firstResponder != null)
					return firstResponder;
			}
			return null;
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

			LoginBtn.TouchUpInside += (sender, e) =>
			{
				MaybeLogin();
			};

			this.Title = "Profile_Title".Localize();

			if (AppDelegate.IsMini)
			{
				ProfileImageWidth.Constant = 160;
				ProfileImageHeight.Constant = 160;
				BunnyTop.Constant = 24;
				View.LayoutIfNeeded();
			}

			UIBarButtonItem menuBtn = new UIBarButtonItem(UIImage.FromBundle("menu-48"), UIBarButtonItemStyle.Plain, null);
			this.NavigationItem.SetLeftBarButtonItem(menuBtn, false);

			menuBtn.Clicked += (object sender, EventArgs e) =>
			{
				SidebarController.ToggleMenu();
			};



			NicknameField.ShouldReturn = delegate
			{
				HideKeyboard();
				return false;
			};

			UsernameField.ShouldReturn = delegate
			{
				HideKeyboard();
				return false;
			};



			UITapGestureRecognizer tapScrollHandler = new UITapGestureRecognizer();
			tapScrollHandler.NumberOfTapsRequired = 1;
			tapScrollHandler.AddTarget(() =>
			{
				HideKeyboard();
			});
			MainScroll.AddGestureRecognizer(tapScrollHandler);

			if (!string.IsNullOrEmpty(Game.CurrentPlayer.userimage))
			{
				UserProfileImage.SetImage(new NSUrl(Game.CurrentPlayer.userimage));
			}


		}

		private void MaybeChangeProfileImage() {
			UIActionSheet sheet;
			if (!String.IsNullOrEmpty(Game.CurrentPlayer.userimage))
				sheet = new UIActionSheet(chooseFromText, null, cancelText, null, new string[] {
					fromCameraText,
					fromGalleryText,
					deleteCurrentPhotoText
				});
			else
				sheet = new UIActionSheet(chooseFromText, null, cancelText, null, new string[] {
					fromCameraText, fromGalleryText
				});
			sheet.ShowInView(View);
			sheet.Clicked += FileChooseActionSheetClicked;
		}

		private void FileChooseActionSheetClicked(object sender, UIButtonEventArgs eventArgs)
		{
			var filePicker = new BGImagePickerController();
			filePicker.FinishedPickingMedia += FileChooseFinished;
			filePicker.Canceled += (sender1, eventArguments) =>
			{

				filePicker.DismissViewController(true,
					() => UIApplication.SharedApplication.SetStatusBarHidden(true, UIStatusBarAnimation.Slide));
			};
			if (eventArgs.ButtonIndex == 1)
			{
				filePicker.SourceType = UIImagePickerControllerSourceType.PhotoLibrary;
				PresentViewController(filePicker, true, null);
			}
			else if (eventArgs.ButtonIndex == 0)
			{
				filePicker.SourceType = UIImagePickerControllerSourceType.Camera;
				PresentViewController(filePicker, true,
					() => UIApplication.SharedApplication.SetStatusBarHidden(true, UIStatusBarAnimation.Slide));
			}
			else if (eventArgs.ButtonIndex == 2)
			{
				DeleteUserImage();

			}
		}


		private void DeleteUserImage()
		{
			Server.RecordUserImage("");
			Game.CurrentPlayer.userimage = null;
			UserProfileImage.Image = UIImage.FromBundle("bunny");
		}

		private void FileChooseFinished(object sender, UIImagePickerMediaPickedEventArgs eventArgs)
		{
			UIImage image = UIImageHelper.ScaleAndRotateImage(eventArgs.OriginalImage);
			DateTime now = DateTime.Now;
			string imageName = String.Format("{0}_{1}.jpg", now.ToLongDateString(), Game.CurrentPlayer.username);

			Server.UploadImage(image.AsJPEG().AsStream(),
													  imageName,
													  ProfileImageUploadCallback);

			progressIndicator = new UIActivityIndicatorView(UIActivityIndicatorViewStyle.Gray);
			progressIndicator.TranslatesAutoresizingMaskIntoConstraints = false;
			var constraintWidth = NSLayoutConstraint.Create(progressIndicator, NSLayoutAttribute.Width, NSLayoutRelation.Equal, null, NSLayoutAttribute.NoAttribute, 1, 40);
			var constraintHeight = NSLayoutConstraint.Create(progressIndicator, NSLayoutAttribute.Height, NSLayoutRelation.Equal, null, NSLayoutAttribute.NoAttribute, 1, 40);
			progressIndicator.AddConstraints(new NSLayoutConstraint[] { constraintHeight, constraintWidth });
			progressIndicator.HidesWhenStopped = true;
			UserProfileImage.Hidden = true;
			View.AddSubview(progressIndicator);
			View.BringSubviewToFront(progressIndicator);
			var constraintX = NSLayoutConstraint.Create(progressIndicator, NSLayoutAttribute.CenterX, NSLayoutRelation.Equal, UserProfileImage, NSLayoutAttribute.CenterX, 1, 0);
			var constraintY = NSLayoutConstraint.Create(progressIndicator, NSLayoutAttribute.CenterY, NSLayoutRelation.Equal, UserProfileImage, NSLayoutAttribute.CenterY, 1, 0);
			View.AddConstraints(new NSLayoutConstraint[] { constraintX, constraintY });
			progressIndicator.StartAnimating();

			((BGImagePickerController)sender).DismissViewController(true,
				() => UIApplication.SharedApplication.SetStatusBarHidden(true, UIStatusBarAnimation.Slide));

		}

		private void ProfileImageUploadCallback(string result)
		{
			if (!String.IsNullOrEmpty(result))
			{
				InvokeOnMainThread(() =>
				{
					progressIndicator.StopAnimating();
					UserProfileImage.Hidden = false;
					NSUrl theURL = new NSUrl(result);
					
					UserProfileImage.SetImage(theURL);
					Game.CurrentPlayer.userimage = result;
					Server.RecordUserImage(result);
				});
			}
			else
			{
				InvokeOnMainThread(() =>
					{
						progressIndicator.StopAnimating();
						UserProfileImage.Hidden = false;
						DeleteUserImage();
					});
			}
			Console.WriteLine(result);
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
						HomeViewController.ShowMessageBox ("Error_Title".Localize(), "Username_Msg".Localize(), "username_Btn".Localize());
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
						HomeViewController.ShowMessageBox ("Error_Title".Localize(), "Nickname_Msg".Localize(), "Nickname_Btn".Localize());
					}
				});

			}

		}
			

		private void MaybeChangePassword(bool forced = false) {
			
			string titleStr;

			if (forced)
				titleStr = "Must_Change_Pwd_Msg".Localize();
			else
				titleStr = "Change_Pwd_Msg".Localize();

			UIAlertView alert = new UIAlertView ();
			alert.Title = "Change_Pwd_Title".Localize();
			alert.AddButton ("Change_Btn_Title".Localize());
			if (!forced)
				alert.AddButton ("cancel_btn".Localize());
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
							HomeViewController.ShowMessageBox ("Error".Localize(), "Change_Username_Err_Msg".Localize(), "Change_Username_Err_Btn".Localize());
						}
					});
				}
			};
			alert.Show ();

		}

		public override void ViewWillAppear (bool animated)
		{
			base.ViewWillAppear (animated);
			NavController.NavigationBarHidden = false;
			UpdateUIForUser ();

		}

		private void UpdateUIForUser() {
			// title
			string titleStr = "";
			if (string.IsNullOrEmpty (Game.CurrentPlayer.nickname)) {
				titleStr = "Profile_Title_Unknown".Localize();
			} else {
				titleStr = string.Format("Profile_Title".Localize(), Game.CurrentPlayer.nickname);
			}
			this.Title = titleStr;

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
			HomeViewController.ShowTutorialStep("profile_tutorial", "profile_tutorial".Localize());
		}

		private void MaybeLogin()
		{
			UITextField userNameFld = null;
			UITextField passWordFld = null;
			UIAlertController loginPrompt = UIAlertController.Create("Login_Title".Localize(), "Login_Prompt".Localize(), UIAlertControllerStyle.Alert);

			loginPrompt.AddTextField(theField =>
			{
				userNameFld = theField;
			});

			loginPrompt.AddTextField(theField =>
			{
				passWordFld = theField;
				theField.SecureTextEntry = true;
			});

			loginPrompt.AddAction(UIAlertAction.Create("cancel_btn".Localize(), UIAlertActionStyle.Cancel, null));
			loginPrompt.AddAction(UIAlertAction.Create("Login_Btn".Localize(), UIAlertActionStyle.Destructive, (obj) =>
			{
					// to do - handle signin.
				string username = userNameFld.Text;
				string pwd = passWordFld.Text;

				Server.Login(username, pwd, (thePlayer) =>
				{
					if (thePlayer != null)
					{
						// log in with the new player
						Game.CurrentPlayer = thePlayer;
						Game.SavePlayer(true);
						Game.NewPlayerLoaded = true;
						InvokeOnMainThread(() =>
						{
							UpdateUIForUser();
						});
					}
					else {
						// error - probably bad password
						HomeViewController.ShowMessageBox("Login_Failure_Title".Localize(), "Login_Failure_msg".Localize(), "Login_Failure_btn".Localize());
					}
				});

			}));

			PresentViewController(loginPrompt, true, () =>
			{

			});
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

	[Register("BImagePickerController")]
	public class BGImagePickerController : UIImagePickerController
	{

		public BGImagePickerController() : base()
		{
			NavigationBar.TitleTextAttributes = new UIStringAttributes { ForegroundColor = UIColor.White };
		}
	}
}


