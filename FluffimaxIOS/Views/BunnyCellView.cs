using System;

using Foundation;
using UIKit;
using Fluffimax.Core;


namespace Fluffimax.iOS
{
	public partial class BunnyCellView : UITableViewCell
	{
		public static readonly NSString Key = new NSString ("BunnyCellView");
		public static readonly UINib Nib;
		private BunnyShopTableSource dataSource = null;
		private Bunny LinkedBunny = null;

		static BunnyCellView ()
		{
			Nib = UINib.FromName ("BunnyCellView", NSBundle.MainBundle);

		}

		public BunnyCellView (IntPtr handle) : base (handle)
		{
			
		}

		private void  HandleBuyBtnClick(object sender, EventArgs e) {
			dataSource.ShopView.MaybeBuyBunny(LinkedBunny);
		}

		public void ConformToRecord(Bunny theBuns, BunnyShopTableSource theSource) {
			dataSource = theSource;
			//this.BunnyImage = //todo: get image for this bunny;
			this.BreedLabel.Text = theBuns.BreedName;
			this.GenderLabel.Text = theBuns.Female ? "female" : "male";
			this.FurColorLabel.Text = "fur: " + theBuns.FurColorName;
			this.EyeColorLabel.Text = "eyes: " + theBuns.EyeColorName;
			this.PriceLabel.Text = theBuns.Price.ToString ();
			this.LinkedBunny = theBuns;
			this.BuyBtn.TouchUpInside -= HandleBuyBtnClick;
			this.BuyBtn.TouchUpInside += HandleBuyBtnClick;
		}


	}
}
