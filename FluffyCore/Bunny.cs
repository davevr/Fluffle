using System;
using System.Collections.Generic;

namespace Fluffimax.Core
{
	public class Bunny {
		public List<long>	Children { get; set; }
		public int HorizontalLoc { get; set; }
		public int VerticalLoc { get; set; }
		public long OriginalOwner { get; set; }
		public long CurrentOwner { get; set; }
		public DateTime LastFeedDate { get; set; }
		public DateTime LastBredDate { get; set; }
		public string BreedName { get; set; }
		public string EyeColorName { get; set; }
		public string FurColorName { get; set; }
		public long BreedId { get; set; }
		public long FurColorId { get; set; }
		public long EyeColorId { get; set; }
		public long MotherId { get; set; }
		public long FatherId { get; set; }
		public int BunnySize { get; set; }
		public int FeedState { get; set; }
		public int Price { get; set; }
		public string BunnyName { get; set; }
		public bool Female { get; set; }
		public long id { get; set; }
		public int TotalShares {get; set;}
		public string CurrentOwnerName { get; set; }
		public string CurrentOwnerImg { get; set; }
		public static List<int> _growthStages = null;
		public int Happiness { get; set;}


		public Bunny() {
			

			Children = new List<long> ();
		}

		public void UpdateLocation(int xLoc, int yLoc) {
			HorizontalLoc = xLoc;
			VerticalLoc = yLoc;
			Server.RecordBunnyLoc (this);
		}
			
		public double Progress {
			get { 
				return (double)FeedState / (double)(CarrotsForNextSize (BunnySize)); 
			}
		}

		public bool IncrementHappiness()
		{
			bool isNewlyHappy = false;

			if (Happiness < 100)
			{
				Happiness++;
				isNewlyHappy = Happiness == 100;
			}

			return isNewlyHappy;
		}

		public int CarrotsForNextSize(int curSize) {
			if (_growthStages != null)
				return _growthStages [curSize];
			else
				return 1;
		}

		public bool FeedBunny() {
			bool leveledUp = false;
			FeedState++;
			if (FeedState >= CarrotsForNextSize (BunnySize)) {
				FeedState = 0;
				BunnySize++;
				leveledUp = true;
			}
			LastFeedDate = Game.Today;
			return leveledUp;
		}

		public void StarveBunny(int numDays) {
			FeedState -= numDays;
			if (FeedState < 0)
				FeedState = 0;
		}

		public int TotalCarrots {
			get {
				int count = 0;
				int startLevel = BunnySize - 1;

				while (startLevel-- > 0) {
					count += CarrotsForNextSize (startLevel);
				}
				count += FeedState;

				return count;
			}
		}
			

		public int CurrentValue {
			get {
				int multiplier = (int)Math.Pow (2, BunnySize - 1);
				return Price * multiplier;
			}
		}




		public bool CanBreed() {
			if (BunnySize > 1) {
				if (!Female)
					return true;
				else {
					TimeSpan momBredTime = DateTime.Now - LastBredDate;
					if (momBredTime.TotalDays > 1)
						return true;
					else
						return false;
				}

			} else 
				return false;

		}

		public static bool BunniesCanBreed(Bunny momBuns, Bunny dadBuns) {
			if (momBuns.CanBreed() && 
				dadBuns.CanBreed() &&
				(momBuns.Female != dadBuns.Female) &&
				(momBuns.BunnySize == dadBuns.BunnySize) &&
				(momBuns.BreedId == dadBuns.BreedId))
				return true;
			else
				return false;
		}

		public string GetImageID() {
			return "minilop" + "_" + FurColorName.ToLower() + "_" + EyeColorName.ToLower();
		}

		public string GetProfileImage() {
			string imageName = Server.ProfileImagePath + GetImageID () + ".png";

			return imageName;
		}
	}
}

