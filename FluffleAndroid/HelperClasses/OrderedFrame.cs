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

using Android.Graphics;
using Android.Views.Animations;
using Android.Graphics.Drawables;

namespace Fluffle.AndroidApp
{
	[Register("com.eweware.fluffle.Fluffle.AndroidApp.OrderLayout")]
	public class OrderLayout : FrameLayout
	{
		private List<View> viewList;

		public OrderLayout(Context context) : base(context)
		{
			init();
		}

		public OrderLayout(Context context, IAttributeSet attrs) : base (context, attrs)
		{
			init();
		}

		public OrderLayout(Context context, IAttributeSet attrs, int defStyleAttr) : base (context, attrs, defStyleAttr)
		{
			init();
		}

		private void init()
		{
			this.ChildrenDrawingOrderEnabled = true;
			viewList = new List<View>();
		}


		public override void OnViewAdded(View child)
		{
			base.OnViewAdded(child);
			viewList.Add(child);
			ResortList();
		}

		public override void OnViewRemoved(View child)
		{
			base.OnViewRemoved(child);
			viewList.Remove(child);
			ResortList();
		}

		private void ResortList()
		{
			viewList.Sort((x, y) =>
			{
				int xTag = 0, yTag = 0;
				if (x.Tag != null)
					xTag = (int)x.Tag;
				if (y.Tag != null)
					yTag = (int)y.Tag;

				return xTag.CompareTo(yTag);
			});
		}

		protected override int GetChildDrawingOrder(int childCount, int i)
		{
			View targetView = viewList[i];
            int newIndex =  this.IndexOfChild(targetView);

            if (newIndex < 0)
            {
                newIndex = 0;
                System.Console.WriteLine("undershot index");
            }
            else if (newIndex >= ChildCount)
            { 
                newIndex = ChildCount;
                System.Console.WriteLine("overshot index");
            }
            return newIndex;
		}
	}
}
