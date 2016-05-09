using System;
using System.Collections.Generic;
using ServiceStack.Text;
using System.IO;

namespace Fluffimax.Core
{
	public class Game
	{
		public static int kInitialCarrots = 250;
		public static int kBreedChance = 100;

		private static Player _player;
		private static List<Bunny>	_bunnyStore = new List<Bunny> ();
		public static Random Rnd = new Random();
		public static Boolean IsNewGame { get; set; }



		public static List<Bunny> BunnyStore {
			get { return _bunnyStore; }
		}

		public static Player CurrentPlayer {
			get { return _player; }
		}

		public static void InitBunnyStore() {
			for (int i = 0; i < 20; i++) {
				BunnyStore.Add (Bunny.MakeRandomBunny ());
			}
		}

		public static void InitGameForNewPlayer() {
			_player = new Player ();
			Bunny startBunny = Bunny.MakeRandomBunny ();
			_player.GetBunny (startBunny);
			DateTime theDate = Today;
			_player.ListAwardDate = theDate;
			_player.RepeatList = new bool[] { false, false, false, false, false, false, false, false, false, false };
		}

		public static DateTime Today {
			get {
				DateTime now = DateTime.Now;
				DateTime theDate = new DateTime (now.Year, now.Month, now.Day);
				return theDate;
			}
		}

		public static bool LoadExistingPlayer() {
			bool didIt = false;

			string filePath = Path.Combine (
				Environment.GetFolderPath (Environment.SpecialFolder.Personal), "BunnyPlayer.json");

			if (File.Exists(filePath)) {
				string fileText = File.ReadAllText(filePath);
				_player = fileText.FromJson<Player>();
				if (_player != null) {
					
					didIt = true;
				}
			}

			return didIt;
		}

		public static bool SavePlayer() {
			bool didIt = false;
			string jsonString = CurrentPlayer.ToJson ();
			// save it
			string filePath = Path.Combine (
				Environment.GetFolderPath (Environment.SpecialFolder.Personal), "BunnyPlayer.json");
			System.IO.File.WriteAllText (filePath, jsonString);


			return true;
		}
	}


}

