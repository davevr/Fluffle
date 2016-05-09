using System;
using System.Collections.Generic;
using RestSharp;
using ServiceStack.Text;

namespace Fluffimax.Core
{
	public class Server
	{
		public delegate void Player_callback(Player theResult);
		public delegate void BunnyList_callback(List<Bunny> theResult);

		private static RestClient apiClient;
		private static string localHostStr = "http://localhost:8080/api/v1";
		private static string networkHostStr = "http://192.168.0.4:8080/api/v1";
		private static string productionHostStr = "http://lettuce-1045.appspot.com/api/v1";
		private string apiPath =   productionHostStr;
		private string _uploadURL;
		private string _catchURL;
		private string _userImageURL;

		public static void InitServer () {
			System.Console.WriteLine("Using Production Server");
			apiClient = new RestClient(apiPath);
			apiClient.CookieContainer = new CookieContainer();

			// todo:  other init, if any
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

		public static void FetchStore(Player curPlayer, BunnyList_callback callback)
		{
			string fullURL = "store";

			RestRequest request = new RestRequest(fullURL, Method.GET);

			apiClient.ExecuteAsync<Player>(request, (response) =>
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

	}
}

