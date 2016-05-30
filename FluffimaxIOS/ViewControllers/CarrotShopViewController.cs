using System;

using UIKit;
using Fluffimax.Core;
using Foundation;
using System.Collections.Generic;
using StoreKit;

namespace Fluffimax.iOS
{
	public partial class CarrotShopViewController : UIViewController
	{
		public static string BuyCarrot01 = "com.eweware.fluffle.carrot01";
		public static string BuyCarrot02 = "com.eweware.fluffle.carrot02";
		public static string BuyCarrot03 = "com.eweware.fluffle.carrot03";
		public static string BuyCarrot04 = "com.eweware.fluffle.carrot04";
		public static string BuyCarrot05 = "com.eweware.fluffle.carrot05";

		NSObject priceObserver, succeededObserver, failedObserver, requestObserver;

		InAppPurchaseManager iap;
		List<string> products;
		bool pricesLoaded = false;

		public CarrotShopViewController () : base ("CarrotShopViewController", null)
		{
			products = new List<string>() { BuyCarrot01, BuyCarrot02, BuyCarrot03, BuyCarrot04, BuyCarrot05 };
			iap = new InAppPurchaseManager();
		}

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();
			// Perform any additional setup after loading the view, typically from a nib.

			BuyItem1Btn.TouchUpInside += (object sender, EventArgs e) => {
				iap.PurchaseProduct(BuyCarrot01);
			};

			BuyItem2Btn.TouchUpInside += (object sender, EventArgs e) => {
				iap.PurchaseProduct(BuyCarrot02);
			};

			BuyItem3Btn.TouchUpInside += (object sender, EventArgs e) => {
				iap.PurchaseProduct(BuyCarrot03);
			};

			BuyItem4Btn.TouchUpInside += (object sender, EventArgs e) => {
				iap.PurchaseProduct(BuyCarrot04);
			};

			BuyItem5Btn.TouchUpInside += (object sender, EventArgs e) => {
				iap.PurchaseProduct(BuyCarrot05);
			};

			WatchAdBtn.TouchUpInside += (object sender, EventArgs e) => {
				HandleAdView();
			};

		}
			

		private void HandleAdView() {
			int carrotCount = 10;

			if (carrotCount > 0) {
				Game.CurrentPlayer.GiveCarrots (carrotCount);
				UpdateTextLabels ();
			}
		}

