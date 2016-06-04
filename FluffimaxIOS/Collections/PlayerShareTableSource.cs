using System;
using Foundation;
using UIKit;
using System.Collections.Generic;
using Fluffimax.Core;


namespace Fluffimax.iOS
{
	public class PlayerShareTableSource : UITableViewDataSource
	{
		public UITableView myTable {get; set;}

		public List<Player> playerList;

		public PlayerShareTableSource ()
		{
		}

		public UITableView TableView {
			get { return myTable; }
		}



		public override nint RowsInSection (UITableView tableview, nint section)
		{
			nint numPlayers = 0;

			if (playerList != null)
				numPlayers = playerList.Count;

			return numPlayers;
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
			PlayerShareCellView cell = tableView.DequeueReusableCell(PlayerShareCellView.Key, indexPath) as PlayerShareCellView;

			cell.ConformToRecord(playerList[indexPath.Row], this);


			return cell;
		}
	}
}

