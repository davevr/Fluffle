
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
using Xamarin.InAppBilling;
using Android.Graphics;
using Fluffimax.Core;
using Android.Gms.Ads;
using Android.Gms.Ads.Reward;

namespace Fluffle.AndroidApp
{
	[Activity(Label = "CarrotStoreActivity", Icon = "@drawable/baseicon",
			 Theme = "@style/Theme.AppCompat.Light", ScreenOrientation = Android.Content.PM.ScreenOrientation.Portrait)]
	public class CarrotStoreActivity : Activity, IRewardedVideoAdListener
	{
		public static string apikey = "MIIBIjANBgkqhkiG9w0BAQEFAAOCAQ8AMIIBCgKCAQEAwBQMKprukm36hOk5CeswtMHNMXFD604Yqk82DhlPIsSjEEQA9tTjrSxeoN59OjQ98cjrDEl4au2QINE0epNWYUgCVrGGTnqMOKJz3U2BxXvjlpiq8nOy0XLjpnWNiZC8GWRcTVz1T8HbMveps4GBqfEdioLERot/aaFNh+AdFq7S8e0dGCxrChy0n8yIglkKvxRvK7AjwE/amI1X/xYnhzy4Lf5ocCWh3CPmyU6BaAwGyoEMrr1WwflquztOaA4Q6T2TQUH8oJS+7ZOG9VzKidmsUssJw9n0lWZ8BqG653/0mVDv94F2ii9xYJmilbX0M4Et1F7tjIh5Y0nXXnfWeQIDAQAB";
		private InAppBillingServiceConnection _serviceConnection;
		IList<Product> _products;

		LinearLayout item1area;
		TextView item01title;
		TextView item01desc;
		LinearLayout item2area;
		TextView item02title;
		TextView item02desc;
		LinearLayout item3area;
		TextView item03title;
		TextView item03desc;
		LinearLayout item4area;
		TextView item04title;
		TextView item04desc;
		LinearLayout item5area;
		TextView item05title;
		TextView item05desc;
		TextView availableLabel;
        protected AdView mAdView;
        protected IRewardedVideoAd mRewardAd;
        protected Button mLoadInterstitialButton;

        protected override void OnCreate(Bundle savedInstanceState)
		{
			base.OnCreate(savedInstanceState);

			// Create your application here
			SetContentView(Resource.Layout.CarrotStoreLayout);
			availableLabel = FindViewById<TextView>(Resource.Id.availableLabel);

			item1area = FindViewById<LinearLayout>(Resource.Id.buyItem01);
			item01title = FindViewById<TextView>(Resource.Id.item01title);
			item01desc = FindViewById<TextView>(Resource.Id.item01details);

			item2area = FindViewById<LinearLayout>(Resource.Id.buyItem02);
			item02title = FindViewById<TextView>(Resource.Id.item02title);
			item02desc = FindViewById<TextView>(Resource.Id.item02details);

			item3area = FindViewById<LinearLayout>(Resource.Id.buyItem03);
			item03title = FindViewById<TextView>(Resource.Id.item03title);
			item03desc = FindViewById<TextView>(Resource.Id.item03details);

			item4area = FindViewById<LinearLayout>(Resource.Id.buyItem04);
			item04title = FindViewById<TextView>(Resource.Id.item04title);
			item04desc = FindViewById<TextView>(Resource.Id.item04details);

			item5area = FindViewById<LinearLayout>(Resource.Id.buyItem05);
			item05title = FindViewById<TextView>(Resource.Id.item05title);
			item05desc = FindViewById<TextView>(Resource.Id.item05details);

            

            // fonts
            item01title.SetTypeface(MainActivity.bodyFace, TypefaceStyle.Normal);
			item02title.SetTypeface(MainActivity.bodyFace, TypefaceStyle.Normal);
			item03title.SetTypeface(MainActivity.bodyFace, TypefaceStyle.Normal);
			item04title.SetTypeface(MainActivity.bodyFace, TypefaceStyle.Normal);
			item05title.SetTypeface(MainActivity.bodyFace, TypefaceStyle.Normal);
			availableLabel.SetTypeface(MainActivity.bodyFace, TypefaceStyle.Normal);

			item1area.Click += (sender, e) => { BuyItem("carrot_level_01"); };
			item2area.Click += (sender, e) => { BuyItem("carrot_level_02"); };
			item3area.Click += (sender, e) => { BuyItem("carrot_level_03"); };
			item4area.Click += (sender, e) => { BuyItem("carrot_level_04"); };
			item5area.Click += (sender, e) => { BuyItem("carrot_level_05"); };

			_serviceConnection = new InAppBillingServiceConnection(this, apikey);
			_serviceConnection.OnConnected += () =>
			{
				UpdateInventory();
			};

			_serviceConnection.OnInAppBillingError += HandleOnInAppBillingErrorDelegate;
			UpdateCarrotCount();
            mAdView = FindViewById<AdView>(Resource.Id.adView);
            var adRequest = new AdRequest.Builder().AddTestDevice("4B5B3CDF6F421EC8A06A35832AC88E9B").Build();
            mAdView.LoadAd(adRequest);

            mRewardAd = MobileAds.GetRewardedVideoAdInstance(this);
            mRewardAd.RewardedVideoAdListener = this;

            mLoadInterstitialButton = FindViewById<Button>(Resource.Id.load_interstitial_button);
            //mLoadInterstitialButton.SetOnClickListener(new OnClickListener(this));
            mLoadInterstitialButton.Click += MLoadInterstitialButton_Click;
            mLoadInterstitialButton.Visibility = ViewStates.Gone ;


        }

