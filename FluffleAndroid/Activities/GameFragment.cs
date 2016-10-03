
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using Fluffimax.Core;
using System.Timers;
using ZXing.Mobile;
using Android.Graphics;
using Android.Views.Animations;


namespace Fluffle.AndroidApp
{
	public class GameFragment : Android.Support.V4.App.Fragment
	{
		public MainActivity MainPage { get; set;}
        private bool givingCarrot = false;
        private List<BunnyGraphic> _bunnyGraphicList = new List<BunnyGraphic>();
        private static int _bunBaseSize = 48;
        private static int _bunSizePerLevel = 8;
        private static int kBunnyHopChance = 100;
        private static int kVerticalHopMin = 4;
        private static int kHorizontalHopMin = 8;
        private static int kVerticalHopMax = 16;
        private static int kHorizontalHopMax = 32;
        private static float kMinWidth = -100;
        private static float kMinHeight = -100;
        private static float kMaxWidth = 100;
        private static float kMaxHeight = 100;
        private Bunny _currentBuns = null;
        private bool inited = false;
        private Timer _idleTimer = new Timer();

        private FrameLayout field;
        private LinearLayout detailView;
        private TextView bunnyNameLabel;
        private TextView bunnyDescLabel;
        private ProgressBar progressBar;
        private Button feedBtn;
        private Button tossBtn;
        private Button sellBtn;
        private Button buyCarrotsBtn;
        private Button catchBtn;
        private Button adoptBunnyBtn;
        private ImageView CarrotImg;

        private class BunnyGraphic
        {
            public Bunny LinkedBuns { get; set; }
            public ImageView Button { get; set; }
            public int BunnyState { get; set; }
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
		{
			// Use this to return your custom view for this Fragment
			// return inflater.Inflate(Resource.Layout.YourFragment, container, false);

			base.OnCreateView(inflater, container, savedInstanceState);

			var view = inflater.Inflate(Resource.Layout.GameLayout, container, false);
            field = view.FindViewById<FrameLayout>(Resource.Id.grassView);
            detailView = view.FindViewById<LinearLayout>(Resource.Id.detailView);
            bunnyNameLabel = view.FindViewById<TextView>(Resource.Id.bunnyNameLabel);
            bunnyDescLabel = view.FindViewById<TextView>(Resource.Id.bunnyDescLabel);
            progressBar = view.FindViewById<ProgressBar>(Resource.Id.feedProgressBar);
            feedBtn = view.FindViewById<Button>(Resource.Id.FeedBtn);
            tossBtn = view.FindViewById<Button>(Resource.Id.TossBunnyBtn);
            sellBtn = view.FindViewById<Button>(Resource.Id.SellBunnyBtn);
            buyCarrotsBtn = view.FindViewById<Button>(Resource.Id.CarrotShopBtn);
            catchBtn = view.FindViewById<Button>(Resource.Id.CatchBunnyBtn);
            adoptBunnyBtn = view.FindViewById<Button>(Resource.Id.AdoptShopBtn);
            CarrotImg = view.FindViewById<ImageView>(Resource.Id.CarrotImage);
            CarrotImg.Visibility = ViewStates.Gone;

            detailView.Visibility = ViewStates.Gone;

            feedBtn.Click += FeedBtn_Click;
            tossBtn.Click += TossBtn_Click;
            sellBtn.Click += SellBtn_Click;
            buyCarrotsBtn.Click += BuyCarrotsBtn_Click;
            catchBtn.Click += CatchBtn_Click;
            adoptBunnyBtn.Click += AdoptBunnyBtn_Click;

            bunnyNameLabel.Click += BunnyNameLabel_Click;
            return view;
		}

        private void BunnyNameLabel_Click(object sender, EventArgs e)
        {
            ShowRenameBunny();
        }

        private void AdoptBunnyBtn_Click(object sender, EventArgs e)
        {
            if (Game.CurrentPlayer.Bunnies.Count < 50)
            {
                //NavController.PushViewController(new BunnyShopViewController(), true);
            }
            else
            {
                //HomeViewController.ShowMessageBox("Adoption_Agency".Localize(), "Too_Many_Bunnies".Localize(), "Too_Many_Bunnies_Btn".Localize());
            }
        }

        
        private void CatchBtn_Click(object sender, EventArgs e)
        {
            DoCatchBunny();
        }

