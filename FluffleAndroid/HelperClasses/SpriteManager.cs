using Android.App;
using Android.Widget;
using Android.OS;
using Android.Support.V7.Widget;
using Android.Support.V7.View;
using Android.Support.V7.AppCompat;
using Android.Support.V7.App;
using Android.Support.V4.Widget;
using Android.Graphics;
using Android.Media;
using System.Collections.Generic;

using Fluffimax.Core;


namespace Fluffle.AndroidApp
{
	public class BunnyStateSprite
	{
		public Bitmap[]	frontImages;
		public Bitmap[]	backImages;
		public Bitmap[]	rightImages;
		public Bitmap[]	rightFrontImages;
		public Bitmap[]	rightBackImages;

		public BunnyStateSprite() {
			
		}

		public void InitSubState(string state, List<Bitmap> imageList) {
			switch (state) {
			case "front":
				frontImages = imageList.ToArray ();
				break;
			case "back":
				backImages = imageList.ToArray ();
				break;
			case "right":
				rightImages = imageList.ToArray ();
				break;
			case "rightfront":
				rightFrontImages = imageList.ToArray ();
				break;
			case "rightback":
				rightBackImages = imageList.ToArray ();
				break;

			}
		}
	}

	public class BunnyMasterSprite
	{
		public static int kSpriteSize = 128;
		public string spriteKey;

        Bitmap masterImage;

		public BunnyStateSprite idleState;
		public BunnyStateSprite hopState;
		public BunnyStateSprite eatState;

		public void Inflate() {
			string urlStr = Server.SpriteImagePath + spriteKey + ".png";
            masterImage = BitmapHelper.GetImageBitmapFromUrl(urlStr);
			

			idleState = new BunnyStateSprite ();

			// now do the states
			idleState = InflateState(0, 2); // idle has two frames
			hopState = InflateState(2, 3); // hop has three frames
			eatState = InflateState(5, 3);	// eat has three frames
			masterImage.Dispose();
			masterImage = null;
		}

		public BunnyStateSprite InflateState(int offset, int numFrames) {
			BunnyStateSprite theState = new BunnyStateSprite();
			theState.InitSubState ("rightfront", MakeImageList (masterImage, 0, offset, numFrames));
			theState.InitSubState ("right", MakeImageList (masterImage, 1, offset,numFrames));
			theState.InitSubState ("rightback", MakeImageList (masterImage, 2, offset,numFrames));
			theState.InitSubState ("back", MakeImageList (masterImage, 3, offset,numFrames));
			theState.InitSubState ("front", MakeImageList (masterImage, 4, offset,numFrames));

			return theState;
		}

		public List<Bitmap>	MakeImageList (Bitmap stateImageMap, int xOffset, int yOffset, int numFrames) {
			List<Bitmap>	imageList = new List<Bitmap> ();
			xOffset *= kSpriteSize;

			for (int i = 0; i < numFrames; i++) {
                Bitmap curFrame = CropImage (stateImageMap, xOffset, yOffset, kSpriteSize, kSpriteSize);
				imageList.Add(curFrame);
				yOffset += kSpriteSize;
			}

			return imageList;
		}

		public static Bitmap CropImage(Bitmap sourceImage, int crop_x, int crop_y, int width, int height)
		{
            Bitmap.Config config = Bitmap.Config.Argb8888;

            Bitmap newBitmap = Bitmap.CreateBitmap(width, height, config);
            Canvas canvas = new Canvas(newBitmap);
            Rect destRect = new Rect(0, 0, width, height);
            Rect srcRect = new Rect(crop_x, crop_y, width, height);
            Paint thePaint = new Paint(PaintFlags.AntiAlias);
            canvas.DrawBitmap(sourceImage, srcRect, destRect, thePaint);
            return newBitmap;
		}

	}

	public class SpriteManager
	{
		public static Dictionary<string, BunnyMasterSprite>	spriteMap;

		public static void Initialize() {
			// load in the base sprite
			spriteMap = new Dictionary<string, BunnyMasterSprite> ();
		}

		public static BunnyMasterSprite LoadSprite(Bunny theBuns) {
			string spriteKey = theBuns.GetImageID ();

			BunnyMasterSprite sprite;

			if (spriteMap.ContainsKey(spriteKey))
				sprite = spriteMap [spriteKey];
			else {
				sprite = new BunnyMasterSprite ();
				sprite.spriteKey = spriteKey;
				sprite.Inflate ();
				spriteMap.Add (spriteKey, sprite);
			}

			return sprite;
		}

		public static Bitmap[] GetImageList(Bunny theBuns, string stateStr, string dir) {
			BunnyMasterSprite theSprite = LoadSprite (theBuns);
			BunnyStateSprite theStateSprite = null;
            Bitmap[] theImageList = null;

			switch (stateStr) {
			case "idle":
				theStateSprite = theSprite.idleState;
				break;
			case "eat":
				theStateSprite = theSprite.eatState;
				break;
			case "hop":
				theStateSprite = theSprite.hopState;
				break;
			}

			switch (dir) {
			case "front":
				theImageList = theStateSprite.frontImages;
				break;
			case "back":
				theImageList = theStateSprite.backImages;
				break;
			case "right":
				theImageList = theStateSprite.rightImages;
				break;
			case "rightfront":
				theImageList = theStateSprite.rightFrontImages;
				break;
			case "rightback":
				theImageList = theStateSprite.rightBackImages;
				break;

			}

			return theImageList;
		}
	}
}

