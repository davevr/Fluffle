﻿using System;
using System.Collections.Generic;
using RestSharp;
using ServiceStack.Text;
using System.Net;

namespace Fluffimax.Core
{
	public class Server
	{
		public delegate void Player_callback(Player theResult);
		public delegate void BunnyList_callback(List<Bunny> theResult);
		public delegate void string_callback(string theResult);
		public delegate void bool_callback(bool theResult);
		public delegate void Bunny_callback(Bunny theResult);

		private static RestClient apiClient;
		private static string localHostStr = "http://localhost:8080/api/v1";
		private static string networkHostStr = "http://192.168.0.4:8080/api/v1";
		private static string productionHostStr = "http://fluffle-1045.appspot.com/api/v1";
		private static string apiPath =   localHostStr;
		private static string _uploadURL;
		private static string _catchURL;
		private static string _userImageURL;

		public static void InitServer () {
			if (apiPath == localHostStr)
				System.Console.WriteLine("Using Local Server");
			else if (apiPath == productionHostStr)
				System.Console.WriteLine("Using Production Server");
			else if (apiPath == networkHostStr)
				System.Console.WriteLine("Using LAN Server");
			
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

		public static void StartToss(Bunny bunsToToss, string_callback callback)
		{
			string fullURL = "toss";

			RestRequest request = new RestRequest(fullURL, Method.GET);
			request.AddParameter ("bunnyid", bunsToToss.ID);

			apiClient.ExecuteAsync(request, (response) =>
				{
					String tossURL = response.Content;
					if (tossURL != null)
					{
						callback(tossURL);
					}
					else
						callback(null);
				});
		}

		public static void CatchToss(long tossId, Bunny_callback callback)
		{
			string fullURL = "catch";

			RestRequest request = new RestRequest(fullURL, Method.POST);
			request.AddParameter ("tossid", tossId);
			apiClient.ExecuteAsync<Bunny>(request, (response) =>
				{
					Bunny newBuns = response.Data;
					if (newBuns != null)
					{
						callback(newBuns);
					}
					else
						callback(null);
				});
		}

	}
}
