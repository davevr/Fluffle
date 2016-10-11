
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.Provider;
using File = Java.IO.File;
using Uri = Android.Net.Uri;
using Fluffimax.Core;
using Android.Graphics;

namespace Fluffle.AndroidApp
{
	[Activity(Label = "ProfileFragment")]
	public class ProfileFragment : Android.Support.V4.App.Fragment
	{
		public MainActivity MainPage { get; set; }
        private ImageView userImage;
        private Button setImageBtn;
        private EditText nicknameField;
        private EditText usernameField;
        private Button saveChangesBtn;
        private Button changePwdBtn;
        private Button signinBtn;
        private AlertDialog progressDlg;

		public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
		{
			// Use this to return your custom view for this Fragment
			// return inflater.Inflate(Resource.Layout.YourFragment, container, false);

			base.OnCreateView(inflater, container, savedInstanceState);

            var view = inflater.Inflate(Resource.Layout.ProfileLayout, container, false);

            userImage = view.FindViewById<ImageView>(Resource.Id.userImage);
            setImageBtn = view.FindViewById<Button>(Resource.Id.changeImageBtn);
            nicknameField = view.FindViewById<EditText>(Resource.Id.nicknameField);
            usernameField = view.FindViewById<EditText>(Resource.Id.usernameText);
            saveChangesBtn = view.FindViewById<Button>(Resource.Id.saveChangesBtn);
            changePwdBtn = view.FindViewById<Button>(Resource.Id.changePwdBtn);
            signinBtn = view.FindViewById<Button>(Resource.Id.signInBtn);

            nicknameField.AfterTextChanged += TextFieldChanged;
            usernameField.AfterTextChanged += TextFieldChanged;

            setImageBtn.Click += SetImageBtn_Click;
            saveChangesBtn.Click += SaveChangesBtn_Click;
            changePwdBtn.Click += ChangePwdBtn_Click;
            signinBtn.Click += SigninBtn_Click;
            UpdateForUser();

			return view;
		}

        private void TextFieldChanged(object sender, Android.Text.AfterTextChangedEventArgs e)
        {
            UpdateButtonStates();
        }

        private void SigninBtn_Click(object sender, EventArgs e)
        {
            var newView = new LinearLayout(this.Context);
            newView.Orientation = Orientation.Vertical;
            var userNameFld = new EditText(this.Context);
            newView.AddView(userNameFld);
            userNameFld.Hint = "username";
            var passWordFld = new EditText(this.Context);
            newView.AddView(passWordFld);
            passWordFld.Hint = "password";
            passWordFld.InputType = Android.Text.InputTypes.TextVariationPassword;

            var dialog = new AlertDialog.Builder(this.Context)
                .SetTitle(Resource.String.Login_Title.Localize())
                .SetMessage(Resource.String.Login_Prompt.Localize())
                .SetView(newView)
                .SetPositiveButton(Resource.String.Login_Btn.Localize(), (s2, e2) =>
                {
                    // to do - handle signin.
                    string username = userNameFld.Text;
                    string pwd = passWordFld.Text;

                    Server.Login(username, pwd, (thePlayer) =>
                    {
                        if (thePlayer != null)
                        {
                            // log in with the new player
                            thePlayer.pwd = pwd;
                            Game.CurrentPlayer = thePlayer;
                            Game.SavePlayer(true);
                            Game.NewPlayerLoaded = true;
                            this.Activity.RunOnUiThread(() =>
                            {
                                UpdateForUser();
                               
                            });
                        }
                        else
                        {
                            // error - probably bad password
                            this.Activity.RunOnUiThread(() =>
                            {
                                MainActivity.ShowAlert(this.Context, Resource.String.Login_Failure_Title.Localize(),
                                Resource.String.Login_Failure_msg.Localize(),
                                Resource.String.Login_Failure_btn.Localize());
                            });

                        }
                    });
                })
                .SetNegativeButton(Resource.String.cancel_btn.Localize(), (s3, e3) =>
                {

                })
                .SetCancelable(true)
                .Show();

           
        }

        private void ChangePwdBtn_Click(object sender, EventArgs e)
        {
            MaybeChangePassword(false);
        }

