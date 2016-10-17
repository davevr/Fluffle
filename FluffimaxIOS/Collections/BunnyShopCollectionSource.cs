using System;
using Foundation;
using UIKit;
using System.Collections.Generic;
using Fluffimax.Core;

namespace Fluffimax.iOS
{
	public class BunnyShopCollectionSource : UICollectionViewSource
	{
		private UITableView myTable;
		public BunnyShopViewController ShopView { get; set;}
		public List<Bunny> storeList;

		public BunnyShopCollectionSource ()
		{
		}

		public UITableView TableView {
			get { return myTable; }
		}

		public void SetStoreList(List<Bunny> bunList)
		{
			storeList = bunList;

		}


		public override nint NumberOfSections(UICollectionView collectionView)
		{
			return 1;
		}

		public override nint GetItemsCount(UICollectionView collectionView, nint section)
		{
			if (storeList != null)
				return storeList.Count;
			else
				return 0;
		}

		public override UICollectionViewCell GetCell(UICollectionView collectionView, NSIndexPath indexPath)
		{
			AdoptionViewCell cell = collectionView.DequeueReusableCell(AdoptionViewCell.Key, indexPath) as AdoptionViewCell;
			int item = indexPath.Row;

			cell.ConformToRecord(storeList[item], this);

			return cell;
		}




	}

	public class BunnyShopCollectionDelegate : UICollectionViewDelegate
	{
		public BunnyShopViewController Controller { get; set;}

		public override void ItemSelected(UICollectionView collectionView, NSIndexPath indexPath)
		{
			Bunny theBuns = null;
			int index = (int)indexPath.Item;
			if (index >= 0 && index < Controller.CurrentFilterSet.Count)
				theBuns = Controller.CurrentFilterSet[index];

			if (theBuns != null)
				Controller.MaybeBuyBunny(theBuns);
		}


	}
}

