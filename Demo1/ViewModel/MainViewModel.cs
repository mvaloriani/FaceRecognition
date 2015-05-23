using GalaSoft.MvvmLight;
using System;
using System.Windows.Media.Imaging;
using Microsoft.ProjectOxford.Face;
using Microsoft.ProjectOxford.Face.Contract;
using System.Windows.Media;
using System.Threading.Tasks;
using System.Windows;
using System.IO;
using System.Drawing;
using System.Linq;
using FaceRecognitionEMGU;

using System.Xml;
using System.Text;

namespace Demo1.ViewModel
{
    /// <summary>
    /// This class contains properties that the main View can data bind to.
    /// <para>
    /// Use the <strong>mvvminpc</strong> snippet to add bindable properties to this ViewModel.
    /// </para>
    /// <para>
    /// You can also use Blend to data bind with the tool's support.
    /// </para>
    /// <para>
    /// See http://www.galasoft.ch/mvvm
    /// </para>
    /// </summary>
    public class MainViewModel : ViewModelBase
    {

        public const string ImageSourcePathPropertyName = "ImageSourcePath";
        private String _imageSourcePath;
        public String ImageSourcePath
        {
            get { return _imageSourcePath; }
            set
            {
                if (_imageSourcePath == value)
                { return; }

                _imageSourcePath = value;
                RaisePropertyChanged(ImageSourcePathPropertyName);
            }
        }

        public const string EmguResultPropertyName = "EmguResult";
        private String _emguResult;
        public String EmguResult
        {
            get { return _emguResult; }
            set
            {
                if (_emguResult == value)
                { return; }

                _emguResult = value;
                RaisePropertyChanged(EmguResultPropertyName);
            }
        }


        /// <summary>
        /// Initializes a new instance of the MainViewModel class.
        /// </summary>
        public MainViewModel()
        {
            
        }


        #region  Emgu

        EmguFaceDetector emguDetector;

        internal async void Emgu()
        {
            EmguFaceDetector emguDetector = new EmguFaceDetector(); 
            emguDetector.detect(new Bitmap(ImageSourcePath));
        }


        #endregion


        #region Oxford

        public const string OxfordResultPropertyName = "OxfordResult";
        private String _oxfordResult;
        public String OxfordResult
        {
            get { return _oxfordResult; }
            set
            {
                if (_oxfordResult == value)
                { return; }

                _oxfordResult = value;
                RaisePropertyChanged(OxfordResultPropertyName);
            }
        }


        public const string OxfordImageResultPropertyName = "OxfordImageResult";
        private BitmapSource _oxfordImageResult = null;
        public BitmapSource OxfordImageResult
        {
            get { return _oxfordImageResult; }
            set
            {
                if (_oxfordImageResult == value)
                { return; }
                _oxfordImageResult = value;
                RaisePropertyChanged(OxfordImageResultPropertyName);
            }
        }



        private readonly IFaceServiceClient faceServiceClient = new FaceServiceClient("eb03b136ac4247e99e5f9e8587f3580c"); //PrimaryKey

        internal async void Oxford()
        {

            OxfordResult = "Detecting...";

            Bitmap res = new Bitmap(ImageSourcePath);            

            Face[] face = await UploadAndDetectFaces(ImageSourcePath);

            OxfordResult = String.Format("Detection Finished. {0} face(s) detected", face.Length);

            if (face.Length > 0) {

                foreach (var item in face)
                {
                    res = InsertShape(res, "rectangle",
                        item.FaceRectangle.Top, item.FaceRectangle.Left,
                        item.FaceRectangle.Width, item.FaceRectangle.Height,
                        "Red");

                }
            }

            OxfordImageResult = ConvertBitmap(res);

            //{
            //    DrawingVisual visual = new DrawingVisual();
            //    DrawingContext drawingContext = visual.RenderOpen();

            //    drawingContext.DrawImage(bitmapSource,
            //        new Rect(0, 0, bitmapSource.Width, bitmapSource.Height));

            //    double dpi = bitmapSource.DpiX;
            //    double resizeFactor = 96 / dpi;


            //    foreach (var faceRect in faceRects)
            //    {
            //        drawingContext.DrawRectangle(
            //            System.Windows.Media.Brushes.Transparent,
            //            new Pen(Brushes.Red, 2),
            //            new Rect(
            //                faceRect.Left * resizeFactor,
            //                faceRect.Top * resizeFactor,
            //                faceRect.Width * resizeFactor,
            //                faceRect.Height * resizeFactor
            //                )
            //        );
            //    }

            //    drawingContext.Close();

            //    RenderTargetBitmap faceWithRectBitmap = new RenderTargetBitmap(
            //        (int)(bitmapSource.PixelWidth * resizeFactor),
            //        (int)(bitmapSource.PixelHeight * resizeFactor),
            //        96,
            //        96,
            //        PixelFormats.Pbgra32);

            //    faceWithRectBitmap.Render(visual);

            //    OxfordImageResult.source = faceWithRectBitmap;
            //}
        }

      
        private async Task<Face[]> UploadAndDetectFaces(string imageFilePath)
        {
            try
            {
                using (Stream imageFileStream = File.OpenRead(imageFilePath))
                {
                    var faces = await faceServiceClient.DetectAsync(imageFileStream);

                    return faces.ToArray();
                }
            }
            catch (Exception e)
            {
                return new Face[0];
            }

        }

