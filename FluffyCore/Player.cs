using System;
using System.Collections.Generic;

namespace Fluffimax.Core
{
	public class Player {

		// match java
		public long id { get; set; }
		public String username { get; set; }
		public String pwd { get; set; }
		public String nickname { get; set; }
		public String userimage { get; set; }
		public DateTime creationDate { get; set; }
		public DateTime lastActiveDate { get; set; }
		public Boolean signedOn { get; set; }
		public Boolean isAdmin { get; set; }
		public int totalBunnies { get; set; }
		public int totalCarrotsFed { get; set; }
		public DateTime lastAwardDate { get; set; }
		public List<DateTime> RepeatPlayList { get; set; }
		public int carrotCount { get; set; }
		public bool FromServer { get; set; }
		public List<Bunny> Bunnies { get; set; }
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
				Server.RecordFeedBunny (theBuns);
				return theBuns.FeedBunny();
			} else {
				return false;
			}
		}

		public void GiveCarrots(int numCarrots) {
			carrotCount += numCarrots;
			Server.RecordGiveCarrots (numCarrots);
		}

		public bool BuyBunny(Bunny theBuns) {
			if (carrotCount >= theBuns.Price) {
				carrotCount -= theBuns.Price;
				Bunnies.Add (theBuns);
				totalBunnies++;
				RecentlyPurchased = true;
				UpdateBunnyOwnership (theBuns);
				Server.RecordBuyBunny (theBuns);
				return true;
			} else {
				return false;
			}
		}

		/*
		public bool GetBunny(Bunny theBuns) {
			Bunnies.Add (theBuns);
			totalBunnies++;
			return true;
		}
*/
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
			Server.RecordSellBunny(theBuns);
			return true;
		}
	}
}

