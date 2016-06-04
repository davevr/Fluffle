using System;
using Foundation;
using UIKit;
using System.Collections.Generic;
using Fluffimax.Core;


namespace Fluffimax.iOS
{
	public class PlayerCountTableSource : UITableViewDataSource
	{
		public UITableView myTable {get; set;}

		public List<Player> playerList;

		public PlayerCountTableSource ()
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
			PlayerCountCellView cell = tableView.DequeueReusableCell(PlayerCountCellView.Key, indexPath) as PlayerCountCellView;

			cell.ConformToRecord(playerList[indexPath.Row], this);


			return cell;
		}
	}
}

