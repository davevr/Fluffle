using System;

using Foundation;
using UIKit;
using Fluffimax.Core;

using SDWebImage;

namespace Fluffimax.iOS
{
	public partial class PlayerCountCellView : UITableViewCell
	{
		public static readonly NSString Key = new NSString ("PlayerCountCellView");
		public static readonly UINib Nib;

		static PlayerCountCellView ()
		{
			Nib = UINib.FromName ("PlayerCountCellView", NSBundle.MainBundle);
		}

		public PlayerCountCellView (IntPtr handle) : base (handle)
		{
		}

		public void ConformToRecord(Player thePlayer, PlayerCountTableSource theSource) {
			PlayerName.Text = string.IsNullOrEmpty (thePlayer.nickname) ? "unknown" : thePlayer.nickname;
			BunnyCount.Text = thePlayer.totalBunnies.ToString ();
			JoinedDateLabel.Text = thePlayer.creationDate.ToShortDateString ();

			if (String.IsNullOrEmpty (thePlayer.userimage)) {
				PlayerImg.Image = UIImage.FromBundle ("unknown_user");
			} else {
				PlayerImg.SetImage (
					url: new NSUrl (thePlayer.userimage), 
					placeholder: UIImage.FromBundle ("unknown_user")
				);
			}
		}
	}
}
