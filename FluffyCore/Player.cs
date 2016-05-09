using System;
using System.Collections.Generic;

namespace Fluffimax.Core
{
	public class Player {
		private string _playerName;
		private long _id;
		private int _carrotCount;
		private List<Bunny> _bunnies;
		private int _totalCarrotsFed;
		private int _totalBunnies;
		public bool RecentlyPurchased { get; set; }
		public DateTime LastAwardDate { get; set; }
		public List<DateTime> RepeatPlayList { get; set; }
		public bool FromServer { get; set; }
		public string ImageURL { get; set; }

		public Player() {
			_carrotCount = Game.kInitialCarrots;
			_totalCarrotsFed = 0;
			_totalBunnies = 0;
			_bunnies = new List<Bunny> ();
			RepeatPlayList = new List<DateTime> ();
		}

		public string PlayerName {
			get {
				return _playerName;
			}
			set { _playerName = value; }
		}

		public int CarrotCount {
			get {
				return _carrotCount;
			}

			set {
				_carrotCount = value;
			}
		}

		public int TotalBunnies {
			get {
				return _totalBunnies;
			}

			set {
				_totalBunnies = value;
			}
		}

		public List<Bunny>	Bunnies {
			get { return _bunnies; }
			set { _bunnies = value; }
		}

		public long	ID {
			get { return _id; }
			set { _id = value; }
		}

		public int TotalCarrotsFed {
			get { return _totalCarrotsFed; }
			set { _totalCarrotsFed = value; }
		}

		public bool FeedBunny(Bunny theBuns) {
			if (_carrotCount > 0) {
				_carrotCount--;
				return theBuns.FeedBunny();
			} else {
				return false;
			}
		}

		public void GiveCarrots(int numCarrots) {
			_carrotCount += numCarrots;
		}

		public bool BuyBunny(Bunny theBuns) {
			if (_carrotCount >= theBuns.Price) {
				_carrotCount -= theBuns.Price;
				Bunnies.Add (theBuns);
				_totalBunnies++;
				RecentlyPurchased = true;
				return true;
			} else {
				return false;
			}
		}

		public bool GetBunny(Bunny theBuns) {
			Bunnies.Add (theBuns);
			_totalBunnies++;
			return true;
		}

		private void UpdateBunnyOwnership(Bunny theBuns) {
			theBuns.CurrentOwner = ID;
			if (theBuns.OriginalOwner == 0)
				theBuns.OriginalOwner = ID;
		}

		public bool SellBunny(Bunny theBuns) {
			int salePrice = theBuns.CurrentValue;
			Bunnies.Remove (theBuns);
			_carrotCount += salePrice;
			//theBuns. = salePrice;
			return true;
		}
	}
}