        private void BuyCarrotsBtn_Click(object sender, EventArgs e)
        {
            //NavController.PushViewController(new CarrotShopViewController(), true);
        }

        private void SellBtn_Click(object sender, EventArgs e)
        {
            MaybeSellBunny();
        }

        private void TossBtn_Click(object sender, EventArgs e)
        {
            Game.BunnyBeingSold = _currentBuns;
            Game.BunnySellPrice = 0;
            //NavigationController.PushViewController(new GiveBunnyViewController(), true);
        }

        private void FeedBtn_Click(object sender, EventArgs e)
        {
            if (_currentBuns != null)
                MaybeGiveCarrot(_currentBuns);
        }

        public override void OnStart()
        {
            base.OnStart();
            InitFirstGame();
        }
        public void InitFirstGame()
        {
            // todo - populate the bunnies!
            InitGame();

            UpdateScore();

            CheckForNewBunnies();
            CheckForRecentPurchase();

            DoBunnyHop(_bunnyGraphicList[0]);
        }

        private void MaybeSellBunny()
        {
            Server.GetMarketPrice(_currentBuns.id, (thePrice) =>
            {
                Activity.RunOnUiThread(() =>
                {
                    var builder = new AlertDialog.Builder(this.Activity);
                    builder.SetTitle(Resource.String.Sell_Bunny_Title.Localize());
                    builder.SetMessage(string.Format(Resource.String.Sell_Bunny_Msg.Localize(), _currentBuns.BreedName, thePrice));
                    builder.SetPositiveButton(Resource.String.Sell_Bunny_Btn.Localize(), (sender, args) =>
                    {
                        Server.SellBunny(_currentBuns.id, (salePrice) =>
                        {
                            if (salePrice > 0)
                            {
                                Activity.RunOnUiThread(() =>
                                {
                                    Bunny soldBuns = _currentBuns;
                                    Game.CurrentPlayer.carrotCount += salePrice;
                                    SetCurrentBunny(null);
                                    RemoveBunnyFromPlayer(soldBuns);
                                    UpdateScore();

                                });
                            }
                            else
                            {
                                // sell failed for some reason
                                MainActivity.ShowAlert(this.Context, Resource.String.Bunny_Sale_Failed_Title.Localize(), Resource.String.Bunny_Sale_Failed_Msg.Localize(), Resource.String.Bunny_Sale_Failed_Btn.Localize());
                            }
                        });
                    });
                    builder.Show();

                });
            });
        }

        private async void DoCatchBunny()
        {
            var scanner = new ZXing.Mobile.MobileBarcodeScanner();
            var options = new ZXing.Mobile.MobileBarcodeScanningOptions();
            options.PossibleFormats = new List<ZXing.BarcodeFormat>() {
                ZXing.BarcodeFormat.AZTEC
            };
            options.CameraResolutionSelector = (resList) => {
                CameraResolution finalRes = null;

                foreach (CameraResolution curRes in resList)
                {
                    if (((curRes.Height == 640) || (curRes.Width == 640)) && finalRes == null)
                        finalRes = curRes;
                    else if ((curRes.Height == 720) || (curRes.Width == 720))
                        finalRes = curRes;

                }

                return finalRes;
            };

            var result = await scanner.Scan(this.Context, options);

            if (result != null)
            {
                FinalizeCatch(result.Text);
            }
        }

        private void FinalizeCatch(string catchResult)
        {
            long tossId = long.Parse(catchResult);

            Server.GetTossStatus(tossId, (theToss) => {
                Activity.RunOnUiThread(() => {
                    if (!theToss.isValid)
                    {
                        MainActivity.ShowAlert(this.Context, Resource.String.Catch_Failed_Title.Localize(), Resource.String.Catch_Failed_Gone_Msg.Localize(), Resource.String.Catch_Failed_Gone_Btn.Localize());
                    }
                    else if (theToss.price > Game.CurrentPlayer.carrotCount)
                    {
                        MainActivity.ShowAlert(this.Context, Resource.String.Catch_Failed_Title.Localize(), Resource.String.Catch_Failed_Funds_Msg.Localize(), Resource.String.Catch_Failed_Funds_Btn.Localize());
                    }
                    else
                    {
                        var builder = new AlertDialog.Builder(this.Activity);
                        builder.SetTitle(Resource.String.Catch_Title.Localize());

                        if (theToss.price > 0)
                        {
                            builder.SetMessage(string.Format(Resource.String.Catch_Paid_Msg.Localize(), theToss.price));
                            builder.SetPositiveButton(Resource.String.Catch_Paid_OK_Btn.Localize(), (sender, args) => {
                                DoCatchToss(tossId);
                            });
                            builder.SetNegativeButton(Resource.String.Catch_Cancel_Btn.Localize(), (sender, args) => { });
                        }
                        else
                        {
                            builder.SetMessage(string.Format(Resource.String.Catch_Free_Msg.Localize(), theToss.price));
                            builder.SetPositiveButton(Resource.String.Catch_Free_OK_Btn.Localize(), (sender, args) => {
                                DoCatchToss(tossId);
                            });
                        }

                        builder.Show();
                    }
                });
            });
        }

