using System;
using System.Collections.Generic;
using UIKit;
using CoreGraphics;
using Fluffimax.Core;

namespace Fluffimax
{
	public partial class GameViewController : UIViewController
	{
		private bool givingCarrot = false;
		private List<BunnyGraphic> _bunnyGraphicList = new List<BunnyGraphic> ();
		private static int _bunSizePerLevel = 32;

		private class BunnyGraphic
		{
			public Bunny LinkedBuns { get; set;}
			public UIButton Button { get; set;}
			public NSLayoutConstraint Width { get; set;}
			public NSLayoutConstraint Height {get; set;}
			public NSLayoutConstraint Horizontal {get; set;}
			public NSLayoutConstraint Vertical {get; set;}
			public int BunnyState { get; set;}

		}

		public GameViewController () : base ("GameViewController", null)
		{
		}

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();
			// Perform any additional setup after loading the view, typically from a nib

			InitGame();
		}

		private UIButton AddBunnyToScreen(Bunny thebuns) {
			UIButton bunsBtn = UIButton.FromType (UIButtonType.Custom);
			View.AddSubview (bunsBtn);
			bunsBtn.TranslatesAutoresizingMaskIntoConstraints = false;
			UIImage bunsImage = UIImage.FromBundle ("bunny");
			bunsBtn.SetImage (bunsImage, UIControlState.Normal);
			CGRect bunsRect = new CGRect (200, 200, 64, 64);
			bunsBtn.Bounds = bunsRect;
			bunsBtn.Frame = bunsRect;

			NSLayoutConstraint csWidth = NSLayoutConstraint.Create (bunsBtn, NSLayoutAttribute.Width, NSLayoutRelation.Equal,
				                            null, NSLayoutAttribute.NoAttribute, 1, 32);
			csWidth.Active = true;

			NSLayoutConstraint csHeight = NSLayoutConstraint.Create (bunsBtn, NSLayoutAttribute.Height, NSLayoutRelation.Equal,
				null, NSLayoutAttribute.NoAttribute, 1, 32);
			csHeight.Active = true;
			NSLayoutConstraint csHorizontal = NSLayoutConstraint.Create (bunsBtn, NSLayoutAttribute.CenterX, NSLayoutRelation.Equal,
				View, NSLayoutAttribute.CenterX, 1, 0);
			csHorizontal.Active = true;
			NSLayoutConstraint csVertical = NSLayoutConstraint.Create (bunsBtn, NSLayoutAttribute.CenterY, NSLayoutRelation.Equal,
				View, NSLayoutAttribute.CenterX, 1, 0);
			csVertical.Active = true;

			View.AddConstraint (csHorizontal);
			View.AddConstraint (csVertical);

			bunsBtn.AddConstraint (csWidth);
			bunsBtn.AddConstraint (csHeight);
			bunsBtn.UpdateConstraints ();


			BunnyGraphic graphic = new BunnyGraphic ();
			graphic.BunnyState = 1;
			graphic.Button = bunsBtn;
			graphic.Height = csHeight;
			graphic.Width = csWidth;
			graphic.Horizontal = csHorizontal;
			graphic.Vertical = csVertical;
			graphic.LinkedBuns = thebuns;
			_bunnyGraphicList.Add (graphic);


			bunsBtn.TouchUpInside += HandleBunnyClick;
			View.UpdateConstraints ();
			UpdateBunsSizeAndLocation (thebuns);


			return bunsBtn;
		}

		private void UpdateBunsSizeAndLocation(Bunny thebuns) {
			BunnyGraphic theGraphic = _bunnyGraphicList.Find (b => b.LinkedBuns == thebuns);

			if (theGraphic != null) {
				InvokeOnMainThread (() => {
					nfloat bunsSizeBase = (nfloat)BunnySizeForLevel (thebuns.BunnySize);
					double nextLevelSize = BunnySizeForLevel (thebuns.BunnySize + 1);
					nfloat deltaSize = (nfloat)((nextLevelSize - bunsSizeBase) * thebuns.Progress);
					theGraphic.Height.Constant = bunsSizeBase;
					theGraphic.Width.Constant = bunsSizeBase + deltaSize;
					theGraphic.Horizontal.Constant = thebuns.HorizontalLoc;
					theGraphic.Vertical.Constant = thebuns.VerticalLoc;
					View.SetNeedsLayout ();
					View.SetNeedsUpdateConstraints();
					View.LayoutIfNeeded();
					View.UpdateConstraints();
				});
			}
		}

		private void HandleBunnyClick (object sender, EventArgs e) {
			UIButton bunsBtn = sender as UIButton;
			Bunny	theBuns = _bunnyGraphicList.Find (i => i.Button == bunsBtn).LinkedBuns;
			MaybeGiveCarrot(theBuns);
		}

		public double BunnySizeForLevel(int level) {
			return _bunSizePerLevel * level;
		}

		public override void DidReceiveMemoryWarning ()
		{
			base.DidReceiveMemoryWarning ();
			// Release any cached data, images, etc that aren't in use.
		}

		private void InitGame() {
			InvokeOnMainThread (() => {
				foreach (Bunny curBunny in Game.CurrentPlayer.Bunnies) {
					AddBunnyToScreen(curBunny);
				}

				UpdateScore();
				CarrotImg.Hidden = true;
			});
		}

		private void UpdateScore() {
			InvokeOnMainThread (() => {
				CarrotCount.Text = Game.CurrentPlayer.CarrotCount.ToString();
			});
		}

		private void MaybeGiveCarrot(Bunny theBuns) {
			
			if ((Game.CurrentPlayer.CarrotCount > 0) && !givingCarrot) {
				givingCarrot = true;
				// ok give one
				CarrotImg.Hidden = false;

				bool grew = Game.CurrentPlayer.FeedBunny(theBuns);
				AnimateBunsSizeAndLocation (theBuns, grew);

				UpdateScore ();

			}
		}

		private void AnimateBunsSizeAndLocation(Bunny thebuns, bool grew) {
			BunnyGraphic theGraphic = _bunnyGraphicList.Find (b => b.LinkedBuns == thebuns);

			if (theGraphic != null) {
				CSCarrotX.Constant = theGraphic.Horizontal.Constant;
				CSCarrotY.Constant = theGraphic.Vertical.Constant;
				nfloat bunsSizeBase = (nfloat)BunnySizeForLevel (thebuns.BunnySize);
				double nextLevelSize = BunnySizeForLevel (thebuns.BunnySize + 1);
				nfloat deltaSize = (nfloat)((nextLevelSize - bunsSizeBase) * thebuns.Progress);
				double duration = .5;
				if (grew)
					duration = 4;
				
				UIView.Animate (duration, () => {
					theGraphic.Height.Constant = bunsSizeBase;
					theGraphic.Width.Constant = bunsSizeBase + deltaSize;
					theGraphic.Horizontal.Constant = thebuns.HorizontalLoc;
					theGraphic.Vertical.Constant = thebuns.VerticalLoc;
				}, () => {
					UpdateBunsSizeAndLocation(thebuns);
					CarrotImg.Hidden = true;
					givingCarrot = false;
				});


			}
		}

	}
}


