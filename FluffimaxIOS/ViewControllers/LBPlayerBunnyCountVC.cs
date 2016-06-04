using System;
using Foundation;
using UIKit;
using Fluffimax.Core;

namespace Fluffimax.iOS
{
	public partial class LBPlayerBunnyCountVC : UIViewController
	{
		private PlayerCountTableSource dataSource;

		public LBPlayerBunnyCountVC () : base ("LBPlayerBunnyCountVC", null)
		{
		}

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();
			// Perform any additional setup after loading the view, typically from a nib.
			DataTable.RegisterNibForCellReuse (UINib.FromName (PlayerCountCellView.Key, NSBundle.MainBundle), PlayerCountCellView.Key);
			dataSource = new PlayerCountTableSource ();
			DataTable.DataSource = dataSource;
			DataTable.RowHeight = 96;

		}

		public override void ViewWillAppear (bool animated)
		{
			base.ViewWillAppear (animated);

			TopConstraint.Constant = 42;// to do - height to titlebar
			Server.GetPlayerCountLB ((theList) => {
				InvokeOnMainThread(() => {
					dataSource.playerList = theList;
					DataTable.ReloadData();
				});
			});
		}

		public override void DidReceiveMemoryWarning ()
		{
			base.DidReceiveMemoryWarning ();
			// Release any cached data, images, etc that aren't in use.
		}
	}
}


