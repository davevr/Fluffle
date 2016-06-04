using System;
using Foundation;
using UIKit;
using System.Collections.Generic;
using Fluffimax.Core;

namespace Fluffimax.iOS
{
	public class BunnySizeTableSource : UITableViewDataSource
	{
		public UITableView myTable {get; set;}

		public List<Bunny> bunnyList;

		public BunnySizeTableSource ()
		{
		}

		public UITableView TableView {
			get { return myTable; }
		}



		public override nint RowsInSection (UITableView tableview, nint section)
		{
			nint numBuns = 0;

			if (bunnyList != null)
				numBuns = bunnyList.Count;

			return numBuns;
		}



		public override nint NumberOfSections (UITableView tableView)
		{
			return 1;
		}

		public override string TitleForHeader (UITableView tableView, nint section)
		{
			return "";
		}


		public override UITableViewCell GetCell (UITableView tableView, NSIndexPath indexPath)
		{
			BunnySizeCellView cell = tableView.DequeueReusableCell(BunnySizeCellView.Key, indexPath) as BunnySizeCellView;

			cell.ConformToRecord(bunnyList[indexPath.Row], this);


			return cell;
		}


	}
}

