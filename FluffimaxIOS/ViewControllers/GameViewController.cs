using System;
using System.Collections.Generic;
using UIKit;
using CoreGraphics;
using Fluffimax.Core;
using System.Timers;

namespace Fluffimax.iOS
{
	public partial class GameViewController : UIViewController
	{
		private bool givingCarrot = false;
		private List<BunnyGraphic> _bunnyGraphicList = new List<BunnyGraphic> ();
		private static int _bunSizePerLevel = 32;
		private static int kBunnyHopChance = 100;
		private static int kVerticalHopMin = 8;
		private static int kHorizontalHopMin = 16;
		private static int kVerticalHopMax = 32;
		private static int kHorizontalHopMax = 64;
		private static int kMinWidth = -150;
		private static int kMinHeight = -100;
		private static int kMaxWidth = 150;
		private static int kMaxHeight = 200;
		private Bunny _currentBuns = null;
		private bool inited = false;
		private Timer _idleTimer = new Timer ();

		private class BunnyGraphic
		{
			public Bunny LinkedBuns { get; set;}
			public UIImageView Button { get; set;}
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

			UITapGestureRecognizer tapGesture = new UITapGestureRecognizer (() => {
				SetCurrentBunny(null);
			});
			tapGesture.NumberOfTapsRequired = 1;
			GrassField.AddGestureRecognizer (tapGesture);
			BunnyDetailView.Hidden = true;

			FeedBunnyBtn.TouchUpInside += (object sender, EventArgs e) => {
				if (_currentBuns != null)
					MaybeGiveCarrot(_currentBuns);
			};

			SellBunnyBtn.TouchUpInside += (object sender, EventArgs e) => {
				// todo
			};

			BuyBunnyBtn.TouchUpInside += (object sender, EventArgs e) => {
				NavController.PushViewController(new BunnyShopViewController(), true);
			};

			BuyCarrotsBtn.TouchUpInside += (object sender, EventArgs e) => {
				NavController.PushViewController(new CarrotShopViewController(), true);
			};

			UITapGestureRecognizer renameTap = new UITapGestureRecognizer (() => {
				ShowRenameBunny();
			});
			renameTap.NumberOfTapsRequired = 1;

			BunnyNameLabel.AddGestureRecognizer (renameTap);

			GiveBtn.TouchUpInside += (object sender, EventArgs e) => {
				NavigationController.PushViewController (new GiveBunnyViewController (), true);
			};

			CatchBtn.TouchUpInside += (object sender, EventArgs e) => {
				NavigationController.PushViewController (new CatchBunnyViewController (), true);
			};

		}

		public void ShowRenameBunny() {
			UIAlertView alert = new UIAlertView();
			alert.Title = "Rename Bunny";
			alert.AddButton("OK");
			alert.AddButton("Cancel");
			alert.Message = "What do you want to name this cute bunny?";
			alert.AlertViewStyle = UIAlertViewStyle.PlainTextInput;
			alert.Clicked += (object s, UIButtonEventArgs ev) =>
			{
				if(ev.ButtonIndex ==0)
				{
					string input = alert.GetTextField(0).Text;
					_currentBuns.BunnyName = input;
					UpdateBunnyPanel();
				}
			};
			alert.Show();

		}

		public override void ViewWillAppear (bool animated)
		{
			base.ViewWillAppear (animated);
			BunnyNameLabel.UserInteractionEnabled = true;
			GrassField.UserInteractionEnabled = true;
			NavController.NavigationBarHidden = true;
			InitGame();

			UpdateScore ();

			CheckForNewBunnies ();
		}

		private void CheckForNewBunnies() {
			if (Game.CurrentPlayer.RecentlyPurchased) {
				// more bunnies have been bought - add them if needed
				foreach (Bunny curBunny in Game.CurrentPlayer.Bunnies) {
					if (_bunnyGraphicList.Find (b => b.LinkedBuns == curBunny) == null) {
						AddBunnyToScreen (curBunny);
					}
				}
			}
		}

		protected UINavigationController NavController { 
			get {
				return (UIApplication.SharedApplication.Delegate as AppDelegate).NavController;
			} 
		}



		private UIImageView AddBunnyToScreen(Bunny thebuns) {
			UIImageView bunsBtn = new UIImageView ();

			View.AddSubview (bunsBtn);
			bunsBtn.TranslatesAutoresizingMaskIntoConstraints = false;
			UIImage[]	imgList = SpriteManager.GetImageList (thebuns, "idle", "front");
			bunsBtn.AnimationImages = imgList;
			bunsBtn.AnimationDuration = 1;
			//CGRect bunsRect = new CGRect (200, 200, 64, 64);
			//bunsBtn.Bounds = bunsRect;
			//bunsBtn.Frame = bunsRect;

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
				View, NSLayoutAttribute.CenterY, 1, 0);
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


