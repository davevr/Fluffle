using System;

using Foundation;
using UIKit;
using Fluffimax.Core;
using SDWebImage;

namespace Fluffimax.iOS
{
	public partial class AdoptionViewCell : UICollectionViewCell
	{
		public static readonly NSString Key = new NSString("AdoptionViewCell");
		public static readonly UINib Nib;
		private BunnyShopCollectionSource dataSource = null;
		private Bunny LinkedBunny = null;

		static AdoptionViewCell()
		{
			Nib = UINib.FromName("AdoptionViewCell", NSBundle.MainBundle);
		}

		protected AdoptionViewCell(IntPtr handle) : base(handle)
		{
			// Note: this .ctor should not contain any initialization logic.
		}

		private void HandleBuyBtnClick(object sender, EventArgs e)
		{
			dataSource.ShopView.MaybeBuyBunny(LinkedBunny);
		}

		public void ConformToRecord(Bunny theBuns, BunnyShopCollectionSource theSource)
		{
			dataSource = theSource;
			string name = theBuns.BunnyName;
			if (string.IsNullOrEmpty(name))
				name = "new bunny";
			this.BunnyName.Text = name;
			this.BunnyInfo.Text = theBuns.Description;
			this.BunnyPrice.Text = string.Format("{0} c", theBuns.Price);

			this.LinkedBunny = theBuns;

			string bunnyURL = theBuns.GetProfileImage();
			BunnyImage.SetImage(
				url: new NSUrl(bunnyURL),
				placeholder: UIImage.FromBundle("bunny.png")
			);
		}
	}
}