        private void DoCatchToss(long tossId)
        {
            Server.CatchToss(tossId, (theBuns) => {
                Activity.RunOnUiThread(() => {
                    if (theBuns != null)
                    {
                        MainActivity.ShowAlert(this.Context, Resource.String.Catch_Success_Title.Localize(), Resource.String.Catch_Success_Msg.Localize(), Resource.String.Catch_Success_Btn.Localize());
                        Game.RecentlyPurchased = true;
                        Game.CurrentPlayer.Bunnies.Add(theBuns);
                        CheckForNewBunnies();
                    }
                    else
                    {
                        // something went wrong
                        MainActivity.ShowAlert(this.Context, Resource.String.Catch_Failed_Title.Localize(), Resource.String.Catch_Failed_Unknown_Msg.Localize(), Resource.String.Catch_Failed_Unknown_Btn.Localize());
                    }
                });
            });
        }

        private void HandleBunnyTap()
        {
            // todo 
        }

        public void DoPetBunny()
        {
            HappyBuns(this._currentBuns);
        }

        public void HappyBuns(Bunny whichBuns)
        {
            bool superHappy = whichBuns.IncrementHappiness();
            Server.RecordPetBunny(whichBuns);

            BunnyGraphic theGraphic = _bunnyGraphicList.Find(b => b.LinkedBuns == whichBuns);

            /*
            nfloat baseX = theGraphic.Horizontal.Constant;
            nfloat baseY = theGraphic.Vertical.Constant;

            UIImageView heartImage = new UIImageView(new CGRect(100, 100, 24, 24));
            heartImage.Image = UIImage.FromBundle("heart");
            PlayfieldView.AddSubview(heartImage);
            heartImage.TranslatesAutoresizingMaskIntoConstraints = false;
            NSLayoutConstraint csWidth = NSLayoutConstraint.Create(heartImage, NSLayoutAttribute.Width, NSLayoutRelation.Equal,
                                            null, NSLayoutAttribute.NoAttribute, 1, 24);
            csWidth.Active = true;

            NSLayoutConstraint csHeight = NSLayoutConstraint.Create(heartImage, NSLayoutAttribute.Height, NSLayoutRelation.Equal,
                null, NSLayoutAttribute.NoAttribute, 1, 24);
            csHeight.Active = true;
            NSLayoutConstraint csHorizontal = NSLayoutConstraint.Create(heartImage, NSLayoutAttribute.CenterX, NSLayoutRelation.Equal,
                PlayfieldView, NSLayoutAttribute.Left, 1, baseX);
            csHorizontal.Active = true;
            NSLayoutConstraint csVertical = NSLayoutConstraint.Create(heartImage, NSLayoutAttribute.CenterY, NSLayoutRelation.Equal,
                PlayfieldView, NSLayoutAttribute.Top, 1, baseY);
            csVertical.Active = true;
            heartImage.AddConstraint(csHeight);
            heartImage.AddConstraint(csWidth);
            PlayfieldView.AddConstraint(csHorizontal);
            PlayfieldView.AddConstraint(csVertical);
            heartImage.UpdateConstraints();
            PlayfieldView.UpdateConstraints();

            heartImage.Hidden = false;
            heartImage.Layer.Opacity = 1;
            heartImage.Layer.ZPosition = 10000;
            PlayfieldView.LayoutIfNeeded();

            UIView.Animate(1, () =>
            {
                csVertical.Constant = baseY - 128;
                heartImage.Layer.Opacity = 0;
                csHeight.Constant = 64;
                csWidth.Constant = 64;
                PlayfieldView.LayoutIfNeeded();
            }, () =>
            {
                InvokeOnMainThread(() =>
                {
                    heartImage.Hidden = true;
                    heartImage.Layer.Opacity = 1;
                    heartImage.RemoveFromSuperview();
                    PlayfieldView.RemoveConstraint(csHorizontal);
                    PlayfieldView.RemoveConstraint(csVertical);
                    heartImage.RemoveConstraint(csWidth);
                    heartImage.RemoveConstraint(csHeight);
                    heartImage.Dispose();
                });
            });
            */
        }

