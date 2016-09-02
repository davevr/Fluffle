
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace Fluffle.AndroidApp
{
	[Activity(Label = "ProfileFragment")]
	public class ProfileFragment : Android.Support.V4.App.Fragment
	{
		public MainActivity MainPage { get; set; }

		public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
		{
			// Use this to return your custom view for this Fragment
			// return inflater.Inflate(Resource.Layout.YourFragment, container, false);

			base.OnCreateView(inflater, container, savedInstanceState);

			var view = inflater.Inflate(Resource.Layout.ProfileLayout, container, false);

			var profileImageView = view.FindViewById<ImageView>(Resource.Id.userImage);

			return view;
		}
	}
}

