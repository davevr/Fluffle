using System;

using UIKit;

using CoreGraphics;
using Foundation;

using ObjCRuntime;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Fluffimax.iOS
{
	public class FluffleColor
	{
		
		private static UIColor _blackStandard = UIColor.FromRGB(0,0,0);
		private static UIColor _whiteStandard = UIColor.FromRGB(255,255,255);



		public FluffleColor ()
		{
		}


		public static UIColor Black {get { return _blackStandard; }}
		public static UIColor White {get { return _whiteStandard; }}
	}

	public static class LocalizationExtensions
	{
		public static string Localize(this string key)
		{
			return NSBundle.MainBundle.LocalizedString(key, null);
		}
	}

	public static class NSStringExtensions
	{
		[DllImport(Constants.ObjectiveCLibrary, EntryPoint="objc_msgSend")]
		private extern static IntPtr IntPtr_objc_msgSend_IntPtr (IntPtr receiver, IntPtr selector, IntPtr arg1);

		public static NSString LocalizedStringWithFormat (this NSString @string)
		{
			var nsHandle = IntPtr_objc_msgSend_IntPtr (Class.GetHandle (typeof(NSString)), Selector.GetHandle ("localizedStringWithFormat:"), @string.Handle);
			var nsstring = Runtime.GetNSObject<NSString> (nsHandle);
			return nsstring;
		}
	}
}

