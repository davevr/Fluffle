using System;
using Fluffimax.Core;
using UIKit;
using Foundation;

namespace Fluffimax.iOS
{
	public partial class LBBunnySizeVC : UIViewController
	{
		private BunnySizeTableSource dataSource;

		public LBBunnySizeVC () : base ("LBBunnySizeVC", null)
		{

		}

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();
			// Perform any additional setup after loading the view, typically from a nib.
			DataTable.RegisterNibForCellReuse (UINib.FromName (BunnySizeCellView.Key, NSBundle.MainBundle), BunnySizeCellView.Key);
			dataSource = new BunnySizeTableSource ();
			DataTable.DataSource = dataSource;
			DataTable.RowHeight = 96;

		}

		public override void ViewWillAppear (bool animated)
		{
			base.ViewWillAppear (animated);

			TopConstraint.Constant = 42;// to do - height to titlebar
			Server.GetBunnySizeLB ((theList) => {
				InvokeOnMainThread(() => {
					dataSource.bunnyList = theList;
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


