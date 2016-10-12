using System;
using System.Collections.Generic;
using RestSharp;
using ServiceStack.Text;
using System.Net;
using System.IO;

namespace Fluffimax.Core
{
	public delegate void Player_callback(Player theResult);
	public delegate void BunnyList_callback(List<Bunny> theResult);
	public delegate void PlayerList_callback(List<Player> theResult);
	public delegate void string_callback(string theResult);
	public delegate void bool_callback(bool theResult);
	public delegate void Bunny_callback(Bunny theResult);
	public delegate void TossRecord_callback(TossRecord theResult);
	public delegate void intList_callback(List<int> theResult);
	public delegate void int_callback(int theResult);

	public class Server
	{
		class TestDate {
			public DateTime  testDate;

		}

		private static RestClient apiClient;
		private static string apiBase = "/api/v1";
		private static string spriteFolderBase = "/images/sprites/";
		private static string profileFolderBase = "/images/profiles/";
		private static string localHostStr = "http://localhost:8080"; // "http://localhost:8080"; // "http://10.0.3.2:8080"
		private static string networkHostStr = "http://192.168.0.22:8080";
		private static string productionHostStr = "http://fluffle.it";
		private static string serverBase = localHostStr; 
		private static string apiPath;
		public static string SpriteImagePath;
		public static string ProfileImagePath;
		private static string _uploadURL;
		private static string _catchURL;
		private static string _userImageURL;

		public static bool IsOnline{ get; set; }

		public static bool IsLocal {
			get {
				return serverBase == localHostStr || serverBase == networkHostStr;
			}
		}

		public static void InitServer () {
			JsConfig.DateHandler = JsonDateHandler.ISO8601; 

			if (serverBase == localHostStr)
				System.Console.WriteLine("Using Local Server");
			else if (serverBase == productionHostStr)
				System.Console.WriteLine("Using Production Server");
			else if (serverBase == networkHostStr)
				System.Console.WriteLine("Using LAN Server");

			apiPath = serverBase + apiBase;
			SpriteImagePath = serverBase + spriteFolderBase;
			ProfileImagePath = serverBase + profileFolderBase;
			
			apiClient = new RestClient(apiPath);
			apiClient.CookieContainer = new CookieContainer();

			// todo:  other init, if any
		}

		public static void IsAlive(bool_callback callback) {
			string fullURL = "admin/status";

			RestRequest request = new RestRequest(fullURL, Method.GET);
			request.Timeout = 5000;
			apiClient.ExecuteAsync(request, (response) =>
				{
					if (response.StatusCode == HttpStatusCode.OK) {
						bool alive = response.Content.FromJson<bool>();
						callback(alive);
					}
					else
						callback(false);
				});
		}

		public static void Login(string username, string pwd, Player_callback callback) {
			string fullURL = "login";

			RestRequest request = new RestRequest(fullURL, Method.POST);
			if (!String.IsNullOrEmpty(username))
				request.AddParameter ("username", username);
			if (!String.IsNullOrEmpty(pwd))
				request.AddParameter ("pwd", pwd);

			apiClient.ExecuteAsync<Player>(request, (response) =>
				{
					Player newUser = response.Data;
					if (newUser!= null)
					{
						callback(newUser);
					}
					else
						callback(null);
				});
		}

		public static void RecordPurchase(string productStr, string store, string receipt, int_callback callback) {
			string fullURL = "store";
			RestRequest request = new RestRequest(fullURL, Method.POST);
		
			request.AddParameter ("receipt-data", receipt);
			request.AddParameter ("store", store);
			request.AddParameter ("product", productStr);


			apiClient.ExecuteAsync<int>(request, (response) => 
				{
					int result = response.Data;
                    Game.CurrentPlayer.carrotCount += result;
					callback(result);

				});
		}

		public static void UpdateUsername(string newName, string_callback callback) {
			string fullURL = "login";
			RestRequest request = new RestRequest(fullURL, Method.PUT);

			request.AddParameter ("username", newName);

			apiClient.ExecuteAsync(request, (response) => 
				{
					if (response.StatusCode == HttpStatusCode.OK)
						callback(null);
					else
						callback(response.Content);

				});
		}

		public static void UpdatePassword(string newPwd, string_callback callback) {
			string fullURL = "login";
			RestRequest request = new RestRequest(fullURL, Method.PUT);

			request.AddParameter ("pwd", newPwd);

			apiClient.ExecuteAsync(request, (response) => 
				{
					if (response.StatusCode == HttpStatusCode.OK)
						callback(null);
					else
						callback(response.Content);

				});
		}

		public static void UpdateNickname(string newName, string_callback callback) {
			string fullURL = "player";
			RestRequest request = new RestRequest(fullURL, Method.PUT);

			request.AddParameter ("nickname", newName);

			apiClient.ExecuteAsync(request, (response) => 
				{
					if (response.StatusCode == HttpStatusCode.OK)
						callback(null);
					else
						callback(response.Content);

				});
		}