        public void ShowRenameBunny()
        {
            if (string.IsNullOrEmpty(_currentBuns.BunnyName) || _currentBuns.OriginalOwner == Game.CurrentPlayer.id)
            {
                var theText = new EditText(this.Context);
                theText.Text = _currentBuns.BunnyName;
                var builder = new AlertDialog.Builder(this.Activity);
                builder.SetTitle(Resource.String.Rename_Title.Localize());
                builder.SetMessage(Resource.String.Rename_Msg.Localize());
                builder.SetView(theText);
                builder.SetPositiveButton(Resource.String.ok_btn.Localize(), (sender, args) => {
                    string input = theText.Text;
                    _currentBuns.BunnyName = input;
                    Server.RecordRenameBunny(_currentBuns);
                    UpdateBunnyPanel();
                });
                builder.SetNegativeButton(Resource.String.cancel_btn.Localize(), (sender, args) => { });
                builder.Show();
            }
            else
            {
                MainActivity.ShowAlert(this.Context, Resource.String.Rename_Title.Localize(), Resource.String.Rename_Err_Msg.Localize(), Resource.String.Rename_Err_Btn.Localize());
            }

        }

       

        private void CheckForNewBunnies()
        {
            if (Game.RecentlyPurchased)
            {
                // more bunnies have been bought - add them if needed
                foreach (Bunny curBunny in Game.CurrentPlayer.Bunnies)
                {
                    if (_bunnyGraphicList.Find(b => b.LinkedBuns == curBunny) == null)
                    {
                        AddBunnyToScreen(curBunny);
                    }
                }
                Game.RecentlyPurchased = false;
            }
        }

        private void CheckForRecentPurchase()
        {
            if (Game.BunnyBeingSold != null)
            {
                SetCurrentBunny(null);
                RemoveBunnyFromPlayer(Game.BunnyBeingSold);
                Game.BunnyBeingSold = null;
            }
        }

        private void RemoveBunnyFromPlayer(Bunny theBuns)
        {
            Game.CurrentPlayer.Bunnies.Remove(theBuns);
            BunnyGraphic theGraphic = _bunnyGraphicList.Find(b => b.LinkedBuns == theBuns);
            field.RemoveView(theGraphic.Button);
            _bunnyGraphicList.Remove(theGraphic);
        }


        private ImageView AddBunnyToScreen(Bunny thebuns)
        {
            ImageView bunsBtn = new ImageView(this.Context);

            field.AddView(bunsBtn);
            
            Bitmap[] imgList = SpriteManager.GetImageList(thebuns, "idle", "front");
            bunsBtn.SetImageBitmap(imgList[0]);
            bunsBtn.Click += BunsBtn_Click;

            FrameLayout.LayoutParams newParams = new FrameLayout.LayoutParams(32, 32);
            newParams.LeftMargin = 0;
            newParams.TopMargin = 0;
            bunsBtn.LayoutParameters = newParams;

            BunnyGraphic graphic = new BunnyGraphic();
            graphic.BunnyState = 1;
            graphic.Button = bunsBtn;
            graphic.LinkedBuns = thebuns;
            _bunnyGraphicList.Add(graphic);


            // todo:  add click on bunny method  
            UpdateBunsSizeAndLocation(thebuns);
            field.BringChildToFront(bunsBtn);
            DoBunnyHop(graphic);
            return bunsBtn;
        }

        private void BunsBtn_Click(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }

