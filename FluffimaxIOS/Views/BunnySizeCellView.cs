using System;

using Foundation;
using UIKit;
using Fluffimax.Core;
using SDWebImage;

namespace Fluffimax.iOS
{
	public partial class BunnySizeCellView : UITableViewCell
	{
		public static readonly NSString Key = new NSString ("BunnySizeCellView");
		public static readonly UINib Nib;

		static BunnySizeCellView ()
		{
			Nib = UINib.FromName ("BunnySizeCellView", NSBundle.MainBundle);
		}

		public BunnySizeCellView (IntPtr handle) : base (handle)
		{
		}

		public void ConformToRecord(Bunny theBuns, BunnySizeTableSource theSource) {
			
			string bunnyURL = theBuns.GetProfileImage ();

			BunnyName.Text = string.IsNullOrEmpty (theBuns.BunnyName) ? "Unnamed bunny" : theBuns.BunnyName;

			SizeLabel.Text = theBuns.BunnySize.ToString ();
			ProgressLabel.Text = string.Format ("{0}/{1}", theBuns.FeedState, theBuns.CarrotsForNextSize (theBuns.BunnySize));

			BunnyImg.SetImage (
				url: new NSUrl (bunnyURL), 
				placeholder: UIImage.FromBundle ("bunny.png")
			);

			if (theBuns.CurrentOwner == 0) {
				// no owner
				PlayerName.Text = "no owner";
				PlayerImg.Hidden = true;
			} else {
				PlayerName.Text = string.IsNullOrEmpty(theBuns.CurrentOwnerName) ? "unknown" : theBuns.CurrentOwnerName;
				PlayerImg.Hidden = false;
				if (String.IsNullOrEmpty(theBuns.CurrentOwnerImg))
				{
					PlayerImg.Image = UIImage.FromBundle("unknown_user");
				}
				else {
					PlayerImg.SetImage(
						url: new NSUrl(theBuns.CurrentOwnerImg),
						placeholder: UIImage.FromBundle("unknown_user")
					);
				}
			}
		}
	}
}