        private void MaybeChangePassword(bool forced)
        { 
            string titleStr;

            if (forced)
                titleStr = Resource.String.Must_Change_Pwd_Msg.Localize();
            else
                titleStr = Resource.String.Change_Pwd_Msg.Localize();
            var theText = new EditText(this.Context);
            theText.InputType = Android.Text.InputTypes.TextVariationPassword;
            var dialog = new Android.Support.V7.App.AlertDialog.Builder(this.Context)
               .SetTitle(Resource.String.Change_Pwd_Title.Localize())
               .SetMessage(titleStr)
               .SetView(theText)
               .SetPositiveButton(Resource.String.Change_Btn_Title.Localize(), (s2, e2) => {
                   string newPwd = theText.Text;
                   Server.UpdatePassword(newPwd, (resultStr) => {
                       if (string.IsNullOrEmpty(resultStr))
                       {
                           // success
                           this.Activity.RunOnUiThread(() => {
                               Game.CurrentPlayer.pwd = newPwd;
                               Game.SavePlayer(true);
                               UpdateButtonStates();
                           });

                       }
                       else
                       {
                           // something went wrong - show the message
                           MainActivity.ShowAlert(this.Context, Resource.String.Error_Title.Localize(),
                               Resource.String.Change_Username_Err_Msg.Localize(),
                               Resource.String.Change_Username_Err_Btn.Localize());
                       }
                   });
               })
               .SetCancelable(!forced);

            if (!forced)
                dialog = dialog.SetNegativeButton(Resource.String.cancel_btn.Localize(), (s2, e2) => { });

            dialog.Show();
        }

        private void SaveChangesBtn_Click(object sender, EventArgs e)
        {
            if (Game.CurrentPlayer.username != usernameField.Text)
            {
                bool mustSetPassword = false;
                if (Game.CurrentPlayer.username == Game.CurrentPlayer.pwd)
                {
                    // user has to set a password
                    mustSetPassword = true;
                }
                // ok to just set the username
                Server.UpdateUsername(usernameField.Text, (resultStr) => {
                    if (string.IsNullOrEmpty(resultStr))
                    {
                        // success
                        this.Activity.RunOnUiThread(() => {
                            Game.CurrentPlayer.username = usernameField.Text;
                            if (mustSetPassword)
                                MaybeChangePassword(true);
                            else
                            {
                                Game.SavePlayer(true);
                                UpdateButtonStates();
                            }
                        });

                    }
                    else
                    {
                        // something went wrong - show the message
                        MainActivity.ShowAlert(this.Context, Resource.String.Error_Title.Localize(),
                            Resource.String.Username_Msg.Localize(),
                            Resource.String.username_Btn.Localize());
                       
                    }
                });
            }

            // change nickname?
            if (Game.CurrentPlayer.nickname != nicknameField.Text)
            {
                Server.UpdateNickname(nicknameField.Text, (resultStr) => {
                    if (string.IsNullOrEmpty(resultStr))
                    {
                        // success
                        this.Activity.RunOnUiThread(() => {
                            Game.CurrentPlayer.nickname = nicknameField.Text;
                            Game.SavePlayer(true);
                            UpdateButtonStates();
                        });
                    }
                    else
                    {
                        // something went wrong - show the message
                        MainActivity.ShowAlert(this.Context, Resource.String.Error_Title.Localize(),
                            Resource.String.Nickname_Msg.Localize(),
                            Resource.String.Nickname_Btn.Localize());
                    }
                });

            }
        }

        private void SetImageBtn_Click(object sender, EventArgs e)
        {
            string[] optionsList = new string[] { Resource.String.From_Camera_Msg.Localize(),
                Resource.String.From_Gallery_Msg.Localize(),
                Resource.String.Delete_Current_Msg.Localize(),
                Resource.String.cancel_btn.Localize()};
            new Android.Support.V7.App.AlertDialog.Builder(this.Context)
               .SetTitle(Resource.String.New_Image_Msg.Localize())
               .SetCancelable(true)
               .SetItems(optionsList, (s1, e1) =>
               {
                    if (e1.Which == 0)
                   {
                       Intent intent = new Intent(MediaStore.ActionImageCapture);

                       MainActivity._file = new File(MainActivity._dir, String.Format("FlufflePhoto_{0}.jpg", Guid.NewGuid()));

                       intent.PutExtra(MediaStore.ExtraOutput, Uri.FromFile(MainActivity._file));

                       if ((int)Build.VERSION.SdkInt >= 23)
                       {

                       }

                       this.Activity.StartActivityForResult(intent, MainActivity.PHOTO_CAPTURE_EVENT);
                   } else if (e1.Which == 1)
                   {
                       // select an image
                       var imageIntent = new Intent();
                       imageIntent.SetType("image/*");
                       imageIntent.SetAction(Intent.ActionGetContent);

                       this.Activity.StartActivityForResult(
                           Intent.CreateChooser(imageIntent, "Select image"), MainActivity.SELECTIMAGE_REQUEST);
                   } else if (e1.Which == 2)
                   {
                       // remove the image
                       DeleteUserImage();
                   }
                   else if (e1.Which == 3)
                   {
                       // cancel
                      
                   }
               })
               .Show();
        }

