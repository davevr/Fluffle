using System;
using System.Collections.Generic;


namespace Fluffimax.Core
{
	public class Game
	{
		public static int kInitialCarrots = 250;
		public static int kBreedChance = 10;

		private static Player _player;
		private static List<Bunny>	_bunnyStore = new List<Bunny> ();
		public static Random Rnd = new Random();



		public static List<Bunny> BunnyStore {
			get { return _bunnyStore; }
		}

		public static Player CurrentPlayer {
			get { return _player; }
		}

		public static void InitBunnyStore() {
			BunnyStore.Add (Bunny.MakeRandomBunny ("minilop", 200));
			BunnyStore.Add (Bunny.MakeRandomBunny ("minilop", 500));
			BunnyStore.Add (Bunny.MakeRandomBunny ("angora", 1000));
			BunnyStore.Add (Bunny.MakeRandomBunny ("albino", 5000));
			BunnyStore.Add (Bunny.MakeRandomBunny ("flemish giant", 20000));
		}

		public static void InitGameForNewPlayer() {
			_player = new Player ();
			Bunny startBunny = Bunny.MakeRandomBunny ("minilop", 0);
			_player.BuyBunny (startBunny);
			Bunny startBunny2 = Bunny.MakeRandomBunny ("lop", 0);
			_player.BuyBunny (startBunny2);
		}

		public static bool LoadExistingPlayer() {
			bool didIt = false;

			return didIt;
		}

		public static bool SavePlayer() {
			bool didIt = false;

			return didIt;
		}
	}

	public class Player {
		private string _playerName;
		private long _id;
		private int _carrotCount;
		private List<Bunny> _bunnies;
		private int _totalCarrotsFed;
		private int _totalBunnies;

		public Player() {
			_carrotCount = Game.kInitialCarrots;
			_totalCarrotsFed = 0;
			_totalBunnies = 0;
			_bunnies = new List<Bunny> ();

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
		}

		public int TotalBunnies {
			get {
				return _totalBunnies;
			}
		}

		public List<Bunny>	Bunnies {
			get { return _bunnies; }
		}

		public int TotalCarrotsFed {
			get { return _totalCarrotsFed; }
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
				return true;
			} else {
				return false;
			}
		}

		public bool SellBunny(Bunny theBuns) {
			int salePrice = theBuns.CurrentValue;
			Bunnies.Remove (theBuns);
			_carrotCount += salePrice;
			//theBuns. = salePrice;
			return true;
		}
	}

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

		public Bunny() {
			_id = curId++;
			_feedState = 0;
			_size = 1;
			_price = 0;
			_lastBred = DateTime.Now;
			BunnyName = "Flopsy #" + _id.ToString();

			if (Game.Rnd.Next (100) < kBoyChance)
				_gender = "boy";
			else
				_gender = "girl";
		}

		public void UpdateLocation(int xLoc, int yLoc) {
			_xLoc = xLoc;
			_yLoc = yLoc;
		}

		public int HorizontalLoc {
			get { return _xLoc; }
		}

		public int VerticalLoc {
			get { return _yLoc; }
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

		}

		public string FurColor {
			get { return _furColor; }

		}

		public string EyeColor {
			get { return _eyeColor; }

		}

		public int BunnySize {
			get { return _size; }

		}

		public int FeedState {
			get { return _feedState; }

		}

		public double Progress {
			get { 
				return (double)FeedState / (double)(CarrotsForNextSize (BunnySize)); 
			}
		}

		public int Price {
			get { 
				return _price;
			}
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
		}

		public int CurrentValue {
			get {
				int multiplier = (int)Math.Pow (2, BunnySize - 1);
				return Price * multiplier;
			}
		}

		public static Bunny MakeRandomBunny(string breed, int price) {
			Bunny newBuns = new Bunny ();
			newBuns._bunnyBreed = breed;
			newBuns._eyeColor = "Black";
			newBuns._furColor = "White";

			newBuns._price = price;



			return newBuns;
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

					babyBuns._motherId = momBuns._id;
					babyBuns._fatherId = dadBuns._id;
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

