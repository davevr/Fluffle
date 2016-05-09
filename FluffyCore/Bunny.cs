using System;
using System.Collections.Generic;

namespace Fluffimax.Core
{
	public class Bunny {
		private static long curId = 1;
		private static int kBoyChance = 30;
		private string _bunnyName;
		private string _bunnyBreed;
		private string _gender;
		private string _furColor;
		private string _eyeColor;
		private int _size;
		private int _feedState;
		private long _id;
		private long _motherId;
		private long _fatherId;
		private List<long>	_childList;
		private int _price ;
		private int _xLoc;
		private int _yLoc;
		private DateTime _lastBred;

		private int[] _growthStages = new int[] {1, 10,50,100,200,300,500,750,1000,1250,1500,1750,2000,2500,3000,3500,5000,7500,10000};

		private static string[] _breeds = new string[] {"lop", "mini-lop", "flemish giant"};
		private static int[] _breedChance = new int[] { 10, 10, 1 };
		private static string[] _furColors = new string[] {"white", "tan", "brown", "black", "pink"};
		private static int[] _furChance = new int[] {10, 15, 15, 5, 1};
		private static string[] _eyeColors = new string[] {"red", "black", "blue", "brown", "purple"};
		private static int[] _eyeChance = new int[] {5, 20, 10, 20, 1};

		public Bunny() {
			_id = curId++;
			_feedState = 0;
			_size = 1;
			_price = 0;
			_lastBred = DateTime.Now.AddDays (-5);
			BunnyName = "Flopsy #" + _id.ToString();

			if (Game.Rnd.Next (100) < kBoyChance)
				_gender = "boy";
			else
				_gender = "girl";

			_childList = new List<long> ();
		}

		public void UpdateLocation(int xLoc, int yLoc) {
			_xLoc = xLoc;
			_yLoc = yLoc;
		}

		public long ID {
			get { return _id; }
			set { _id = value; }
		}

		public long MotherID {
			get { return _motherId; }
			set { _motherId = value; }
		}

		public long FatherID {
			get { return _fatherId; }
			set { _fatherId = value; }
		}

		public List<long> Children {
			get { return _childList; }
			set { _childList = value; }
		}

		public int HorizontalLoc {
			get { return _xLoc; }
			set { _xLoc = value; }
		}

		public int VerticalLoc {
			get { return _yLoc; }
			set { _yLoc = value; }
		}

		public string BunnyName {
			get { return _bunnyName; }
			set {
				_bunnyName = value;
			}
		}

		public string BunnyBreed {
			get { return _bunnyBreed; }
			set {
				_bunnyBreed = value;
			}
		}

		public string Gender {
			get { return _gender; }
			set {  _gender = value; }
		}

		public string FurColor {
			get { return _furColor; }
			set { _furColor = value; }
		}

		public string EyeColor {
			get { return _eyeColor; }
			set { _eyeColor = value; }
		}

		public int BunnySize {
			get { return _size; }
			set { _size = value; }
		}

		public int FeedState {
			get { return _feedState; }
			set { _feedState = value; }
		}

		public double Progress {
			get { 
				return (double)FeedState / (double)(CarrotsForNextSize (BunnySize)); 
			}
		}

		public int Price {
			get {  return _price; }
			set { _price = value; }
		}

		public int CarrotsForNextSize(int curSize) {
			return _growthStages [curSize];
		}

		public bool FeedBunny() {
			bool leveledUp = false;
			_feedState++;
			if (FeedState >= CarrotsForNextSize (BunnySize)) {
				_feedState = 0;
				_size++;
				leveledUp = true;
			}
			return leveledUp;
		}

		public void StarveBunny() {
			_feedState--;
			if (_feedState < 0)
				_feedState = 0;
		}

		public int TotalCarrots {
			get {
				int count = 0;
				int startLevel = _size - 1;

				while (startLevel-- > 0) {
					count += CarrotsForNextSize (startLevel);
				}
				count += FeedState;

				return count;
			}
		}

		public DateTime LastBred {
			get { return _lastBred; }
			set { _lastBred = value; }
		}

		public int CurrentValue {
			get {
				int multiplier = (int)Math.Pow (2, BunnySize - 1);
				return Price * multiplier;
			}
		}



		public static Bunny MakeRandomBunny() {
			Bunny newBuns = new Bunny ();
			double basePrice = 16;
			double totalChance = 1;
			double curChance = 1;

			newBuns._bunnyBreed = SelectFromList (_breeds, _breedChance, out curChance);
			totalChance *= curChance;

			newBuns._eyeColor = SelectFromList (_eyeColors, _eyeChance, out curChance);
			totalChance *= curChance;

			newBuns._furColor = SelectFromList (_furColors, _furChance, out curChance);
			totalChance *= curChance;


			newBuns._price = (int)(basePrice / totalChance);

			return newBuns;
		}

		private static string SelectFromList(string[] stringList, int[] chanceList, out double chance) {
			int listTotal = 0;
			chance = 1;

			foreach (int curChance in chanceList) {
				listTotal += curChance;
			}

			int whichIndex = Game.Rnd.Next (0, listTotal);
			int curCount = 0;

			for (int i = 0; i < chanceList.Length; i++) {
				curCount += chanceList [i];
				if (curCount > whichIndex) {
					chance = (double)chanceList [i] / (double)listTotal;
					return stringList [i];
				}
			}

			return "";
		}

		public static Bunny BreedBunnies(Bunny momBuns, Bunny dadBuns) {
			Bunny babyBuns = null;

			if (BunniesCanBreed(momBuns,dadBuns)) {
				if (Game.Rnd.Next(0,100) < Game.kBreedChance) {
					// breed them!
					babyBuns = new Bunny();
					babyBuns.BunnyBreed = momBuns.BunnyBreed;

					if (Game.Rnd.Next (10) < 5)
						babyBuns._furColor = momBuns.FurColor;
					else
						babyBuns._furColor = dadBuns.FurColor;

					if (Game.Rnd.Next (10) < 5)
						babyBuns._eyeColor = momBuns.EyeColor;
					else
						babyBuns._eyeColor = dadBuns.EyeColor;

					if (dadBuns.Gender == "boy") {
						babyBuns._motherId = momBuns._id;
						babyBuns._fatherId = dadBuns._id;
					} else {
						babyBuns._motherId = dadBuns._id;
						babyBuns._fatherId = momBuns._id;
					}
					momBuns._childList.Add (babyBuns._id);
					dadBuns._childList.Add (babyBuns._id);
					dadBuns._lastBred = DateTime.Now;
					momBuns._lastBred = DateTime.Now;

				}
			}

			return babyBuns;
		}

		public bool CanBreed() {
			if (BunnySize > 1) {
				if (Gender == "boy")
					return true;
				else {
					TimeSpan momBredTime = DateTime.Now - LastBred;
					if (momBredTime.TotalDays > 1)
						return true;
					else
						return false;
				}

			} else 
				return false;

		}

		public static bool BunniesCanBreed(Bunny momBuns, Bunny dadBuns) {
			TimeSpan momBredTime = DateTime.Now - momBuns.LastBred;

			if (momBuns.CanBreed() && 
				dadBuns.CanBreed() &&
				(momBuns.Gender != dadBuns.Gender) &&
				(momBuns.BunnySize == dadBuns.BunnySize) &&
				(momBuns.BunnyBreed == dadBuns.BunnyBreed))
				return true;
			else
				return false;
		}
	}
}

