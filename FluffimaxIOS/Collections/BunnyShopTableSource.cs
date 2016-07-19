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
		public Dictionary<int, List<Bunny>> bunMap = null;

		public BunnyShopTableSource ()
		{
		}

		public UITableView TableView {
			get { return myTable; }
		}

		public void SetStoreList(List<Bunny> bunList)
		{
			bunMap = new Dictionary<int, List<Bunny>>();

			foreach (Bunny curBuns in bunList)
			{
				int curSize = curBuns.BunnySize;

				if (!bunMap.ContainsKey(curSize))
				{
					List<Bunny> newList = new List<Bunny>();
					bunMap[curSize] = newList;
				}

				bunMap[curSize].Add(curBuns);
			}

			// now sort them by breed
			for (int i = 1; i <= 10; i++)
			{
				if (bunMap.ContainsKey(i))
				{
					bunMap[i].Sort();
				}
			}

		}

		public override nint RowsInSection (UITableView tableview, nint section)
		{
			nint numBuns = 0;

			if (bunMap.ContainsKey((int)section + 1))
				numBuns = bunMap[(int)section + 1].Count;

			return numBuns;
		}



		public override nint NumberOfSections (UITableView tableView)
		{
			if (bunMap != null)
				return 10;
			else
				return 0;
		}

		public override string TitleForHeader (UITableView tableView, nint section)
		{
			return Bunny.SizeString((int)section + 1) + " bunnies";
		}



		public override UITableViewCell GetCell (UITableView tableView, NSIndexPath indexPath)
		{
			BunnyCellView cell = tableView.DequeueReusableCell(BunnyCellView.Key, indexPath) as BunnyCellView;
			int size = indexPath.Section + 1;
			int item = indexPath.Row;

			if (bunMap.ContainsKey(size)) 
				cell.ConformToRecord(bunMap[size][item], this);

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

