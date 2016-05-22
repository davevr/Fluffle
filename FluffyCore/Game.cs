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
		public static long CurrentTossId { get; set; }


		public static List<Bunny> BunnyStore {
			get { return _bunnyStore; }
			set { _bunnyStore = value; }
		}

		public static Player CurrentPlayer {
			get { return _player; }
		}

		public static void InitBunnyStore() {
			Server.FetchStore((storeList) => {
				BunnyStore = storeList;
				});
		}

		public static void InitGrowthChart() {
			Server.FetchGrowthChart((growthChart) => {
				Bunny._growthStages = growthChart;
			});
		}

		public static void InitGameForNewPlayer(Player_callback callback) {
			Server.Login (null, null, (newPlayer) => {
				if (newPlayer != null)
					_player = newPlayer;
				else {
					// cannot load it - create a local one
					// todo:  handle lack of server case
				}
				callback(_player);
			});
		}

		public static DateTime Today {
			get {
				DateTime now = DateTime.Now;
				DateTime theDate = new DateTime (now.Year, now.Month, now.Day);
				return theDate;
			}
		}

		public static void LoadExistingPlayer(Player_callback callback) {
			bool didIt = false;

			string filePath = Path.Combine (
				Environment.GetFolderPath (Environment.SpecialFolder.Personal), "BunnyPlayer.json");

			if (File.Exists(filePath)) {
				string fileText = File.ReadAllText(filePath);
				_player = fileText.FromJson<Player>();
				if (_player != null) {
					string pwd = _player.pwd;
					if (string.IsNullOrEmpty(pwd))
						pwd = _player.username;
					Server.Login (_player.username, pwd, (serverCopy) => {
						if (serverCopy != null) {
							_player = serverCopy;
							_player.FromServer = true;
							callback(_player);
						} else {
							// player on device doesn't exist on server - delete for now
							// todo:  figure out correct action
							_player = null;
							callback(_player);
						}
					});
				} else {
					// just return local player
					_player.FromServer = false;
					callback(_player);
				}
			}

		}

		public static string MaybeRewardPlayer() {
			string rewardStr = null;

			DateTime curDate = Today;
			if (_player.lastAwardDate < curDate) {
				int daysPlayed = 0;
				// player is eligible for an award
				_player.RepeatPlayList.Add(curDate);
				if (_player.RepeatPlayList.Count > 10)
					_player.RepeatPlayList.RemoveAt (0);
				foreach (DateTime lastDate in _player.RepeatPlayList) {
					TimeSpan daysPast = curDate - lastDate;
					if (daysPast.TotalDays < 10)
						daysPlayed++;
				}
				int totalReward = _player.Bunnies.Count * daysPlayed;
				_player.carrotCount += totalReward;
				_player.lastAwardDate = curDate;

				rewardStr = string.Format ("You've played on {0} of the last 10 days.  You've earned {1} carrots - {0} for each bunny!", daysPlayed, totalReward);

			}
			return rewardStr;
		}

		public static bool MaybeStarveBunnies() {
			bool atLeastOne = false;
			DateTime todayDate = Today;

			foreach (Bunny curBunny in CurrentPlayer.Bunnies) {
				TimeSpan feedSpan = todayDate - curBunny.LastFeedDate;
				int daysStarved = (int)Math.Truncate (feedSpan.TotalDays);
				if (daysStarved > 0) {
					curBunny.StarveBunny (daysStarved);
					atLeastOne = true;
				}
			}

			return atLeastOne;
		}

		public static void SavePlayer(bool localOnly = false) {

			if (!localOnly) {
				// save to server
				Server.SavePlayer (CurrentPlayer, (SavePlayer) => {
					SavePlayerLocally();

				});
			} else {
				SavePlayerLocally ();
			}

			// save locally

		}

		private static bool SavePlayerLocally() {
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