        private void UpdateBunsSizeAndLocation(Bunny thebuns)
        {
            BunnyGraphic theGraphic = _bunnyGraphicList.Find(b => b.LinkedBuns == thebuns);

            if (theGraphic != null)
            {
                Activity.RunOnUiThread(() => {
                    Rect bounds = new Rect(0, 0, field.Width, field.Height);
                    float xScale = bounds.Width() / 200;
                    float yScale = bounds.Height() / 200;
                    float newY = (thebuns.VerticalLoc + 100) * yScale;
                    float bunsSizeBase = (float)BunnySizeForLevel(thebuns.BunnySize);
                    double nextLevelSize = BunnySizeForLevel(thebuns.BunnySize + 1);
                    float deltaSize = (float)((nextLevelSize - bunsSizeBase) * thebuns.Progress);
                    // todo:  z-order the views
                    float scale = 0.5f + 0.4f * (((float)thebuns.VerticalLoc + 100) / (float)200);

                    // animate to the new location
                    AnimationSet newAnimation = new AnimationSet(true);
                    TranslateAnimation a = new TranslateAnimation(Dimension.RelativeToSelf, 0, Dimension.Absolute, (thebuns.HorizontalLoc + 100) * xScale,
                        Dimension.RelativeToSelf, 0, Dimension.Absolute, newY);
                    a.Duration = 100;
                    newAnimation.AddAnimation(a);

                    ScaleAnimation s = new ScaleAnimation(1, 1, scale, scale);
                    s.Duration = 100;
                    newAnimation.AddAnimation(s);
                    newAnimation.FillAfter = true;
                    theGraphic.Button.StartAnimation(newAnimation);

                    

                });
            }
        }



        public double BunnySizeForLevel(int level)
        {
            return _bunBaseSize + (_bunSizePerLevel * level);
        }

       
        private void InitGame()
        {
            if (!inited)
            {
                Game.NewPlayerLoaded = false;
                Activity.RunOnUiThread(() =>
                {
                    CarrotImg.Visibility = ViewStates.Gone;
                    // ad bunnies
                    foreach (Bunny curBunny in Game.CurrentPlayer.Bunnies)
                    {
                        AddBunnyToScreen(curBunny);
                    }
                    HideBunnyPanel();
                    UpdateScore();
                    StartTimers();
                    inited = true;
                });
            }
            else if (Game.NewPlayerLoaded)
            {
                // a new player was loaded - get rid of existing bunnies and add new ones.
                Game.NewPlayerLoaded = false;
                Activity.RunOnUiThread(() =>
                {
                    CarrotImg.Visibility = ViewStates.Gone;
                    HideBunnyPanel();
                    UpdateScore();
                    field.RemoveAllViews();
                    _bunnyGraphicList.Clear();
                    foreach (Bunny curBunny in Game.CurrentPlayer.Bunnies)
                    {
                        AddBunnyToScreen(curBunny);
                    }

                });

            }
        }

        private void SetCurrentBunny(Bunny newBuns)
        {
            if (_currentBuns != null)
                DeselectBunny(_currentBuns);
            _currentBuns = newBuns;
            if (newBuns != null)
            {
                SelectBunny(newBuns);
                ShowBunnyPanel();
            }
            else
            {
                HideBunnyPanel();

            }
        }

        private void SelectBunny(Bunny theBuns)
        {
            Activity.RunOnUiThread(() => 
            {
                Bitmap[] imgList = SpriteManager.GetImageList(theBuns, "idle", "front");
                ImageView bunBtn = _bunnyGraphicList.Find(b => b.LinkedBuns == theBuns).Button;

                if (bunBtn != null)
                {
                    field.BringChildToFront(bunBtn);
                    bunBtn.SetImageBitmap(imgList[0]);
                }
            });
        }

        private void DeselectBunny(Bunny theBuns)
        {
            Activity.RunOnUiThread(() => {

            });
        }

        private void ShowBunnyPanel()
        {
            Activity.RunOnUiThread(() => {
                UpdateBunnyPanel();
                if (detailView.Visibility == ViewStates.Gone)
                {
                    detailView.Alpha = 0;
                    detailView.Visibility = ViewStates.Visible;
                    AnimationSet animation = new AnimationSet(true);
                    AlphaAnimation alpha = new AlphaAnimation(0, 1);
                    alpha.Duration = 500;
                    animation.AnimationEnd += (s,e) =>
                    {
                        detailView.Alpha = 1;
                    };
                    detailView.StartAnimation(animation);
                    
                }
            });
        }

     

