using System;
using System.Collections.Generic;
using UIKit;
using CoreGraphics;
using Fluffimax.Core;
using Foundation;
using System.Runtime.InteropServices;


namespace Fluffimax.iOS
{
	public class ShapedImageView : UIImageView
	{
		private CGPoint _previousPoint;
		private bool _previousPointInsideResult;
		public int shapedPixelTolerance = 4;
		public nfloat shapedTransparentMaxAlpha = 0.5f;

		public UIImage ImageFromUIImageView()
		{
			UIGraphics.BeginImageContextWithOptions(this.Bounds.Size, true, 0.0f);
			this.Layer.RenderInContext(UIGraphics.GetCurrentContext());
			UIImage resultImage = UIGraphics.GetImageFromCurrentImageContext();
			UIGraphics.EndImageContext();
			return resultImage;
		}

		private UIColor GetPixelColor(CGPoint myPoint, UIImage myImage)
		{
			var rawData = new byte[4];
			var handle = GCHandle.Alloc(rawData);
			UIColor resultColor = null;
			try
			{
				using (var colorSpace = CGColorSpace.CreateDeviceRGB())
				{
					using (var context = new CGBitmapContext(Marshal.UnsafeAddrOfPinnedArrayElement(rawData, 0), 1, 1, 8, 4, colorSpace, CGBitmapFlags.ByteOrder32Big | CGBitmapFlags.PremultipliedLast))
					{
						context.DrawImage(new CGRect(-myPoint.X, -myPoint.Y, myImage.Size.Width, myImage.Size.Height), myImage.CGImage);
						float red = (rawData[0]) / 255.0f;
						float green = (rawData[1]) / 255.0f;
						float blue = (rawData[2]) / 255.0f;
						float alpha = (rawData[3]) / 255.0f;
						resultColor = UIColor.FromRGBA(red, green, blue, alpha);
						Console.WriteLine(string.Format("r:{0} g:{1} b:{2} a:{3}", red, green, blue, alpha));
					}
				}
			}
			finally
			{
				handle.Free();
			}
			return resultColor;
		}

		private bool GetPixelIsTransparent(CGPoint myPoint, UIImage myImage)
		{
			var rawData = new byte[4];
			var handle = GCHandle.Alloc(rawData);
			bool result = false;
			try
			{
				using (var colorSpace = CGColorSpace.CreateDeviceRGB())
				{
					using (var context = new CGBitmapContext(Marshal.UnsafeAddrOfPinnedArrayElement(rawData, 0), 1, 1, 8, 4, colorSpace, CGBitmapFlags.ByteOrder32Big | CGBitmapFlags.PremultipliedLast))
					{
						context.DrawImage(new CGRect(-myPoint.X, -myPoint.Y, myImage.Size.Width, myImage.Size.Height), myImage.CGImage);
						float alpha = (rawData[3]) / 255.0f;
						result = alpha < 0.1;
					}
				}
			}
			finally
			{
				handle.Free();
			}
			return result;
		}


		public override bool PointInside(CGPoint point, UIEvent uievent)
		{
			bool superResult = base.PointInside(point, uievent);
			return superResult;
			if (!superResult) return false;


			if (point.Equals(_previousPoint)) return _previousPointInsideResult;
			    
			_previousPoint = point;


			// Image might be scaled or translated due to contentMode. We want to convert the view point into an image point first.
			UIImage theImage = ImageFromUIImageView();
			CGPoint imagePoint = imagePointFromViewPoint(theImage, point);
			UIColor targetColor = GetPixelColor(imagePoint, theImage);

			bool result = GetPixelIsTransparent(imagePoint, theImage);
			_previousPointInsideResult = result;
			return result;
		}

		private CGPoint imagePointFromViewPoint (UIImage theImage, CGPoint viewPoint)
		{
		    switch (this.ContentMode) {
		        case UIViewContentMode.ScaleToFill:
		        {
		            CGSize imageSize = theImage.Size;
					CGSize boundsSize = this.Bounds.Size;
					viewPoint.X *= (boundsSize.Width != 0) ? (imageSize.Width / boundsSize.Width) : 1;
		            viewPoint.Y *= (boundsSize.Height != 0) ? (imageSize.Height / boundsSize.Height) : 1;
		            return viewPoint;
		        }
		            break;
		        case UIViewContentMode.TopLeft:
		            return viewPoint;
		        default: // TODO: Handle the rest of contentMode values
		            return viewPoint;
		    }
		}

		private bool isAlphaVisibleAtImagePoint(CGPoint point)
		{
		    CGRect queryRect;
		    
	        CGRect imageRect = new CGRect(0, 0, this.Image.Size.Width, this.Image.Size.Height);
			int pointRectWidth = this.shapedPixelTolerance * 2 + 1;
			CGRect pointRect = new CGRect(point.X - this.shapedPixelTolerance, point.Y - this.shapedPixelTolerance, pointRectWidth, pointRectWidth);
			queryRect = CGRect.Intersect(imageRect, pointRect);
	        if (queryRect.IsNull()) return false;
		    

			CGBitmapContext context;

			uint pixelCount = (uint)(queryRect.Width * queryRect.Height);
			byte[] data = new byte[pixelCount]; 
		    
		    // TODO: Wouldn't it be better to use drawInRect:. See: http://stackoverflow.com/questions/15008270/get-alpha-channel-from-uiimage-rectangle
		    CGSize querySize = queryRect.Size;
			uint bytesPerPixel = sizeof(int);
		    const uint bitsPerComponent = 8;
			context = new CGBitmapContext(data,
			                              (nint)querySize.Width,
										  (nint)querySize.Height,
										  (nint)bitsPerComponent,
										  (nint)bytesPerPixel * (nint)querySize.Width,
										  null,
										  CGImageAlphaInfo.Only);
			//data = (byte[])context.Data;
			context.SetBlendMode(CGBlendMode.Copy);
			context.TranslateCTM(-queryRect.Left, queryRect.Top - this.Image.Size.Height);

			context.DrawImage(new CGRect(0, 0, this.Image.Size.Width, this.Image.Size.Height), this.Image.CGImage);

		    for (int i = 0; i<pixelCount; i++)
		    {
				byte alphaChar = data[i];
				nfloat alpha = (nfloat)alphaChar / (nfloat)255;
		        if (alpha > this.shapedTransparentMaxAlpha)
		        {
					//CGContextRelease(context);
					// free(data);
		            return true;
		        }
		    }

			//CGContextRelease(context);

			///free(data);
		    return false;
		}
	}
}

