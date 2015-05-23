using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;


//using Emgu.Util;

using System.Drawing.Imaging;
using System.Windows;
using System.Drawing;
using Microsoft.Kinect;
using Emgu.CV;
using Emgu.CV.Structure;
using System.Windows.Media.Imaging;
using System.Windows.Media;
using System.Runtime.InteropServices;

namespace IRS
{
    public static class ImageHelper
    {

        public static Image<Bgr, Byte> cropFace(ColorImageFrame colorFrame, Skeleton skeleton)
        {

            CoordinateMapper mapper = KinectSensor.KinectSensors.FirstOrDefault(x => x.Status == KinectStatus.Connected && x.SkeletonStream.IsEnabled == true).CoordinateMapper;

            ColorImagePoint head = mapper.MapSkeletonPointToColorPoint(skeleton.Joints[JointType.Head].Position, colorFrame.Format);
            ColorImagePoint shoulderCenter = mapper.MapSkeletonPointToColorPoint(skeleton.Joints[JointType.ShoulderCenter].Position, colorFrame.Format);
            ColorImagePoint shoulderLeft = mapper.MapSkeletonPointToColorPoint(skeleton.Joints[JointType.ShoulderLeft].Position, colorFrame.Format);
            ColorImagePoint shoulderRight = mapper.MapSkeletonPointToColorPoint(skeleton.Joints[JointType.ShoulderRight].Position, colorFrame.Format);


            float headWidth = (shoulderRight.X - shoulderLeft.X) / 3;
            float headHeight = (shoulderCenter.Y - head.Y) * 3/2;
            float maxSize = Math.Max(headHeight, headWidth) * 1.10f;

            Int32Rect cropRect = new Int32Rect((int)(head.X - maxSize / 2), (int)(head.Y - maxSize * 2/ 5), (int)maxSize, (int)maxSize);

            if (cropRect.X > 0 && cropRect.Y > 0 && cropRect.Width > 0 && cropRect.Height > 0)
            {

                byte[] pixelData = new byte[colorFrame.PixelDataLength];
                colorFrame.CopyPixelDataTo(pixelData);


                BitmapSource bitmapsource = BitmapSource.Create(
                        colorFrame.Width, colorFrame.Height,
                        96, 96, PixelFormats.Bgr32,
                        null, pixelData,
                        colorFrame.Width * colorFrame.BytesPerPixel);
                 Bitmap bitmap = null;
                try
                {
                     bitmap = bitmapSourceToBitmap(new CroppedBitmap(bitmapsource, cropRect));
                }
                catch (Exception e) {                 }
                Image<Bgr, byte> tarImage = new Image<Bgr, byte>(bitmap);
                return tarImage;
            }

            else
                return null;        }


        private static Bitmap bitmapSourceToBitmap(BitmapSource source)
        {
            if (source != null)
            {
                Bitmap bmp = new Bitmap(source.PixelWidth, source.PixelHeight, System.Drawing.Imaging.PixelFormat.Format32bppPArgb);
                BitmapData data = bmp.LockBits(new System.Drawing.Rectangle(System.Drawing.Point.Empty, bmp.Size), System.Drawing.Imaging.ImageLockMode.WriteOnly, System.Drawing.Imaging.PixelFormat.Format32bppPArgb);
                source.CopyPixels(Int32Rect.Empty, data.Scan0, data.Height * data.Stride, data.Stride);
                bmp.UnlockBits(data);
                return bmp;
            }
            else
                return null;

        }

        public static System.Drawing.Bitmap ToBitmap(this BitmapSource bitmapsource)
        {
            System.Drawing.Bitmap bitmap;
            using (var outStream = new MemoryStream())
            {
                // from System.Media.BitmapImage to System.Drawing.Bitmap
                BitmapEncoder enc = new BmpBitmapEncoder();
                enc.Frames.Add(BitmapFrame.Create(bitmapsource));
                enc.Save(outStream);
                bitmap = new System.Drawing.Bitmap(outStream);
                return bitmap;
            }
        }
        public static BitmapSource ToBitmapSource(IImage image)
        {
            if (image != null)
            {
                using (System.Drawing.Bitmap source = image.Bitmap)
                {
                    IntPtr ptr = source.GetHbitmap(); //obtain the Hbitmap

                    BitmapSource bs = System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(
                        ptr,
                        IntPtr.Zero,
                        Int32Rect.Empty,
                        System.Windows.Media.Imaging.BitmapSizeOptions.FromEmptyOptions());

                    DeleteObject(ptr); //release the HBitmap
                    return bs;
                }
            }
            else
                return null;
        }

        [DllImport("gdi32")]
        private static extern int DeleteObject(IntPtr o);

    }

}