			// todo:  add a gesture recognizer  
			//bunsBtn.TouchUpInside += HandleBunnyClick;
			View.UpdateConstraints ();
			UpdateBunsSizeAndLocation (thebuns);
			View.BringSubviewToFront (bunsBtn);
			bunsBtn.StartAnimating ();
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
			UIImageView bunsBtn = sender as UIImageView;
			Bunny	theBuns = _bunnyGraphicList.Find (i => i.Button == bunsBtn).LinkedBuns;

			SetCurrentBunny(theBuns);
		}

		public double BunnySizeForLevel(int level) {
			return _bunSizePerLevel * (level+1);
		}

		public override void DidReceiveMemoryWarning ()
		{
			base.DidReceiveMemoryWarning ();
			// Release any cached data, images, etc that aren't in use.
		}

		private void InitGame() {
			if (!inited) {
				InvokeOnMainThread (() => {
					CarrotImg.Hidden = true;
					// ad bunnies
					foreach (Bunny curBunny in Game.CurrentPlayer.Bunnies) {
						AddBunnyToScreen (curBunny);
					}
					HideBunnyPanel ();
					UpdateScore ();
					StartTimers ();
					inited = true;
				});
			}
		}

		private void SetCurrentBunny(Bunny newBuns) {
			if (_currentBuns != null)
				DeselectBunny (_currentBuns);
			_currentBuns = newBuns;
			if (newBuns != null) {
				SelectBunny (newBuns);
				ShowBunnyPanel ();
			}
			else {
				HideBunnyPanel ();

			}
		}

		private void SelectBunny(Bunny theBuns) {
			InvokeOnMainThread (() => {
				UIImage[]	imgList = SpriteManager.GetImageList(theBuns, "idle", "front");
				UIImageView bunBtn = _bunnyGraphicList.Find(b => b.LinkedBuns == theBuns).Button;

				if (bunBtn != null) {
					View.BringSubviewToFront(bunBtn);
					bunBtn.AnimationImages = imgList;
					bunBtn.AnimationDuration = 1;
					bunBtn.StartAnimating ();
				}
			});
		}

		private void DeselectBunny(Bunny theBuns) {
			InvokeOnMainThread (() => {

			});
		}

		private void ShowBunnyPanel() {
			InvokeOnMainThread (() => {
				UpdateBunnyPanel();
				if (BunnyDetailView.Hidden) {
					BunnyDetailView.Layer.Opacity = 0;
					BunnyDetailView.Hidden = false;
					UIView.Animate (.5, () => {
						BunnyDetailView.Layer.Opacity = 1;
					}, () => {
						BunnyDetailView.Layer.Opacity = 1;
						UpdateBunnyPanel();
					});
				}
			});
		}

		private void UpdateBunnyPanel() {
			if (_currentBuns != null) {
				InvokeOnMainThread (() => {
					BunnyNameLabel.Text = _currentBuns.BunnyName;
					BunnyBreedLabel.Text = _currentBuns.BreedName;
					BunnyGenderLabel.Text = _currentBuns.Female ? "female" : "male";
					FurColorLabel.Text = _currentBuns.FurColorName;
					EyeColorLabel.Text = _currentBuns.EyeColorName;
					SizeCount.Text = _currentBuns.BunnySize.ToString();
					ProgressCount.Text = String.Format("{0}/{1}", _currentBuns.FeedState,_currentBuns.CarrotsForNextSize(_currentBuns.BunnySize));
				});
			}
		}

		private void HideBunnyPanel() {
			if (!BunnyDetailView.Hidden) {
				InvokeOnMainThread (() => {
					BunnyDetailView.Layer.Opacity = 1;
					BunnyDetailView.Hidden = false;
					UIView.Animate (.25, () => {
						BunnyDetailView.Layer.Opacity = 0;
					}, () => {
						BunnyDetailView.Layer.Opacity = 0;
						BunnyDetailView.Hidden = true;
					});
				});
			}
		}

		private void StartTimers() {
			_idleTimer.Interval = 500;
			_idleTimer.AutoReset = false;
			_idleTimer.Elapsed += (object sender, ElapsedEventArgs e) => {
				MaybeBunniesHop();
			};
			_idleTimer.Start ();
		}

