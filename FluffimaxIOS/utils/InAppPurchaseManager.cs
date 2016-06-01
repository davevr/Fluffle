using System;
using System.Collections.Generic;
using StoreKit;
using Foundation;
using UIKit;
using Fluffimax.Core;

namespace Fluffimax.iOS {
	public class InAppPurchaseManager : SKProductsRequestDelegate {
		public static NSString InAppPurchaseManagerProductsFetchedNotification = new NSString("InAppPurchaseManagerProductsFetchedNotification");
		public static NSString InAppPurchaseManagerTransactionFailedNotification  = new NSString("InAppPurchaseManagerTransactionFailedNotification");
		public static NSString InAppPurchaseManagerTransactionSucceededNotification  = new NSString("InAppPurchaseManagerTransactionSucceededNotification");
		public static NSString InAppPurchaseManagerRequestFailedNotification = new NSString("InAppPurchaseManagerRequestFailedNotification");

		SKProductsRequest productsRequest;
		CustomPaymentObserver theObserver;

		public static Action Done {get;set;}

		public InAppPurchaseManager ()
		{
			theObserver = new CustomPaymentObserver(this);

			// Call this once upon startup of in-app-purchase activities
			// This also kicks off the TransactionObserver which handles the various communications
			SKPaymentQueue.DefaultQueue.AddTransactionObserver(theObserver);
		}

		// Verify that the iTunes account can make this purchase for this application
		public bool CanMakePayments()
		{
			return SKPaymentQueue.CanMakePayments;	
		}

		// request multiple products at once
		public void RequestProductData (List<string> productIds)
		{
			var array = new NSString[productIds.Count];
			for (var i = 0; i < productIds.Count; i++) {
				array[i] = new NSString(productIds[i]);
			}
			NSSet productIdentifiers = NSSet.MakeNSObjectSet<NSString>(array);			

			//set up product request for in-app purchase
			productsRequest  = new SKProductsRequest(productIdentifiers);
			productsRequest.Delegate = this; // SKProductsRequestDelegate.ReceivedResponse
			productsRequest.Start();
		}

		public override void ReceivedResponse (SKProductsRequest request, SKProductsResponse response)
		{
			SKProduct[] products = response.Products;

			NSDictionary userInfo = null;
			if (products.Length > 0) {
				NSObject[] productIdsArray = new NSObject[response.Products.Length];
				NSObject[] productsArray = new NSObject[response.Products.Length];
				for (int i = 0; i < response.Products.Length; i++) {
					productIdsArray[i] = new NSString(response.Products[i].ProductIdentifier);
					productsArray[i] = response.Products[i];
				}
				userInfo = NSDictionary.FromObjectsAndKeys (productsArray, productIdsArray);
			}
			NSNotificationCenter.DefaultCenter.PostNotificationName(InAppPurchaseManagerProductsFetchedNotification,this,userInfo);

			foreach (string invalidProductId in response.InvalidProducts) {
				Console.WriteLine("Invalid product id: " + invalidProductId );
			}
		}

		public void PurchaseProduct(string appStoreProductId)
		{
			if (Game.CurrentPlayer.isAdmin) {
				Server.RecordPurchase (appStoreProductId, "", "", (theResult) => {
					// true if it did it
					FinishTransaction (null, true);
				});
			} else {
				SKPayment payment = SKPayment.PaymentWithProduct (appStoreProductId);
				SKPaymentQueue.DefaultQueue.AddPayment (payment);
			}
		}

		public void CompleteTransaction (SKPaymentTransaction transaction)
		{
			Console.WriteLine("CompleteTransaction " + transaction.TransactionIdentifier);
			var productId = transaction.Payment.ProductIdentifier;
			NSUrl receiptURL = NSBundle.MainBundle.AppStoreReceiptUrl;
			NSData receipt = NSData.FromUrl (receiptURL);
			NSDictionary requestContents = 
				NSDictionary.FromObjectAndKey((NSString)receipt.GetBase64EncodedString(
					NSDataBase64EncodingOptions.None), 
					(NSString)"receipt-data");

			string receiptData = (requestContents["receipt-data"] as NSString).ToString();

			Server.RecordPurchase (transaction.Payment.ProductIdentifier, "apple", receiptData, (theResult) => {
				// true if it did it
				FinishTransaction (transaction, theResult);
			});
		}

		public void FailedTransaction (SKPaymentTransaction transaction)
		{
			//SKErrorPaymentCancelled == 2
			if (transaction.Error.Code == 2) // user cancelled
				Console.WriteLine("User CANCELLED FailedTransaction Code=" + transaction.Error.Code + " " + transaction.Error.LocalizedDescription);
			else // error!
				Console.WriteLine("FailedTransaction Code=" + transaction.Error.Code + " " + transaction.Error.LocalizedDescription);

			FinishTransaction(transaction,false);
		}
		public void FinishTransaction(SKPaymentTransaction transaction, bool wasSuccessful)
		{
			if (wasSuccessful) {
				if (transaction == null) {
					NSDictionary theDict = NSDictionary.FromObjectAndKey (new NSString ("admin"), new NSString ("SKU"));

					Flurry.Analytics.FlurryAgent.LogEvent ("buy_carrots", theDict);
				} else {
					NSDictionary theDict = NSDictionary.FromObjectAndKey (new NSString (transaction.Payment.ProductIdentifier), new NSString ("SKU"));

					Flurry.Analytics.FlurryAgent.LogEvent ("buy_carrots", theDict);
				}
			}
			Console.WriteLine("FinishTransaction " + wasSuccessful);
			// remove the transaction from the payment queue.
			if (transaction != null)
				SKPaymentQueue.DefaultQueue.FinishTransaction(transaction);		// THIS IS IMPORTANT - LET'S APPLE KNOW WE'RE DONE !!!!

			using (var pool = new NSAutoreleasePool()) {
				NSDictionary userInfo = NSDictionary.FromObjectsAndKeys(new NSObject[] {transaction},new NSObject[] { new NSString("transaction")});
				if (wasSuccessful) {
					// send out a notification that we’ve finished the transaction
					NSNotificationCenter.DefaultCenter.PostNotificationName(InAppPurchaseManagerTransactionSucceededNotification,this,userInfo);
				} else {
					// send out a notification for the failed transaction
					NSNotificationCenter.DefaultCenter.PostNotificationName(InAppPurchaseManagerTransactionFailedNotification,this,userInfo);
				}
			}
		}

		/// <summary>
		/// Probably could not connect to the App Store (network unavailable?)
		/// </summary>
		public override void RequestFailed (SKRequest request, NSError error)
		{
			Console.WriteLine (" ** InAppPurchaseManager RequestFailed() " + error.LocalizedDescription);
			using (var pool = new NSAutoreleasePool()) {
				NSDictionary userInfo = NSDictionary.FromObjectsAndKeys(new NSObject[] {error},new NSObject[] {new NSString("error")});
				// send out a notification for the failed transaction
				NSNotificationCenter.DefaultCenter.PostNotificationName(InAppPurchaseManagerRequestFailedNotification,this,userInfo);
			}
		}
	}
}