		public static void GetCarrotCount(int_callback callback) {
			string fullURL = "player";

			RestRequest request = new RestRequest(fullURL, Method.GET);
			request.AddParameter ("carrotcount", true);

			apiClient.ExecuteAsync<int>(request, (response) =>
				{
					int carrotCount = response.Data;
					Game.CurrentPlayer.carrotCount = carrotCount;

					callback( carrotCount);
				});
		}

		public static void GetMarketPrice(long bunnyId, int_callback callback) {
			string fullURL = "store";

			RestRequest request = new RestRequest(fullURL, Method.GET);
			request.AddParameter ("bunnyid", bunnyId);

			apiClient.ExecuteAsync<int>(request, (response) =>
				{
					int thePrice = response.Data;

					callback( thePrice);
				});
		}

		public static void SellBunny(long bunnyId, int_callback callback) {
			string fullURL = "store";

			RestRequest request = new RestRequest(fullURL, Method.POST);
			request.AddParameter ("bunnyid", bunnyId);

			apiClient.ExecuteAsync<int>(request, (response) =>
				{
					int soldPrice = response.Data;

					callback( soldPrice);
				});
		}


		public static void CreatePlayer(Player_callback callback)
		{
			string fullURL = "player";

			RestRequest request = new RestRequest(fullURL, Method.POST);

			apiClient.ExecuteAsync<Player>(request, (response) =>
				{
					Player newUser = response.Data;
					if (newUser!= null)
					{
						callback(newUser);
					}
					else
						callback(null);
				});
		}

		public static void LoadPlayer(long playerId, Player_callback callback)
		{
			string fullURL = "player";


			RestRequest request = new RestRequest(fullURL, Method.GET);
			request.AddParameter ("id", playerId);

			apiClient.ExecuteAsync<Player>(request, (response) =>
				{
					Player newUser = response.Data;
					if (newUser!= null)
					{
						callback(newUser);
					}
					else
						callback(null);
				});
		}

		public static void SavePlayer(Player curPlayer, Player_callback callback)
		{
			string fullURL = "player";


			RestRequest request = new RestRequest(fullURL, Method.PUT);
			request.AddBody (curPlayer);

			apiClient.ExecuteAsync<Player>(request, (response) =>
				{
					Player newUser = response.Data;
					if (newUser!= null)
					{
						callback(newUser);
					}
					else
						callback(null);
				});
		}

		public static void FetchStore(BunnyList_callback callback)
		{
			string fullURL = "store";

			RestRequest request = new RestRequest(fullURL, Method.GET);

			apiClient.ExecuteAsync<List<Bunny>>(request, (response) =>
				{
					List<Bunny> bunsList = response.Data;
					if (bunsList!= null)
					{
						callback(bunsList);
					}
					else
						callback(null);
				});
		}

		public static void RecordPlayerAction(string actionName, object valueObj)
		{
			string fullURL = "player";

			RestRequest request = new RestRequest(fullURL, Method.PUT);
			request.AddParameter (actionName, valueObj);

			apiClient.ExecuteAsync(request, (response) =>
				{
					// todo:  check for error?
				});
		}

		public static void RecordFeedBunny(Bunny theBuns)
		{
			RecordPlayerAction ("feed", theBuns.id);
		}

		public static void RecordPetBunny(Bunny theBuns)
		{
			RecordPlayerAction("pet", theBuns.id);
		}

		public static void RecordUserImage(string theImageURL)
		{
			RecordPlayerAction("image", theImageURL);
		}


		public static void RecordGiveCarrots(int numCarrots)
		{
			RecordPlayerAction ("givecarrots", numCarrots);
		}

		public static void RecordBuyBunny(Bunny theBuns)
		{
			RecordPlayerAction ("buybunny", theBuns.id);
		}

		public static void RecordSellBunny(Bunny theBuns)
		{
			RecordPlayerAction ("sellbunny", theBuns.id);
		}

		public static void RecordRenameBunny(Bunny theBuns)
		{
			string fullURL = "bunny";

			RestRequest request = new RestRequest(fullURL, Method.PUT);
			request.AddParameter ("renamebunny", theBuns.id);
			request.AddParameter ("name", theBuns.BunnyName);

			apiClient.ExecuteAsync (request, null);
		}

		public static void RecordBunnyLoc(Bunny theBuns)
		{
			string fullURL = "bunny";

			RestRequest request = new RestRequest(fullURL, Method.PUT);
			request.AddParameter ("move", theBuns.id);
			request.AddParameter ("xloc", theBuns.HorizontalLoc);
			request.AddParameter ("yloc", theBuns.VerticalLoc);

			apiClient.ExecuteAsync (request, null);
		}

		public static void FetchGrowthChart(intList_callback callback)
		{
			string fullURL = "game";

			RestRequest request = new RestRequest(fullURL, Method.GET);
			request.AddParameter ("growthchart", true);

			apiClient.ExecuteAsync<List<int>>(request, (response) =>
				{
					List<int> sizeChart = response.Data;
					if (sizeChart!= null)
					{
						callback(sizeChart);
					}
					else
						callback(null);
				});
		}

