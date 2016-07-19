using System;
using UIKit;
using Fluffimax.Core;
using ZXing.Mobile;
using System.Timers;
using SDWebImage;
using Foundation;

namespace Fluffimax.iOS
{
	public partial class GiveBunnyViewController : UIViewController
	{
		Timer	tossTimer;
		int secondsLeft;

		public GiveBunnyViewController () : base ("GiveBunnyViewController", null)
		{
		}

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();
			// Perform any additional setup after loading the view, typically from a nib.
		
			DoneBtn.TouchUpInside += (object sender, EventArgs e) => {
				EndToss();
			};
		}

		public override void DidReceiveMemoryWarning ()
		{
			base.DidReceiveMemoryWarning ();
			// Release any cached data, images, etc that aren't in use.
		}

		public override void ViewWillAppear (bool animated)
		{
			base.ViewWillAppear (animated);
			string bunsName = Game.BunnyBeingSold.BunnyName;
			if (string.IsNullOrEmpty(bunsName))
				bunsName = "unnamed bunny";

			BunnyNameLabel.Text = bunsName;
			BunnyImage.SetImage(new NSUrl(Game.BunnyBeingSold.GetProfileImage()));
			BunnyInfoLabel.Text = Game.BunnyBeingSold.Description;

			NavController.NavigationBarHidden = true;
			Server.StartToss (Game.BunnyBeingSold.id, Game.BunnySellPrice, (theToss) => {
				var writer = new BarcodeWriter {
					Format = ZXing.BarcodeFormat.AZTEC,
					Options = new ZXing.Common.EncodingOptions {
						Width = 240,
						Height = 240,
						Margin = 1
					}
				};
				Game.CurrentTossId = theToss.id;
				string guid = Game.CurrentTossId.ToString ();
				string url = guid;
				var bitMap = writer.Write (url);
				InvokeOnMainThread(() => 
					{
						TossImageView.Image = bitMap;
						StartTossTimer();
					});
			});

		}

		private void StartTossTimer()
		{
			tossTimer = new Timer ();
			tossTimer.Interval = 1000;
			tossTimer.AutoReset = true;
			tossTimer.Elapsed += HandleTossTimerTick;
			secondsLeft = 60;
			tossTimer.Start ();
		}

		private void HandleTossTimerTick(object sender, ElapsedEventArgs e)
		{
			secondsLeft--;
			if (secondsLeft < 0) {
				EndToss ();
			} else {
				Server.GetTossStatus(Game.CurrentTossId, (tossRec) => {
					InvokeOnMainThread (() => {
						if (tossRec.catcherId != 0)
							EndToss();
						else 
							DoneBtn.SetTitle(String.Format ("Done (ending in {0} seconds)", secondsLeft), UIControlState.Normal);
					});
				});
			}
		}



		private void EndToss()
		{
			StopTossTimer ();
			Server.GetTossStatus(Game.CurrentTossId, (tossRec) => {
				InvokeOnMainThread(() => {
					if (tossRec.catcherId == 0)
						Game.BunnyBeingSold = null;
					NavController.PopViewController(true);
				});
			});
		}

		private void StopTossTimer()
		{
			if (tossTimer != null)
				tossTimer.Stop ();

		}

		protected UINavigationController NavController { 
			get {
				return (UIApplication.SharedApplication.Delegate as AppDelegate).RootController.NavController;
			} 
		}	
	}
}