        private void UpdateBunnyPanel()
        {
            if (_currentBuns != null)
            {
                Activity.RunOnUiThread(() => {
                    string nameStr = _currentBuns.BunnyName;
                    if (string.IsNullOrEmpty(nameStr))
                        nameStr = Resource.String.Unamed_Bunny.Localize();
                    bunnyNameLabel.Text = nameStr;
                    bunnyDescLabel.Text = _currentBuns.Description;
                    progressBar.Max = _currentBuns.CarrotsForNextSize(_currentBuns.BunnySize);
                    progressBar.Progress = _currentBuns.FeedState;
                });
            }
        }

        private void HideBunnyPanel()
        {
            if (detailView.Visibility == ViewStates.Visible)
            {
                Activity.RunOnUiThread(() => {
                    AnimationSet animation = new AnimationSet(true);
                    AlphaAnimation alpha = new AlphaAnimation(1, 0);
                    alpha.Duration = 250;
                    animation.AnimationEnd += (s, e) =>
                    {
                        detailView.Visibility = ViewStates.Gone;
                        detailView.Alpha = 0;
                    };
                    detailView.StartAnimation(animation);
                });
            }
        }

        private void StartTimers()
        {
            _idleTimer.Interval = 500;
            _idleTimer.AutoReset = false;
            _idleTimer.Elapsed += (object sender, ElapsedEventArgs e) => {
                MaybeBunniesHop();
            };
            _idleTimer.Start();
        }

        private void MaybeBunniesHop()
        {
            if ((_bunnyGraphicList.Count > 0) && (Game.Rnd.Next(100) < kBunnyHopChance))
            {
                int whichBunny = Game.Rnd.Next(_bunnyGraphicList.Count);
                BunnyGraphic bunsGraphic = _bunnyGraphicList[whichBunny];
                if ((bunsGraphic.LinkedBuns == _currentBuns) && givingCarrot)
                {
                    // don't jump when eating
                    _idleTimer.Start();
                }
                else
                    DoBunnyHop(bunsGraphic);

            }
            else
            {
                _idleTimer.Start();
            }
        }

        private void DoBunnyHop(BunnyGraphic buns)
        {
            int dir = Game.Rnd.Next(8);
            int xDif = 0, yDif = 0;
            int verticalHop = Game.Rnd.Next(kVerticalHopMin, kVerticalHopMax);
            int horizontalHop = Game.Rnd.Next(kHorizontalHopMin, kHorizontalHopMax);
            Bitmap[] bunnyJumpImageFrames = null;
            Bitmap[] bunnyIdleImageFrames = null;
            bool flip = false;

            switch (dir)
            {
                case 0://up
                    yDif = -verticalHop;
                    bunnyJumpImageFrames = SpriteManager.GetImageList(buns.LinkedBuns, "hop", "back");
                    bunnyIdleImageFrames = SpriteManager.GetImageList(buns.LinkedBuns, "idle", "back");
                    break;
                case 1: //upright
                    yDif = -verticalHop;
                    xDif = horizontalHop;
                    bunnyJumpImageFrames = SpriteManager.GetImageList(buns.LinkedBuns, "hop", "rightback");
                    bunnyIdleImageFrames = SpriteManager.GetImageList(buns.LinkedBuns, "idle", "rightback");
                    break;
                case 2: // right
                    xDif = horizontalHop;
                    bunnyJumpImageFrames = SpriteManager.GetImageList(buns.LinkedBuns, "hop", "right");
                    bunnyIdleImageFrames = SpriteManager.GetImageList(buns.LinkedBuns, "idle", "right");
                    break;
                case 3: // downright
                    yDif = verticalHop;
                    xDif = horizontalHop;
                    bunnyJumpImageFrames = SpriteManager.GetImageList(buns.LinkedBuns, "hop", "rightfront");
                    bunnyIdleImageFrames = SpriteManager.GetImageList(buns.LinkedBuns, "idle", "rightfront");
                    break;
                case 4: // down
                    yDif = verticalHop;
                    bunnyJumpImageFrames = SpriteManager.GetImageList(buns.LinkedBuns, "hop", "front");
                    bunnyIdleImageFrames = SpriteManager.GetImageList(buns.LinkedBuns, "idle", "front");
                    break;
                case 5: // downleft
                    yDif = verticalHop;
                    xDif = -horizontalHop;
                    bunnyJumpImageFrames = SpriteManager.GetImageList(buns.LinkedBuns, "hop", "rightfront");
                    bunnyIdleImageFrames = SpriteManager.GetImageList(buns.LinkedBuns, "idle", "rightfront");
                    flip = true;
                    break;
                case 6:// left
                    xDif = -horizontalHop;
                    bunnyJumpImageFrames = SpriteManager.GetImageList(buns.LinkedBuns, "hop", "right");
                    bunnyIdleImageFrames = SpriteManager.GetImageList(buns.LinkedBuns, "idle", "right");
                    flip = true;
                    break;
                case 7: // upleft
                    yDif = -verticalHop;
                    xDif = -horizontalHop;
                    bunnyJumpImageFrames = SpriteManager.GetImageList(buns.LinkedBuns, "hop", "rightback");
                    bunnyIdleImageFrames = SpriteManager.GetImageList(buns.LinkedBuns, "idle", "rightback");
                    flip = true;
                    break;
            }

            if (bunnyJumpImageFrames == null || bunnyIdleImageFrames == null)
            {

            }


            Activity.RunOnUiThread(() => {
                float scale = 0.5f + (0.5f * (((float)buns.LinkedBuns.VerticalLoc + 100) / 200));
                xDif = (int)((float)xDif * scale);
                yDif = (int)((float)yDif * scale);

                float newX = buns.LinkedBuns.HorizontalLoc + xDif;
                float newY = buns.LinkedBuns.VerticalLoc + yDif;

                if (newX < kMinWidth)
                    newX = kMinWidth;
                else if (newX > kMaxWidth)
                    newX = kMaxWidth;

                if (newY < kMinHeight)
                    newY = kMinHeight;
                else if (newY > kMaxHeight)
                    newY = kMaxHeight;

                scale = 0.5f + (0.5f * ((newY + 100) / 200));

                if (flip)
                    buns.Button.RotationY = 180;
                else
                    buns.Button.RotationY = 0;

                BunnyHopToNewLoc(buns, dir, newX, newY, bunnyJumpImageFrames, bunnyIdleImageFrames);
            });
        }