		private void UpdateTextLabels() {
			InvokeOnMainThread (() => {
				CurrentCarrotLabel.Text = String.Format("you have {0} carrots", Game.CurrentPlayer.carrotCount);
			});
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
			UpdateTextLabels ();

			priceObserver = NSNotificationCenter.DefaultCenter.AddObserver (InAppPurchaseManager.InAppPurchaseManagerProductsFetchedNotification, 
				(notification) => {
					var info = notification.UserInfo;
					if (info == null) return;

					var NSBuyProduct01Id = new NSString(BuyCarrot01);
					var NSBuyProduct02Id = new NSString(BuyCarrot02);
					var NSBuyProduct03Id = new NSString(BuyCarrot03);
					var NSBuyProduct04Id = new NSString(BuyCarrot04);
					var NSBuyProduct05Id = new NSString(BuyCarrot05);

					if (info.ContainsKey(NSBuyProduct01Id)) {
						pricesLoaded = true;

						var product = (SKProduct) info.ObjectForKey(NSBuyProduct01Id);

						Console.WriteLine("Product id: " + product.ProductIdentifier);
						Console.WriteLine("Product title: " + product.LocalizedTitle);
						Console.WriteLine("Product description: " + product.LocalizedDescription);
						Console.WriteLine("Product price: " + product.Price);
						Console.WriteLine("Product l10n price: " + product.LocalizedPrice());	

						BuyItem1Btn.Enabled = true;
						BuyItem1Btn.SetTitle(product.LocalizedTitle + " - " + product.LocalizedDescription + " - " + product.LocalizedPrice(), UIControlState.Normal);

					}
					if (info.ContainsKey(NSBuyProduct02Id)) {
						pricesLoaded = true;

						var product = (SKProduct) info.ObjectForKey(NSBuyProduct02Id);

						BuyItem2Btn.Enabled = true;
						BuyItem2Btn.SetTitle(product.LocalizedTitle + " - " + product.LocalizedDescription + " - " + product.LocalizedPrice(), UIControlState.Normal);
					}

					if (info.ContainsKey(NSBuyProduct03Id)) {
						pricesLoaded = true;

						var product = (SKProduct) info.ObjectForKey(NSBuyProduct03Id);

						BuyItem3Btn.Enabled = true;
						BuyItem3Btn.SetTitle(product.LocalizedTitle + " - " + product.LocalizedDescription + " - " + product.LocalizedPrice(), UIControlState.Normal);
					}

					if (info.ContainsKey(NSBuyProduct04Id)) {
						pricesLoaded = true;

						var product = (SKProduct) info.ObjectForKey(NSBuyProduct04Id);

						BuyItem4Btn.Enabled = true;
						BuyItem4Btn.SetTitle(product.LocalizedTitle + " - " + product.LocalizedDescription + " - " + product.LocalizedPrice(), UIControlState.Normal);
					}

					if (info.ContainsKey(NSBuyProduct05Id)) {
						pricesLoaded = true;

						var product = (SKProduct) info.ObjectForKey(NSBuyProduct05Id);

						BuyItem5Btn.Enabled = true;
						BuyItem5Btn.SetTitle(product.LocalizedTitle + " - " + product.LocalizedDescription + " - " + product.LocalizedPrice(), UIControlState.Normal);
					}

				});

			// only if we can make payments, request the prices
			if (iap.CanMakePayments()) {
				// now go get prices, if we don't have them already
				if (!pricesLoaded)
					iap.RequestProductData(products); // async request via StoreKit -> App Store
			} else {
				// can't make payments (purchases turned off in Settings?)
				BuyItem1Btn.SetTitle ("AppStore disabled", UIControlState.Disabled);
				BuyItem2Btn.SetTitle ("AppStore disabled", UIControlState.Disabled);
				BuyItem3Btn.SetTitle ("AppStore disabled", UIControlState.Disabled);
				BuyItem4Btn.SetTitle ("AppStore disabled", UIControlState.Disabled);
				BuyItem5Btn.SetTitle ("AppStore disabled", UIControlState.Disabled);
			}
				

			succeededObserver = NSNotificationCenter.DefaultCenter.AddObserver (InAppPurchaseManager.InAppPurchaseManagerTransactionSucceededNotification, 
				(notification) => {
					Console.WriteLine("Purchase Worked!");
					//todo: update thing
					Server.GetCarrotCount((newCount)=> {
						UpdateTextLabels();
					});
				});
			failedObserver = NSNotificationCenter.DefaultCenter.AddObserver (InAppPurchaseManager.InAppPurchaseManagerTransactionFailedNotification, 
				(notification) => {
					// TODO: 
					Console.WriteLine ("Transaction Failed");
				});

			requestObserver = NSNotificationCenter.DefaultCenter.AddObserver (InAppPurchaseManager.InAppPurchaseManagerRequestFailedNotification, 
				(notification) => {
					// TODO: 
					Console.WriteLine ("Request Failed");
					BuyItem1Btn.SetTitle ("Network down?", UIControlState.Disabled);
					BuyItem2Btn.SetTitle ("Network down?", UIControlState.Disabled);
					BuyItem3Btn.SetTitle ("Network down?", UIControlState.Disabled);
					BuyItem4Btn.SetTitle ("Network down?", UIControlState.Disabled);
					BuyItem5Btn.SetTitle ("Network down?", UIControlState.Disabled);
				});
		}

		public override void ViewWillDisappear (bool animated)
		{
			// remove the observer when the view isn't visible
			NSNotificationCenter.DefaultCenter.RemoveObserver (priceObserver);
			NSNotificationCenter.DefaultCenter.RemoveObserver (succeededObserver);
			NSNotificationCenter.DefaultCenter.RemoveObserver (failedObserver);
			NSNotificationCenter.DefaultCenter.RemoveObserver (requestObserver);

			base.ViewWillDisappear (animated);
		}

		protected UINavigationController NavController { 
			get {
				return (UIApplication.SharedApplication.Delegate as AppDelegate).NavController;
			} 
		}

	}
}


