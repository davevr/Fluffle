using System;

using Foundation;
using UIKit;
using Fluffimax.Core;
using SDWebImage;

namespace Fluffimax.iOS
{
	public partial class PlayerShareCellView : UITableViewCell
	{
		public static readonly NSString Key = new NSString ("PlayerShareCellView");
		public static readonly UINib Nib;

		static PlayerShareCellView ()
		{
			Nib = UINib.FromName ("PlayerShareCellView", NSBundle.MainBundle);
		}

		public PlayerShareCellView (IntPtr handle) : base (handle)
		{
		}

		public void ConformToRecord(Player thePlayer, PlayerShareTableSource theSource) {
			PlayerName.Text = string.IsNullOrEmpty (thePlayer.nickname) ? "unknown" : thePlayer.nickname;
			ShareCount.Text = thePlayer.totalShares.ToString ();
			JoinDateLabel.Text = thePlayer.creationDate.ToShortDateString ();

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
