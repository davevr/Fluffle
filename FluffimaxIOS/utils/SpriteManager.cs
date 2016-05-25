using System;
using System.Collections.Generic;
using UIKit;
using CoreGraphics;
using Fluffimax.Core;


namespace Fluffimax.iOS
{
	public class BunnyStateSprite
	{
		public UIImage[]	frontImages;
		public UIImage[]	backImages;
		public UIImage[]	rightImages;
		public UIImage[]	rightFrontImages;
		public UIImage[]	rightBackImages;

		public BunnyStateSprite() {
			
		}

		public void InitSubState(string state, List<UIImage> imageList) {
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
		public string eyeColor;
		public string furColor;
		public string breed;

		UIImage masterImage;

		public BunnyStateSprite idleState;
		public BunnyStateSprite hopState;
		public BunnyStateSprite eatState;

		public void Inflate() {
			string spriteKey = breed.ToLower () + "_" + furColor.ToLower () + "_" + eyeColor.ToLower ();

			masterImage = UIImage.FromBundle (spriteKey);	// to do - load from a URL?

			idleState = new BunnyStateSprite ();

			// now do the states
			idleState = InflateState(0, 2); // idle has two frames
			hopState = InflateState(2, 3); // hop has three frames
			eatState = InflateState(5, 3);	// eat has three frames
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

		public List<UIImage>	MakeImageList (UIImage stateImageMap, int xOffset, int yOffset, int numFrames) {
			List<UIImage>	imageList = new List<UIImage> ();
			xOffset *= kSpriteSize;

			for (int i = 0; i < numFrames; i++) {
				UIImage curFrame = CropImage (stateImageMap, xOffset, yOffset, kSpriteSize, kSpriteSize);
				imageList.Add(curFrame);
				yOffset += kSpriteSize;
			}

			return imageList;
		}

		public static UIImage CropImage(UIImage sourceImage, int crop_x, int crop_y, int width, int height)
		{
			var imgSize = sourceImage.Size;
			UIGraphics.BeginImageContext(new CGSize(width, height));
			var context = UIGraphics.GetCurrentContext();
			var clippedRect = new CGRect(0, 0, width, height);
			context.ClipToRect(clippedRect);
			var drawRect = new CGRect(-crop_x, -crop_y, imgSize.Width, imgSize.Height);
			sourceImage.Draw(drawRect);
			var modifiedImage = UIGraphics.GetImageFromCurrentImageContext();
			UIGraphics.EndImageContext();
			return modifiedImage;
		}

	}

	public class SpriteManager
	{
		public static Dictionary<string, BunnyMasterSprite>	spriteMap;

		public static void Initialize() {
			// load in the base sprite
			spriteMap = new Dictionary<string, BunnyMasterSprite> ();
			LoadSprite("minilop", "white", "blue");
		}

		public static BunnyMasterSprite LoadSprite(string breedStr, string furStr, string eyeStr) {
			string spriteKey = breedStr.ToLower () + "_" + furStr.ToLower () + "_" + eyeStr.ToLower ();

			// force it
			spriteKey = "minilop_white_blue";

			BunnyMasterSprite sprite;

			if (spriteMap.ContainsKey(spriteKey))
				sprite = spriteMap [spriteKey];
			else {
				sprite = new BunnyMasterSprite ();
				sprite.breed = breedStr;
				sprite.eyeColor = eyeStr;
				sprite.furColor = furStr;
				sprite.Inflate ();
				spriteMap.Add (spriteKey, sprite);
			}

			return sprite;
		}

		public static UIImage[] GetImageList(Bunny theBuns, string stateStr, string dir) {
			// to do:  use the actual breed, once we have them.  return GetImageList (theBuns.BreedName, theBuns.FurColorName, theBuns.EyeColorName, stateStr, dir);
			return GetImageList ("minilop", theBuns.FurColorName, theBuns.EyeColorName, stateStr, dir);
		}


		public static UIImage[] GetImageList(string breedStr, string furStr, string eyeStr, string stateStr, string dir) {
			BunnyMasterSprite theSprite = LoadSprite (breedStr, furStr, eyeStr);
			BunnyStateSprite theStateSprite = null;
			UIImage[] theImageList = null;

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