		private void MaybeBunniesHop() {
			if ((_bunnyGraphicList.Count > 0) && (Game.Rnd.Next (100) < kBunnyHopChance)) {
				int whichBunny = Game.Rnd.Next (_bunnyGraphicList.Count);
				BunnyGraphic bunsGraphic = _bunnyGraphicList [whichBunny];
				if ((bunsGraphic.LinkedBuns == _currentBuns) && givingCarrot) {
					// don't jump when eating
					_idleTimer.Start ();
				} else 
					DoBunnyHop (bunsGraphic);

			} else {
				_idleTimer.Start ();
			}
		}

		private void DoBunnyHop(BunnyGraphic buns) {
			int dir = Game.Rnd.Next (8);
			int xDif = 0, yDif = 0;
			int verticalHop = Game.Rnd.Next (kVerticalHopMin, kVerticalHopMax);
			int horizontalHop = Game.Rnd.Next (kHorizontalHopMin, kHorizontalHopMax);
			UIImage[]  bunnyJumpImageFrames = null;
			UIImage[] bunnyIdleImageFrames = null;
			switch (dir) {
			case 0://up
				yDif = -verticalHop;
				bunnyJumpImageFrames = SpriteManager.GetImageList (buns.LinkedBuns, "hop", "back");
				bunnyIdleImageFrames = SpriteManager.GetImageList (buns.LinkedBuns, "idle", "back");
				break;
			case 1: //upright
				yDif = -verticalHop;
				xDif = horizontalHop;
				bunnyJumpImageFrames = SpriteManager.GetImageList (buns.LinkedBuns, "hop", "rightback");
				bunnyIdleImageFrames = SpriteManager.GetImageList (buns.LinkedBuns, "idle", "rightback");
				break;
			case 2: // right
				xDif = horizontalHop;
				bunnyJumpImageFrames = SpriteManager.GetImageList (buns.LinkedBuns, "hop", "right");
				bunnyIdleImageFrames = SpriteManager.GetImageList (buns.LinkedBuns, "idle", "right");
				break;
			case 3: // downright
				yDif = verticalHop;
				xDif = horizontalHop;
				bunnyJumpImageFrames = SpriteManager.GetImageList (buns.LinkedBuns, "hop", "rightfront");
				bunnyIdleImageFrames = SpriteManager.GetImageList (buns.LinkedBuns, "idle", "rightfront");
				break;
			case 4: // down
				yDif = verticalHop;
				bunnyJumpImageFrames = SpriteManager.GetImageList (buns.LinkedBuns, "hop", "front");
				bunnyIdleImageFrames = SpriteManager.GetImageList (buns.LinkedBuns, "idle", "front");
				break;
			case 5: // downleft
				yDif = verticalHop;
				xDif = -horizontalHop;
				bunnyJumpImageFrames = SpriteManager.GetImageList (buns.LinkedBuns, "hop", "rightfront");
				bunnyIdleImageFrames = SpriteManager.GetImageList (buns.LinkedBuns, "idle", "rightfront");
				break;
			case 6:// left
				xDif = -horizontalHop;
				bunnyJumpImageFrames = SpriteManager.GetImageList (buns.LinkedBuns, "hop", "right");
				bunnyIdleImageFrames = SpriteManager.GetImageList (buns.LinkedBuns, "idle", "right");
				break;
			case 7: // upleft
				yDif = -verticalHop;
				xDif = -horizontalHop;
				bunnyJumpImageFrames = SpriteManager.GetImageList (buns.LinkedBuns, "hop", "rightback");
				bunnyIdleImageFrames = SpriteManager.GetImageList (buns.LinkedBuns, "idle", "rightback");
				break;
			}

			if (bunnyJumpImageFrames == null || bunnyIdleImageFrames == null) {

			}


			InvokeOnMainThread (() => {
				int newX = (int)buns.Horizontal.Constant + xDif;
				int newY = (int)buns.Vertical.Constant + yDif;

				if (newX < kMinWidth)
					newX = kMinWidth;
				else if (newX > kMaxWidth)
					newX = kMaxWidth;

				if (newY < kMinHeight)
					newY = kMinHeight;
				else if (newY > kMaxHeight)
					newY = kMaxHeight;

				BunnyHopToNewLoc (buns, dir, newX, newY, bunnyJumpImageFrames, bunnyIdleImageFrames);
			});
		}

		private void BunnyHopToNewLoc(BunnyGraphic buns, int dir, int newX, int newY, UIImage[] jumpFrames, UIImage[] idleFrames) {
			InvokeOnMainThread (() => {
				buns.Button.AnimationImages = jumpFrames;
				buns.Button.AnimationDuration = .1;
				buns.Button.StartAnimating ();
			});
			UIView.Animate (.5, () => {
				buns.Horizontal.Constant = newX;
				buns.Vertical.Constant = newY;
				View.LayoutIfNeeded();
			}, () => {
				//SetBunnyIdleGraphic (buns);
				InvokeOnMainThread (() => {
					buns.Button.AnimationImages = idleFrames;
					buns.Button.AnimationDuration = 1;
					buns.Button.StartAnimating ();
				});
				buns.LinkedBuns.UpdateLocation(newX, newY);
				_idleTimer.Start();
				CheckBunnyBreeding();
			});
		}

