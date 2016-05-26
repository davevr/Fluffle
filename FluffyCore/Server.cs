using System;
using System.Collections.Generic;
using RestSharp;
using ServiceStack.Text;
using System.Net;

namespace Fluffimax.Core
{
	public delegate void Player_callback(Player theResult);
	public delegate void BunnyList_callback(List<Bunny> theResult);
	public delegate void string_callback(string theResult);
	public delegate void bool_callback(bool theResult);
	public delegate void Bunny_callback(Bunny theResult);
	public delegate void TossRecord_callback(TossRecord theResult);
	public delegate void intList_callback(List<int> theResult);

	public class Server
	{
		class TestDate {
			public DateTime  testDate;

		}

		private static RestClient apiClient;
		private static string apiBase = "/api/v1";
		private static string spriteFolderBase = "/images/sprites/";
		private static string profileFolderBase = "/images/profiles/";
		private static string localHostStr = "http://localhost:8080";
		private static string networkHostStr = "http://192.168.0.4:8080";
		private static string productionHostStr = "https://fluffle-1306.appspot.com";
		private static string serverBase =   localHostStr;
		private static string apiPath;
		public static string SpriteImagePath;
		public static string ProfileImagePath;
		private static string _uploadURL;
		private static string _catchURL;
		private static string _userImageURL;

		public static void InitServer () {
			JsConfig.DateHandler = JsonDateHandler.ISO8601; 
			String dateString = "2016-05-13T16:02:47.480-07:00";
			DateTime theDate = dateString.FromJson<DateTime> ();

			String testStr = "{\"testDate\":\"2016-05-13T16:02:51.923-07:00\"}";
			TestDate tester = testStr.FromJson<TestDate> ();

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

		public static void Login(string username, string pwd, Player_callback callback) {
			string fullURL = "login";

			RestRequest request = new RestRequest(fullURL, Method.POST);
			if (!String.IsNullOrEmpty(username))
				request.AddParameter ("username", username);
			if (!String.IsNullOrEmpty(pwd))
				request.AddParameter ("pwd", pwd);

			string bunnyStr = "{\"id\":6122080743456768}";
			Bunny bunTest = new Bunny ();
			bunTest.id = 234234234;
			bunTest.BreedName = "Holland Lop";
			string testStr = bunTest.ToJson ();
			Bunny theBuns = bunnyStr.FromJson<Bunny> ();
			Bunny testBackBun = testStr.FromJson<Bunny> ();
			string bunnyListStr = "[" + bunnyStr + "]";
			List<Bunny> bunList = bunnyListStr.FromJson<List<Bunny>> ();

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
			var request = new RestRequest("", Method.POST);
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




	}
}

