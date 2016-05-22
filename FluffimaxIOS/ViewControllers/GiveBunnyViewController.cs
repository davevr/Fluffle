﻿using System;
using UIKit;
using Fluffimax.Core;
using ZXing.Mobile;
using System.Timers;
using System.Collections.Generic;

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
		}

		public override void DidReceiveMemoryWarning ()
		{
			base.DidReceiveMemoryWarning ();
			// Release any cached data, images, etc that aren't in use.
		}

		public override void ViewWillAppear (bool animated)
		{
			base.ViewWillAppear (animated);
			NavController.NavigationBarHidden = false;
			Server.StartToss (Game.CurrentPlayer.BunnyBeingSold.id, Game.CurrentPlayer.BunnyBeingSold.Price, (theToss) => {
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
						if (tossRec.ownerId != 0)
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
			DismissViewController(true, () => {
				// do nothing for now
			});
		}

		private void StopTossTimer()
		{
			tossTimer.Stop ();

		}

		protected UINavigationController NavController { 
			get {
				return (UIApplication.SharedApplication.Delegate as AppDelegate).NavController;
			} 
		}	
	}
}