		private void CheckBunnyBreeding() {
			if (_bunnyGraphicList.Count > 1) {
				for (int i = 0; i < _bunnyGraphicList.Count - 1; i++) {
					BunnyGraphic firstBuns = _bunnyGraphicList [i];
					for (int j = 1; j < _bunnyGraphicList.Count; j++) {
						BunnyGraphic secondBuns = _bunnyGraphicList [j];

						if (firstBuns.Button.Frame.IntersectsWith (secondBuns.Button.Frame)) {
							// todo:  breed bunnies on server
							/*
							Bunny newBuns = Bunny.BreedBunnies (firstBuns.LinkedBuns, secondBuns.LinkedBuns);
							if (newBuns != null) {
								Game.CurrentPlayer.GetBunny (newBuns);
								AddBunnyToScreen (newBuns);
								return;
							}
							*/
						}
					}
				}
			}
		}

		private void SetBunnyDirectionGraphic(BunnyGraphic buns, int dir) {
			BeginInvokeOnMainThread (() => {
				// to do - change the bunny to face the direction
			});
		}

		private void SetBunnyIdleGraphic(BunnyGraphic buns) {
			BeginInvokeOnMainThread (() => {
				// to do - change the bunny to a random idle state
			});
		}

		private void PauseTimers() {
			_idleTimer.Stop ();
		}

		private void ResumeTimers() {
			_idleTimer.Start ();
		}

		private void UpdateScore() {
			InvokeOnMainThread (() => {
				CarrotCount.Text = Game.CurrentPlayer.carrotCount.ToString();

			});
		}

		private void MaybeGiveCarrot(Bunny theBuns) {
			
			if ((Game.CurrentPlayer.carrotCount > 0) && !givingCarrot) {
				givingCarrot = true;
				// ok give one
				InvokeOnMainThread (() => {
					FeedBunnyBtn.Enabled = false;
					CarrotImg.Hidden = false;
					UpdateScore();
					UIImage[] imageList = SpriteManager.GetImageList(theBuns, "idle", "front");
					UIImageView bunBtn = _bunnyGraphicList.Find(b => b.LinkedBuns == theBuns).Button;

					if (bunBtn != null) {
						View.BringSubviewToFront(bunBtn);
						bunBtn.AnimationImages = imageList;
						bunBtn.AnimationDuration = 1;
						bunBtn.StartAnimating ();
					}
				});

				bool grew = Game.CurrentPlayer.FeedBunny(theBuns);
				AnimateBunsSizeAndLocation (theBuns, grew);

			}
		}

		private void AnimateBunsSizeAndLocation(Bunny thebuns, bool grew) {
			BunnyGraphic theGraphic = _bunnyGraphicList.Find (b => b.LinkedBuns == thebuns);
			View.BringSubviewToFront (CarrotImg);
			if (theGraphic != null) {
				
				nfloat bunsSizeBase = (nfloat)BunnySizeForLevel (thebuns.BunnySize);
				double nextLevelSize = BunnySizeForLevel (thebuns.BunnySize + 1);
				nfloat deltaSize = (nfloat)((nextLevelSize - bunsSizeBase) * thebuns.Progress);
				nfloat oldX = CSCarrotX.Constant;
				nfloat oldY = CSCarrotY.Constant;
				double duration = .5;
				if (grew)
					duration = 4;
				
				UIView.Animate (1, () => {
					CSCarrotX.Constant = theGraphic.Horizontal.Constant;
					CSCarrotY.Constant = theGraphic.Vertical.Constant;
					View.LayoutIfNeeded();
				}, () => {
					InvokeOnMainThread(() => {
						CSCarrotX.Constant = oldX;
						CSCarrotY.Constant = oldY;
						CarrotImg.Hidden = true;
						View.LayoutIfNeeded();

						UIView.Animate (duration, () => {
							theGraphic.Height.Constant = bunsSizeBase;
							theGraphic.Width.Constant = bunsSizeBase + deltaSize;
							View.LayoutIfNeeded();
						}, () => {
							InvokeOnMainThread(() => {
								UpdateBunsSizeAndLocation(thebuns);
								UpdateBunnyPanel(); 
								givingCarrot = false;
								FeedBunnyBtn.Enabled = true;
							});
						});

					});
				});


			}
		}

	}
}