        private void BunnyHopToNewLoc(BunnyGraphic buns, int dir, float newX, float newY, Bitmap[] jumpFrames, Bitmap[] idleFrames)
        {
            Rect bounds = new Rect(0, 0, field.Width, field.Height);
            float xScale = bounds.Width() / 200;
            float yScale = bounds.Height() / 200;

            Activity.RunOnUiThread(() => {

                buns.Button.SetImageBitmap(jumpFrames[0]);
                // todo: animate bunny
            });
            AnimationSet animation = new AnimationSet(true);
            TranslateAnimation ta = new TranslateAnimation(Dimension.RelativeToSelf, 0, Dimension.RelativeToSelf, 0,
                Dimension.Absolute, (newX + 100) * xScale, Dimension.Absolute, (newY + 100) * yScale);

            ta.Duration = 500;
            animation.AnimationEnd += (s, e) =>
            {
                Activity.RunOnUiThread(() =>
                {
                    buns.Button.SetImageBitmap(idleFrames[0]);
                    // todo - fix zorder after bunny jump
                    buns.LinkedBuns.UpdateLocation((int)newX, (int)newY);
                    _idleTimer.Start();
                    CheckBunnyBreeding();
                });
            };
            animation.FillAfter = true;
            buns.Button.StartAnimation(animation);
        }

        private void CheckBunnyBreeding()
        {
            if (_bunnyGraphicList.Count > 1)
            {
                for (int i = 0; i < _bunnyGraphicList.Count - 1; i++)
                {
                    BunnyGraphic firstBuns = _bunnyGraphicList[i];
                    Rect firstRect = new Rect();
                    firstBuns.Button.GetGlobalVisibleRect(firstRect);
                    for (int j = 1; j < _bunnyGraphicList.Count; j++)
                    {
                        BunnyGraphic secondBuns = _bunnyGraphicList[j];
                        Rect secondRect = new Rect();
                        secondBuns.Button.GetGlobalVisibleRect(secondRect);

                        if (secondRect.Intersect(firstRect))
                        {
                            if (Bunny.BunniesCanBreed(secondBuns.LinkedBuns, firstBuns.LinkedBuns))
                            {
                                HappyBuns(firstBuns.LinkedBuns);
                                // todo - should they breed??
                            }
                        }
                    }
                }
            }
        }

