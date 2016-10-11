
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
using System.Timers;
using Fluffimax.Core;
using ZXing.Mobile;
using Android.Graphics;
using Android.Views.Animations;
using Android.Graphics.Drawables;


namespace Fluffle.AndroidApp
{
	[Activity(Label = "TossBunnyActivity", Theme = "@style/Theme.AppCompat.Light", ScreenOrientation = Android.Content.PM.ScreenOrientation.Portrait)]
	public class TossBunnyActivity : Activity
	{
        TextView title;
        TextView bunnyName;
        TextView bunnyInfo;
        Button doneBtn;
        ImageView bunnyIcon;
        ImageView aztekView;

        Timer tossTimer;
        int secondsLeft;

        protected override void OnCreate(Bundle savedInstanceState)
		{
			base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.TossBunnyLayout);
            title = FindViewById<TextView>(Resource.Id.titleText);
            bunnyName = FindViewById<TextView>(Resource.Id.bunnyName);
            bunnyInfo = FindViewById<TextView>(Resource.Id.bunnyInfo);
            doneBtn = FindViewById<Button>(Resource.Id.doneBtn);
            bunnyIcon = FindViewById<ImageView>(Resource.Id.bunnyImg);
            aztekView = FindViewById<ImageView>(Resource.Id.aztekView);
            aztekView.SetScaleType(ImageView.ScaleType.FitStart);
            title.SetTypeface(MainActivity.bodyFace, Android.Graphics.TypefaceStyle.Normal);
            bunnyName.SetTypeface(MainActivity.bodyFace, Android.Graphics.TypefaceStyle.Normal);

            doneBtn.Click += (s, e) => { EndToss(); };

            // init the buns
            aztekView.Post(() =>
            {
                LinearLayout.LayoutParams layout = new LinearLayout.LayoutParams(aztekView.Width, aztekView.Width);
                layout.LeftMargin = 0;
                layout.TopMargin = 8;
                aztekView.LayoutParameters = layout;
            });
            InitForBuns();
            
        }

       

        void InitForBuns()
        {
            string bunsName = Game.BunnyBeingSold.BunnyName;
            if (string.IsNullOrEmpty(bunsName))
                bunsName = Resource.String.Unamed_Bunny.Localize();

            bunnyName.Text = bunsName;
            bunnyInfo.Text = Game.BunnyBeingSold.Description;

            Koush.UrlImageViewHelper.SetUrlDrawable(bunnyIcon, Game.BunnyBeingSold.GetProfileImage(), Resource.Drawable.baseicon);

            Server.StartToss(Game.BunnyBeingSold.id, Game.BunnySellPrice, (theToss) => {
                var writer = new BarcodeWriter
                {
                    Format = ZXing.BarcodeFormat.AZTEC,
                    Options = new ZXing.Common.EncodingOptions
                    {
                        Width = 240,
                        Height = 240,
                        Margin = 1
                    }
                };
                Game.CurrentTossId = theToss.id;
                string guid = Game.CurrentTossId.ToString();
                string url = guid;
                var bitMap = writer.Write(url);
                RunOnUiThread(() =>
                {
                    aztekView.SetImageBitmap(bitMap);
                    StartTossTimer();
                });
            });
        }

        private void StartTossTimer()
        {
            tossTimer = new Timer();
            tossTimer.Interval = 1000;
            tossTimer.AutoReset = true;
            tossTimer.Elapsed += HandleTossTimerTick;
            secondsLeft = 60;
            tossTimer.Start();
        }

        private void HandleTossTimerTick(object sender, ElapsedEventArgs e)
        {
            secondsLeft--;
            if (secondsLeft < 0)
            {
                EndToss();
            }
            else
            {
                Server.GetTossStatus(Game.CurrentTossId, (tossRec) => {
                    RunOnUiThread(() => {
                        if (tossRec.catcherId != 0)
                            EndToss();
                        else
                        {
                            string formatStr = Resource.String.Timeout_Msg.Localize();
                            string textStr = string.Format(formatStr, secondsLeft);

                            doneBtn.Text = textStr;
                        }
                    });
                });
            }
        }

        private void EndToss()
        {
            StopTossTimer();
            Server.GetTossStatus(Game.CurrentTossId, (tossRec) => {
                RunOnUiThread(() => {
                    if (tossRec.catcherId == 0)
                    {
                        Game.BunnyBeingSold = null;
                        SetResult(Result.Canceled);
                    } else
                    {
                        var resultIntent = new Intent();
                        resultIntent.PutExtra("catcherid", tossRec.catcherId);
                        SetResult(Result.Ok, resultIntent);
                    }
                    
                    Finish();
                });
            });
        }

        private void StopTossTimer()
        {
            if (tossTimer != null)
                tossTimer.Stop();

        }

    }
}

