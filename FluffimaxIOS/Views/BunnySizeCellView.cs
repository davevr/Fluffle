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
			PlayerName.Text = string.IsNullOrEmpty (theBuns.CurrentOwnerName) ? "unknown" : theBuns.CurrentOwnerName;
			SizeLabel.Text = theBuns.BunnySize.ToString ();
			ProgressLabel.Text = string.Format ("{0}/{1}", theBuns.Progress, theBuns.CarrotsForNextSize (theBuns.BunnySize));

			BunnyImg.SetImage (
				url: new NSUrl (bunnyURL), 
				placeholder: UIImage.FromBundle ("bunny.png")
			);

			if (String.IsNullOrEmpty (theBuns.CurrentOwnerImg)) {
				PlayerImg.Image = UIImage.FromBundle ("unknown_user");
			} else {
				PlayerImg.SetImage (
					url: new NSUrl (theBuns.CurrentOwnerImg), 
					placeholder: UIImage.FromBundle ("unknown_user")
				);
			}
		}
	}
}
