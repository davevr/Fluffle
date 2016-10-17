// WARNING
//
// This file has been generated automatically by Xamarin Studio to store outlets and
// actions made in the UI designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using Foundation;
using System.CodeDom.Compiler;

namespace Fluffimax.iOS
{
	[Register ("BunnyShopViewController")]
	partial class BunnyShopViewController
	{
		[Outlet]
		UIKit.UICollectionView AdoptionCollection { get; set; }

		[Outlet]
		UIKit.UIButton BreedBtn { get; set; }

		[Outlet]
		UIKit.UILabel CarrotCountLabel { get; set; }

		[Outlet]
		UIKit.UIButton EyesBtn { get; set; }

		[Outlet]
		UIKit.UIButton FilterBtn { get; set; }

		[Outlet]
		UIKit.UIView FilterPanel { get; set; }

		[Outlet]
		UIKit.NSLayoutConstraint FilterPanelHeight { get; set; }

		[Outlet]
		UIKit.UILabel FilterResultText { get; set; }

		[Outlet]
		UIKit.UIButton FurBtn { get; set; }

		[Outlet]
		UIKit.UIButton GenderBtn { get; set; }

		[Outlet]
		UIKit.UIButton PriceBtn { get; set; }

		[Outlet]
		UIKit.UIButton SizeBtn { get; set; }
		
		void ReleaseDesignerOutlets ()
		{
			if (AdoptionCollection != null) {
				AdoptionCollection.Dispose ();
				AdoptionCollection = null;
			}

			if (BreedBtn != null) {
				BreedBtn.Dispose ();
				BreedBtn = null;
			}

			if (CarrotCountLabel != null) {
				CarrotCountLabel.Dispose ();
				CarrotCountLabel = null;
			}

			if (EyesBtn != null) {
				EyesBtn.Dispose ();
				EyesBtn = null;
			}

			if (FilterBtn != null) {
				FilterBtn.Dispose ();
				FilterBtn = null;
			}

			if (FilterPanel != null) {
				FilterPanel.Dispose ();
				FilterPanel = null;
			}

			if (FilterResultText != null) {
				FilterResultText.Dispose ();
				FilterResultText = null;
			}

			if (FurBtn != null) {
				FurBtn.Dispose ();
				FurBtn = null;
			}

			if (GenderBtn != null) {
				GenderBtn.Dispose ();
				GenderBtn = null;
			}

			if (PriceBtn != null) {
				PriceBtn.Dispose ();
				PriceBtn = null;
			}

			if (SizeBtn != null) {
				SizeBtn.Dispose ();
				SizeBtn = null;
			}

			if (FilterPanelHeight != null) {
				FilterPanelHeight.Dispose ();
				FilterPanelHeight = null;
			}
		}
	}
}
