using System;
using System.Collections.Generic;

namespace Fluffimax.Core
{
	public class Player {

		// match java
		public long id;
		public String username;
		public String nickname;
		public String userimage;
		public DateTime creationDate;
		public DateTime lastActiveDate;
		public Boolean signedOn;
		public Boolean isAdmin;
		public int totalBunnies;
		public int totalCarrotsFed;
		public DateTime lastAwardDate;
		public List<DateTime> RepeatPlayList;
		public int carrotCount;
		public bool FromServer;
		public List<Bunny> Bunnies;
		public bool RecentlyPurchased { get; set; }
		public Bunny BunnyBeingSold { get; set; }

		public Player() {
			carrotCount = Game.kInitialCarrots;
			totalCarrotsFed = 0;
			totalBunnies = 0;
			Bunnies = new List<Bunny> ();
			RepeatPlayList = new List<DateTime> ();
		}



		public bool FeedBunny(Bunny theBuns) {
			if (carrotCount > 0) {
				carrotCount--;
				return theBuns.FeedBunny();
			} else {
				return false;
			}
		}

		public void GiveCarrots(int numCarrots) {
			carrotCount += numCarrots;
		}

		public bool BuyBunny(Bunny theBuns) {
			if (carrotCount >= theBuns.Price) {
				carrotCount -= theBuns.Price;
				Bunnies.Add (theBuns);
				totalBunnies++;
				RecentlyPurchased = true;
				return true;
			} else {
				return false;
			}
		}

		public bool GetBunny(Bunny theBuns) {
			Bunnies.Add (theBuns);
			totalBunnies++;
			return true;
		}

		private void UpdateBunnyOwnership(Bunny theBuns) {
			theBuns.CurrentOwner = id;
			if (theBuns.OriginalOwner == 0)
				theBuns.OriginalOwner = id;
		}

		public bool SellBunny(Bunny theBuns) {
			int salePrice = theBuns.CurrentValue;
			Bunnies.Remove (theBuns);
			carrotCount += salePrice;
			//theBuns. = salePrice;
			return true;
		}
	}
}

