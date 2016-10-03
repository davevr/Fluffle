
using Android.Views;
using Android.Views.Animations;
using Android.Widget;


namespace Fluffle.AndroidApp
{
    public class ResizeAnimation : Animation
    {
        int startWidth;
        int startHeight;
        int targetWidth;
        int targetHeight;
        View targetView;

        public ResizeAnimation(View view, int theWidth, int theHeight)
        {
            targetView = view;
            startWidth = view.Width;
            startHeight = view.Height;
            targetWidth = theWidth;
            targetHeight = theHeight;
        }

        public override void Initialize(int width, int height, int parentWidth, int parentHeight)
        {
            base.Initialize(width, height, parentWidth, parentHeight);
        }

        protected override void ApplyTransformation(float interpolatedTime, Transformation t)
        {
            int newWidth = (int)(startWidth + (targetWidth - startWidth) * interpolatedTime);
            int newHeight = (int)(startHeight + (targetHeight - startHeight) * interpolatedTime);
            var layout = targetView.LayoutParameters;
            layout.Width = newWidth;
            layout.Height = newHeight;
            targetView.RequestLayout();
        }

        

        public override bool WillChangeBounds()
        {
            return true;
        }


    }

    public class MoveAnimation : Animation
    {
        int startX;
        int startY;
        int targetX;
        int targetY;
        View targetView;

        public MoveAnimation(View view, int theX, int theY)
        {
            targetView = view;
            startX = view.Left;
            startY = view.Top;
            targetX = theX;
            targetY = theY;
        }

        public override void Initialize(int width, int height, int parentWidth, int parentHeight)
        {
            base.Initialize(width, height, parentWidth, parentHeight);
        }

        protected override void ApplyTransformation(float interpolatedTime, Transformation t)
        {
            int newX = (int)(startX + (targetX - startX) * interpolatedTime);
            int newY = (int)(startY + (targetY - startY) * interpolatedTime);
            var layout = new FrameLayout.LayoutParams(targetView.Width, targetView.Height);

            layout.LeftMargin = newX;
            layout.TopMargin = newY;
            targetView.LayoutParameters = layout;
            targetView.RequestLayout();
        }



        public override bool WillChangeBounds()
        {
            return true;
        }


    }
    public class MoveResizeAnimation : Animation
    {
        int startWidth;
        int startHeight;
        int startX;
        int startY;
        int targetX;
        int targetY;
        int targetWidth;
        int targetHeight;
        View targetView;

        public MoveResizeAnimation(View view, int theX, int theY, int theWidth, int theHeight)
        {
            targetView = view;
            startWidth = view.Width;
            startHeight = view.Height;
            targetWidth = theWidth;
            targetHeight = theHeight;
            startX = view.Left;
            startY = view.Top;
            targetX = theX;
            targetY = theY;
        }

        public override void Initialize(int width, int height, int parentWidth, int parentHeight)
        {
            base.Initialize(width, height, parentWidth, parentHeight);
        }

        protected override void ApplyTransformation(float interpolatedTime, Transformation t)
        {
            int newWidth = (int)(startWidth + (targetWidth - startWidth) * interpolatedTime);
            int newHeight = (int)(startHeight + (targetHeight - startHeight) * interpolatedTime);
            int newX = (int)(startX + (targetX - startX) * interpolatedTime);
            int newY = (int)(startY + (targetY - startY) * interpolatedTime);
            var layout = new FrameLayout.LayoutParams(newWidth, newHeight);
            
            layout.Width = newWidth;
            layout.Height = newHeight;
            layout.LeftMargin = newX;
            layout.TopMargin = newY;
            targetView.LayoutParameters = layout;
            targetView.RequestLayout();
        }



        public override bool WillChangeBounds()
        {
            return true;
        }


    }

}