        private void MLoadInterstitialButton_Click(object sender, EventArgs e)
        {
            if (mRewardAd.IsLoaded)
            {
                mLoadInterstitialButton.Visibility = ViewStates.Gone;
                mRewardAd.UserId = Game.CurrentPlayer.id.ToString();
                mRewardAd.Show();
            }
        }

        private Product FindProduct(string prodIdStr)
		{
			foreach (Product curProd in _products)
			{
				if (curProd.ProductId == prodIdStr)
					return curProd;
			}

			return null;
		}

        public void RequestNewRewardAd()
        {
            var adRequest = new AdRequest.Builder().AddTestDevice("4B5B3CDF6F421EC8A06A35832AC88E9B").Build();
            mRewardAd.LoadAd(GetString(Resource.String.test_reward_ad_unit_id), adRequest);
            
        }

        public void OnRewardedVideoAdClosed()
        {

        }

        public void OnRewardedVideoAdFailedToLoad(int errCode)
        {

        }

        public void OnRewardedVideoAdLeftApplication()
        {

        }

        public void OnRewarded(IRewardItem theItem)
        {
            Toast.MakeText(this, "Thanks for supporting Fluffle!", ToastLength.Short).Show();

            Server.RecordPurchase("video_ad", "ad", theItem.Type, (theResult) =>
            {
                UpdateCarrotCount();
            });
           

        }

        public void OnRewardedVideoAdLoaded()
        {
            mLoadInterstitialButton.Visibility = ViewStates.Visible;
        }

        public void OnRewardedVideoAdOpened()
        {

        }

        public void OnRewardedVideoStarted()
        {

        }






      

        protected override void OnPause()
        {
            if (mAdView != null)
            {
                mAdView.Pause();
            }
            mRewardAd.Pause();
            base.OnPause();
        }

        protected override void OnResume()
        {
            base.OnResume();
            _serviceConnection.Connect();
            if (mAdView != null)
            {
                mAdView.Resume();
            }
            if (!mRewardAd.IsLoaded)
            {
                RequestNewRewardAd();
            }
            mRewardAd.Resume();
        }

        protected override void OnDestroy()
        {
            if (mAdView != null)
            {
                mAdView.Destroy();
            }
            mRewardAd.Destroy();
            base.OnDestroy();
        }



        private void BuyItem(string prodIdStr)
		{
            Product _selectedProduct = FindProduct(prodIdStr);

			_serviceConnection.BillingHandler.BuyProduct(_selectedProduct);
		}

		protected override void OnActivityResult(int requestCode, Result resultCode, Intent data)
		{
			// Ask the open service connection's billing handler to process this request
			_serviceConnection.BillingHandler.HandleActivityResult(requestCode, resultCode, data);

			// TODO: Use a call back to update the purchased items
			// or listen to the OnProductPurchased event to
			// handle a successful purchase
		}

		private async void UpdateInventory()
		{
			_products = await _serviceConnection.BillingHandler.QueryInventoryAsync(new List<string> {
                "carrot_level_01",
				"carrot_level_02",
				"carrot_level_03",
				"carrot_level_04",
				"carrot_level_05"
			}, ItemType.Product);

			// Were any products returned?
			if (_products == null)
			{
				// No, abort
				return;
			}
			else {
				UpdateStoreProducts();
			}
		}


		protected override void OnStop()
		{
			_serviceConnection.Disconnect();
			base.OnStop();
		}

		void HandleOnInAppBillingErrorDelegate(InAppBillingErrorType error, string message)
		{
			//todo - do something on this error
		}

		private void UpdateStoreProducts()
		{
			_serviceConnection.BillingHandler.OnProductPurchased += HandleOnProductPurchasedDelegate;
			_serviceConnection.BillingHandler.OnPurchaseConsumed += BillingHandler_OnPurchaseConsumed;
			RunOnUiThread(() =>
			{
				foreach (Product curProd in _products)
				{
					TextView titleItem = null;
					TextView infoItem = null;

					switch (curProd.ProductId)
					{
						case "carrot_level_01":
							titleItem = item01title;
							infoItem = item01desc;
							break;
						case "carrot_level_02":
							titleItem = item02title;
							infoItem = item02desc;
							break;
						case "carrot_level_03":
							titleItem = item03title;
							infoItem = item03desc;
							break;
						case "carrot_level_04":
							titleItem = item04title;
							infoItem = item04desc;
							break;
						case "carrot_level_05":
							titleItem = item05title;
							infoItem = item05desc;
							break;
					}

					if (titleItem != null && infoItem != null)
					{
                        var titleText = curProd.Title.Substring(0, curProd.Title.IndexOf(" ("));
						titleItem.Text = string.Format("{0} - {1}", titleText, curProd.Price);
						infoItem.Text = curProd.Description;
					}

					MainActivity.ShowTutorialStep(this, "carrot_shop_tutorial", Resource.String.carrot_shop_tutorial);


				}
			});
		}

		void HandleOnProductPurchasedDelegate(int response, Purchase purchase, string purchaseData, string purchaseSignature)
		{
			// we consume these instantly to get carrots
			bool result = _serviceConnection.BillingHandler.ConsumePurchase(purchase);

			// Was the product consumed?
			if (result)
			{
				// ok, notify the server
				Server.RecordPurchase(purchase.ProductId, "google", purchaseData, (theResult) =>
				{
					if (theResult > 0)
						UpdateCarrotCount();
				});
			}
		}

		void BillingHandler_OnPurchaseConsumed(string token)
		{
			Console.WriteLine("purchased consumed - " + token);
		}

		void UpdateCarrotCount()
		{
			RunOnUiThread(() =>
			{
				availableLabel.Text = string.Format("{0} carrots available", Game.CurrentPlayer.carrotCount);

			});
		}
    }

    
}