		public static void StartToss(long bunnyId, int price, TossRecord_callback callback)
		{
			string fullURL = "toss";

			RestRequest request = new RestRequest(fullURL, Method.POST);
			request.AddParameter("bunny", bunnyId);
			request.AddParameter("price", price);

			apiClient.ExecuteAsync<TossRecord>(request, (response) =>
				{
					callback(response.Data);
				});
		}

		public static void CatchToss(long tossid, Bunny_callback callback)
		{
			var request = new RestRequest("catch", Method.POST);
			request.AddParameter("toss", tossid);


			apiClient.ExecuteAsync<Bunny>(request, (response) =>
				{
					callback(response.Data);
				});
		}

		public static void GetTossStatus(long tossId, TossRecord_callback callback)
		{
			string fullURL = "toss";

			RestRequest request = new RestRequest(fullURL, Method.GET);
			request.AddParameter("tossid", tossId);

			apiClient.ExecuteAsync<TossRecord>(request, (response) =>
				{
					callback(response.Data);
				});

		}

		public static void GetBunnySizeLB(BunnyList_callback callback)
		{
			string fullURL = "leaderboards";

			RestRequest request = new RestRequest(fullURL, Method.GET);
			request.AddParameter("bysize", true);

			apiClient.ExecuteAsync<List<Bunny>>(request, (response) =>
				{
					callback(response.Data);
				});
		}

		public static void GetBunnySpreadLB(BunnyList_callback callback)
		{
			string fullURL = "leaderboards";

			RestRequest request = new RestRequest(fullURL, Method.GET);
			request.AddParameter("byspread", true);

			apiClient.ExecuteAsync<List<Bunny>>(request, (response) =>
				{
					callback(response.Data);
				});
		}

		public static void GetPlayerCountLB(PlayerList_callback callback)
		{
			string fullURL = "leaderboards";

			RestRequest request = new RestRequest(fullURL, Method.GET);
			request.AddParameter("bycount", true);

			apiClient.ExecuteAsync<List<Player>>(request, (response) =>
				{
					callback(response.Data);
				});
		}

		public static void GetPlayerShareLB(PlayerList_callback callback)
		{
			string fullURL = "leaderboards";

			RestRequest request = new RestRequest(fullURL, Method.GET);
			request.AddParameter("byshares", true);

			apiClient.ExecuteAsync<List<Player>>(request, (response) =>
				{
					callback(response.Data);
				});
		}

		public static void UploadImage(Stream photoStream, string fileName, string_callback callback)
		{
			string uploadURL = GetImageUploadURL();
			int pathSplit = uploadURL.IndexOf("/", 10);
			string appPath = uploadURL.Substring(0, pathSplit);
            if (appPath == "http://MasterSheep:8080")
                appPath = networkHostStr;
			string requestPath = uploadURL.Substring(pathSplit);
			RestClient onetimeClient = new RestClient(appPath);
			//onetimeClient.CookieContainer = apiClient.CookieContainer;
			var request = new RestRequest(requestPath, Method.POST);
			request.AddHeader("Accept", "*/*");
			request.AddFile("file", ReadToEnd(photoStream), fileName, "image/jpeg");

			onetimeClient.ExecuteAsync(request, (response) =>
				{
					if (response.StatusCode == HttpStatusCode.OK)
					{
                        string urlStr = response.Content;
                        urlStr = urlStr.Replace("http://0.0.0.0:8080", networkHostStr);
                        callback(urlStr);
					}
					else
					{
						//error ocured during upload
						callback(null);
					}
				});
		}

		public static string GetImageUploadURL()
		{
			RestRequest request = new RestRequest("uploadImage", Method.GET);
			IRestResponse response = apiClient.Execute(request);
			return response.Content;
		}


		//method for converting stream to byte[]
		public static byte[] ReadToEnd(System.IO.Stream stream)
		{
			long originalPosition = stream.Position;
			stream.Position = 0;

			try
			{
				byte[] readBuffer = new byte[4096];

				int totalBytesRead = 0;
				int bytesRead;

				while ((bytesRead = stream.Read(readBuffer, totalBytesRead, readBuffer.Length - totalBytesRead)) > 0)
				{
					totalBytesRead += bytesRead;

					if (totalBytesRead == readBuffer.Length)
					{
						int nextByte = stream.ReadByte();
						if (nextByte != -1)
						{
							byte[] temp = new byte[readBuffer.Length * 2];
							Buffer.BlockCopy(readBuffer, 0, temp, 0, readBuffer.Length);
							Buffer.SetByte(temp, totalBytesRead, (byte)nextByte);
							readBuffer = temp;
							totalBytesRead++;
						}
					}
				}

				byte[] buffer = readBuffer;
				if (readBuffer.Length != totalBytesRead)
				{
					buffer = new byte[totalBytesRead];
					Buffer.BlockCopy(readBuffer, 0, buffer, 0, totalBytesRead);
				}
				return buffer;
			}
			finally
			{
				stream.Position = originalPosition;
			}
		}




	}
}

