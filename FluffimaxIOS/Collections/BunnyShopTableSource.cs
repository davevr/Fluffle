using System;
using Foundation;
using UIKit;
using System.Collections.Generic;
using Fluffimax.Core;

namespace Fluffimax.iOS
{
	public class BunnyShopTableSource : UITableViewSource
	{
		private UITableView myTable;
		public BunnyShopViewController ShopView { get; set;}

		public BunnyShopTableSource ()
		{
		}

		public UITableView TableView {
			get { return myTable; }
		}
			


		public override nint RowsInSection (UITableView tableview, nint section)
		{
			nint numBuns = 1;

			numBuns = Game.BunnyStore.Count;

			return numBuns;
		}



		public override nint NumberOfSections (UITableView tableView)
		{
			return 2;
		}

		public override string TitleForHeader (UITableView tableView, nint section)
		{
			return "Size " + section;
		}



		public override UITableViewCell GetCell (UITableView tableView, NSIndexPath indexPath)
		{
			BunnyCellView cell = tableView.DequeueReusableCell(BunnyCellView.Key, indexPath) as BunnyCellView;

			cell.ConformToRecord(Game.BunnyStore[indexPath.Row], this);


			return cell;
		}
	}

	public class BunnyShopTableDelegate : UITableViewDelegate
	{
		public override nfloat GetHeightForHeader(UITableView tableView, nint section)
		{
			if (section == 0)
				return 20;
			else
				return 20;
		}

		public override nfloat GetHeightForFooter(UITableView tableView, nint section)
		{
			return 0;
		}

		public override nfloat EstimatedHeightForHeader(UITableView tableView, nint section)
		{
			if (section == 0)
				return 20;
			else
				return 20;
		}

		public override nfloat EstimatedHeightForFooter(UITableView tableView, nint section)
		{
			return 0;
		}

	}
}