        #endregion


        #region Betaface

        public const string BetafaceResultPropertyName = "BetafaceResult";
        private String _betafaceResult;
        public String BetafaceResult
        {
            get { return _betafaceResult; }
            set
            {
                if (_betafaceResult == value)
                { return; }

                _betafaceResult = value;
                RaisePropertyChanged(BetafaceResultPropertyName);
            }
        }



        internal void Betaface()
        {

            if (ImageSourcePath != null)
            {
                BetafaceResult = "Detecting...";

                BetafaceServiceConnection conn = new BetafaceServiceConnection();

                Task betafaceTask = new Task(
                        () =>
                        {
                            Image userImage = Image.FromFile(ImageSourcePath);

                            BetafaceResult = FormatXml(conn.GetUserInfo(userImage));
                        });

                betafaceTask.Start();
            }
            else
            {
                BetafaceResult = "No user image available";
            }
        }

        public string FormatXml(string xml)
        {
            var doc = new XmlDocument();
            doc.LoadXml(xml);
            var stringBuilder = new StringBuilder();
            var xmlWriterSettings = new XmlWriterSettings { Indent = true, OmitXmlDeclaration = true };
            doc.Save(XmlWriter.Create(stringBuilder, xmlWriterSettings));
            return stringBuilder.ToString();
        }



        #endregion


        internal void Initialize()
        {
            EmguFaceDetector emguDetector = new EmguFaceDetector(); 

        }

        internal void SelectImage()
        {
            // Create OpenFileDialog 
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();

            // Set filter for file extension and default file extension 
            dlg.DefaultExt = ".png";
            dlg.Filter = "Image files (*.jpg, *.jpeg, *.jpe, *.jfif, *.png) | *.jpg; *.jpeg; *.jpe; *.jfif; *.png";


            // Display OpenFileDialog by calling ShowDialog method 
            Nullable<bool> result = dlg.ShowDialog();


            // Get the selected file name and display in a TextBox 
            if (result == true)
            {
                // Open document 
                string filename = dlg.FileName;
                ImageSourcePath = filename;
            }
        }





        public Bitmap InsertShape(Bitmap image, string shapeType, int xPosition, int yPosition, int width, int height, string colorName)
        {
          
            Bitmap bmap = (Bitmap)image.Clone();

            Graphics gr = Graphics.FromImage(bmap);

            if (string.IsNullOrEmpty(colorName))
                colorName = "Black";
            System.Drawing.Pen pen = new System.Drawing.Pen(System.Drawing.Color.FromName(colorName));
            switch (shapeType.ToLower())
            {
                case "filledellipse":
                    gr.FillEllipse(pen.Brush, xPosition,
                yPosition, width, height);
                    break;
                case "filledrectangle":
                    gr.FillRectangle(pen.Brush, xPosition,
                yPosition, width, height);
                    break;
                case "ellipse":
                    gr.DrawEllipse(pen, xPosition, yPosition, width, height);
                    break;
                case "rectangle":
                default:
                    gr.DrawRectangle(pen, xPosition, yPosition, width, height);
                    break;

            }
            return (Bitmap)bmap.Clone();
        }

        public static BitmapSource ConvertBitmap(Bitmap source)
        {
            return System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(
                          source.GetHbitmap(),
                          IntPtr.Zero,
                          Int32Rect.Empty,
                          BitmapSizeOptions.FromEmptyOptions());
        }

        public static Bitmap BitmapFromSource(BitmapSource bitmapsource)
        {
            Bitmap bitmap;
            using (var outStream = new MemoryStream())
            {
                BitmapEncoder enc = new BmpBitmapEncoder();
                enc.Frames.Add(BitmapFrame.Create(bitmapsource));
                enc.Save(outStream);
                bitmap = new Bitmap(outStream);
            }
            return bitmap;
        }
       
    }
}