        private void SetBunnyDirectionGraphic(BunnyGraphic buns, int dir)
        {
            Activity.RunOnUiThread(() => {
                // to do - change the bunny to face the direction
            });
        }

        private void SetBunnyIdleGraphic(BunnyGraphic buns)
        {
            Activity.RunOnUiThread(() => {
                // to do - change the bunny to a random idle state
            });
        }

        private void PauseTimers()
        {
            _idleTimer.Stop();
        }

        private void ResumeTimers()
        {
            _idleTimer.Start();
        }

        private void UpdateScore()
        {
            Activity.RunOnUiThread(() => {
                //CarrotCount.Text = Game.CurrentPlayer.carrotCount.ToString();
                this.Activity.Title = Game.CurrentPlayer.carrotCount.ToString() + " carrots";
            });
        }

        private void MaybeGiveCarrot(Bunny theBuns)
        {

            if ((Game.CurrentPlayer.carrotCount > 0) && !givingCarrot)
            {
                givingCarrot = true;
                // ok give one
                Activity.RunOnUiThread(() => {
                    feedBtn.Enabled = false;
                    CarrotImg.Visibility = ViewStates.Visible;
                    field.BringChildToFront(CarrotImg);
                    UpdateScore();
                    Bitmap[] imageList = SpriteManager.GetImageList(theBuns, "idle", "front");
                    ImageView bunBtn = _bunnyGraphicList.Find(b => b.LinkedBuns == theBuns).Button;

                    if (bunBtn != null)
                    {
                        field.BringChildToFront(bunBtn);
                       
                        bunBtn.SetImageBitmap(imageList[0]);
                        bool grew = Game.CurrentPlayer.FeedBunny(theBuns);
                        AnimateBunsSizeAndLocation(theBuns, grew);
                    }
                });

                

            }
        }

        private void AnimateBunsSizeAndLocation(Bunny thebuns, bool grew)
        {
            BunnyGraphic theGraphic = _bunnyGraphicList.Find(b => b.LinkedBuns == thebuns);
            field.BringChildToFront(CarrotImg);
            if (theGraphic != null)
            {
                float bunsSizeBase = (float)BunnySizeForLevel(thebuns.BunnySize);
                double nextLevelSize = BunnySizeForLevel(thebuns.BunnySize + 1);
                float deltaSize = (float)((nextLevelSize - bunsSizeBase) * thebuns.Progress);
                CarrotImg.Alpha = 1;

                FrameLayout.LayoutParams layout = new FrameLayout.LayoutParams(96, 96);
                layout.LeftMargin = theGraphic.Button.Left + theGraphic.Button.Width / 2;
                layout.TopMargin = theGraphic.Button.Top + theGraphic.Button.Height / 2;

                CarrotImg.LayoutParameters = layout;

                double duration = 0;
                if (grew)
                    duration = 4;

                AnimationSet animateCarrot = new AnimationSet(true);
                ScaleAnimation scale = new ScaleAnimation(1, 1, 0, 0);
                scale.Duration = 1000;
                animateCarrot.AddAnimation(scale);
                AlphaAnimation alpha = new AlphaAnimation(1, 0);
                alpha.Duration = 1000;
                animateCarrot.AddAnimation(alpha);
                animateCarrot.FillAfter = true;
                animateCarrot.AnimationEnd += (s, e) =>
                {
                    Activity.RunOnUiThread(() =>
                    {
                        CarrotImg.Visibility = ViewStates.Gone;
                        CarrotImg.Alpha = 1;

                        // now animate the bunny growing
                        AnimationSet sizeAnimator = new AnimationSet(true);
                        /*
                        ViewPropertyAnimator
                        UIView.Animate(duration, () => {
                            theGraphic.Height.Constant = bunsSizeBase;
                            theGraphic.Width.Constant = bunsSizeBase + deltaSize;
                            PlayfieldView.LayoutIfNeeded();
                        }, () => {
                            InvokeOnMainThread(() => {
                                UpdateBunsSizeAndLocation(thebuns);
                                UpdateBunnyPanel();
                                givingCarrot = false;
                                FeedBunnyBtn.Enabled = true;
                            });
                        });
                        */

                    });
                };
                CarrotImg.StartAnimation(animateCarrot);
            }
        }
    }
}

