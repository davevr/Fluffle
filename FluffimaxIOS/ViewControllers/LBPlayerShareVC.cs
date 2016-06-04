using System;
using Fluffimax.Core;
using UIKit;
using Foundation;

namespace Fluffimax.iOS
{
	public partial class LBPlayerShareVC : UIViewController
	{
		private PlayerShareTableSource dataSource;

		public LBPlayerShareVC () : base ("LBPlayerShareVC", null)
		{
			
		}

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();
			// Perform any additional setup after loading the view, typically from a nib.
			DataTable.RegisterNibForCellReuse (UINib.FromName (PlayerShareCellView.Key, NSBundle.MainBundle), PlayerShareCellView.Key);
			dataSource = new PlayerShareTableSource ();
			DataTable.DataSource = dataSource;
			DataTable.RowHeight = 96;

		}

		public override void ViewWillAppear (bool animated)
		{
			base.ViewWillAppear (animated);

			TopConstraint.Constant = 42;// to do - height to titlebar
			Server.GetPlayerShareLB ((theList) => {
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


