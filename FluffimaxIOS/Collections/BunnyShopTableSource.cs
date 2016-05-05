using System;
using Foundation;
using UIKit;
using System.Collections.Generic;
using Fluffimax.Core;

namespace Fluffimax.iOS
{
	public class BunnyShopTableSource : UITableViewDataSource
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
			return 1;
		}

		public override string TitleForHeader (UITableView tableView, nint section)
		{
			return "Bunnies for sale";
		}


		public override UITableViewCell GetCell (UITableView tableView, NSIndexPath indexPath)
		{
			BunnyCellView cell = tableView.DequeueReusableCell(BunnyCellView.Key, indexPath) as BunnyCellView;

			cell.ConformToRecord(Game.BunnyStore[indexPath.Row], this);


			return cell;
		}
	}
}