        public void HandleActivityResult(int requestCode, Android.App.Result resultCode, Intent data)
        {
            if ((requestCode == MainActivity.SELECTIMAGE_REQUEST || requestCode == MainActivity.PHOTO_CAPTURE_EVENT)
                            && resultCode == Android.App.Result.Ok)
            {
                /*  todo:  progress
                progressBarImageLoading.Visibility = ViewStates.Visible;
                imageCreateBlahLayout.Visibility = ViewStates.Visible;
                imageCreateBlah.SetImageDrawable(null);
                */

                System.IO.Stream fileStream;
                String fileName;

                if (requestCode == MainActivity.SELECTIMAGE_REQUEST)
                {
                    fileStream = StreamHelper.GetStreamFromFileUri(MainActivity.instance, data.Data);
                    //fileStream = this.Context.ContentResolver.OpenInputStream(data.Data);
                    fileName = "some file";// StreamHelper.GetFileName(MainActivity.instance, data.Data);
                }
                else
                {
                    Bitmap scaledBitmap = BitmapHelper.LoadAndResizeBitmap(MainActivity._file.AbsolutePath, MainActivity.MAX_IMAGE_SIZE);
                    fileStream = new System.IO.MemoryStream();
                    scaledBitmap.Compress(Bitmap.CompressFormat.Jpeg, 90, fileStream);
                    fileStream.Flush();
                    fileName = MainActivity._file.Name;
                }

                if (fileStream != null)
                {
                    Server.UploadImage(fileStream, fileName, ProfileImageUploadCallback);
                    fileStream.Close();
                }
                else
                {
                    this.Activity.RunOnUiThread(() =>
                    {
                        var toast = Toast.MakeText(this.Context, "Cannot upload this type of image", ToastLength.Long);
                        toast.Show();
                        //to do:  hide progress
                        //progressBarImageLoading.Visibility = ViewStates.Gone;
                        //ClearImages();
                    });
                }
            }
        }

        private void ProfileImageUploadCallback(string result)
        {
            if (!String.IsNullOrEmpty(result))
            {
                this.Activity.RunOnUiThread(() =>
                {
                    //progressIndicator.StopAnimating();

                    Koush.UrlImageViewHelper.SetUrlDrawable(userImage, result, Resource.Drawable.unknown_user);
                    Game.CurrentPlayer.userimage = result;
                    Server.RecordUserImage(result);
                });
            }
            else
            {
                this.Activity.RunOnUiThread(() =>
                {
                    //progressIndicator.StopAnimating();
                   
                    DeleteUserImage();
                });
            }
            Console.WriteLine(result);
        }


        private void DeleteUserImage()
        {
            Server.RecordUserImage("");
            Game.CurrentPlayer.userimage = null;
            userImage.SetImageResource(Resource.Drawable.unknown_user);
        }

        private void UpdateForUser()
        {
            this.Activity.RunOnUiThread(() =>
            {
                Koush.UrlImageViewHelper.SetUrlDrawable(userImage, Game.CurrentPlayer.userimage, Resource.Drawable.unknown_user);
                nicknameField.Text = Game.CurrentPlayer.nickname;
                usernameField.Text = Game.CurrentPlayer.username;

                if (Game.CurrentPlayer.username == Game.CurrentPlayer.pwd)
                {
                    changePwdBtn.Enabled = false;
                }
                else
                {
                    changePwdBtn.Enabled = true;
                }
                UpdateButtonStates();

            });
        }

        public override void OnHiddenChanged(bool hidden)
        {
            if (hidden)
            {
                
            }
            else
            {
                UpdateForUser();
            }
        }

        private void UpdateButtonStates()
        {
            bool changed = false;

            if (nicknameField.Text != Game.CurrentPlayer.nickname)
                changed = true;

            if (usernameField.Text != Game.CurrentPlayer.username)
                changed = true;

            if (changed)
            {
                saveChangesBtn.Enabled = true;
            }
            else
            {
                saveChangesBtn.Enabled = false;
            }
        }
    }
}

