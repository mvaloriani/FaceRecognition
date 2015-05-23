using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using IRS.Data;
using Microsoft.Kinect;
using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;

namespace IRS
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        DispatcherTimer timer;
        int time = 0;
        long minute;
        int time1 = 0;
        long minute1;

        public MainWindow()
        {
            InitializeComponent();

            inizializeKinect();

            analizer = new Analizer(sensor);
            sessionManager = new SessionManager(sensor, analizer);

            stackPanelUser.DataContext = analizer;

            KinectSensor.KinectSensors.StatusChanged += KinectSensors_StatusChanged;

      

            this.PlotModel = new PlotModel();
            this.PlotModel.Series.Add(new FunctionSeries());
            this.PlotModel.Series.Add(new FunctionSeries());
            this.PlotModel.Series.Add(new FunctionSeries());
            this.plot.DataContext = this;

 

        }

        void analizer_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "LastRotation")
            {
                LabelActualHeadRotationUser1.Content = "X:" + analizer.LastRotation.X
                    + "Y:" + analizer.LastRotation.Y + "Z:" + analizer.LastRotation.Z;
            }
        }



        #region Fields and Proprieties


        private WriteableBitmap colorImageBitmap;


        private Int32Rect colorImageBitmapRect;
        private int colorImageStride;

        private Skeleton[] skeletons;

        private KinectSensor sensor;


        private CoordinateMapper kinectMapper;
        private bool AngleSet = false;

        private short[] depthByte;
        private byte[] colorByte;
        private byte[] pixelData;

        GlobalDataSTS g = new GlobalDataSTS();

        private Analizer analizer;
        private SessionManager sessionManager;


        #endregion


        #region kinect methods

        void sensor_AllFramesReady(object sender, AllFramesReadyEventArgs e)
        {
            using (ColorImageFrame cframe = e.OpenColorImageFrame())
            {
                using (DepthImageFrame dframe = e.OpenDepthImageFrame())
                {
                    using (SkeletonFrame sframe = e.OpenSkeletonFrame())
                    {
                        if (sframe != null && cframe != null)
                        {

                            sframe.CopySkeletonDataTo(skeletons);
                            cframe.CopyPixelDataTo(pixelData);


                            colorImageBitmap.WritePixels(colorImageBitmapRect, pixelData, colorImageStride, 0);

                            draw(skeletons);

                            sessionManager.RefreshUser(skeletons);
                            sessionManager.Analizer.Update(skeletons, cframe, dframe);


                        }
                    }
                }
            }
        }


        private void setCameraAngle(Skeleton s)
        {
            if (s.TrackingState == SkeletonTrackingState.Tracked)
            {
                DepthImagePoint headPoint = kinectMapper.MapSkeletonPointToDepthPoint(s.Joints[JointType.Head].Position, sensor.DepthStream.Format);
                DepthImagePoint kneePoint = kinectMapper.MapSkeletonPointToDepthPoint(s.Joints[JointType.KneeLeft].Position, sensor.DepthStream.Format);

                if (headPoint.Y > sensor.DepthStream.FrameHeight / 4 && sensor.ElevationAngle > sensor.MaxElevationAngle)
                {
                    sensor.ElevationAngle += 2;
                    return;
                }

                if (sensor.ElevationAngle > sensor.MinElevationAngle && (headPoint.Y <= 0 || kneePoint.Y > sensor.DepthStream.FrameHeight / 2))
                {
                    sensor.ElevationAngle -= 2;
                    return;
                }
                AngleSet = true;
            }
            if (s.TrackingState == SkeletonTrackingState.PositionOnly)
            {
                DepthImagePoint positionPoint = kinectMapper.MapSkeletonPointToDepthPoint(s.Position, sensor.DepthStream.Format);
                if (sensor.ElevationAngle > sensor.MaxElevationAngle && positionPoint.Y > 3 * sensor.DepthStream.FrameHeight / 4)
                {
                    sensor.ElevationAngle += 2;
                    return;
                }

                if (sensor.ElevationAngle < sensor.MinElevationAngle && positionPoint.Y > sensor.DepthStream.FrameHeight / 4)
                {
                    sensor.ElevationAngle -= 2;
                    return;
                }
                AngleSet = true;
            }

        }


        void draw(Skeleton[] skeletons)
        {
            CanvasSkeletons.Children.Clear();
            foreach (Skeleton s in skeletons.Where(s => s.TrackingState != SkeletonTrackingState.NotTracked))
            {

                //IIT.VEP.KinectManager.KinectSensorUtility.DrawCompleteSkeleton(s,
                //    s.TrackingState == SkeletonTrackingState.Tracked ? 
                //            new SolidColorBrush(Color.FromRgb(255, 0, 0)) : new SolidColorBrush(Color.FromRgb(0, 255, 0)),
                //    CanvansSkeletons);

                Ellipse ell = new Ellipse();
                ell.Width = 10;
                ell.Height = 10;
                ell.Fill = s.TrackingState == SkeletonTrackingState.Tracked ?
                    new SolidColorBrush(Color.FromRgb(255, 0, 0)) :
                    new SolidColorBrush(Color.FromRgb(0, 255, 0));

                CanvasSkeletons.Children.Add(ell);

                ColorImagePoint p = kinectMapper.MapSkeletonPointToColorPoint(s.Position, ColorImageFormat.InfraredResolution640x480Fps30);

                Canvas.SetTop(ell, p.Y - ell.Height / 2);
                Canvas.SetLeft(ell, p.X - ell.Width / 2);

            }

        }

        void KinectSensors_StatusChanged(object sender, StatusChangedEventArgs e)
        {
            switch (e.Status)
            {
                case KinectStatus.Connected:
                    Debug.WriteLine("new kinect connected");
                    if (sensor == null)
                    {
                        sensor = e.Sensor;
                        Debug.WriteLine("switch to new device");
                    }
                    break;
                case KinectStatus.DeviceNotGenuine:
                    break;
                case KinectStatus.DeviceNotSupported:
                    break;
                case KinectStatus.Disconnected:
                    if (sensor == e.Sensor)
                    {
                        unizializeKinect();
                        sensor = KinectSensor.KinectSensors.FirstOrDefault(x => x.Status == KinectStatus.Connected);
                        if (sensor == null)
                        {
                            Debug.WriteLine("all sensors disconnected");
                        }
                    }
                    break;
                case KinectStatus.Error:
                    break;
                case KinectStatus.Initializing:
                    break;
                case KinectStatus.InsufficientBandwidth:
                    break;
                case KinectStatus.NotPowered:
                    break;
                case KinectStatus.NotReady:
                    break;
                case KinectStatus.Undefined:
                    break;
                default:
                    break;
            }
        }

        private void inizializeKinect()
        {
            try
            {
                sensor = KinectSensor.KinectSensors.First(x => x.Status == KinectStatus.Connected);
            }
            catch (Exception e) { }

            if (sensor != null)
            {
                sensor.ColorStream.Enable(ColorImageFormat.RgbResolution640x480Fps30);
                sensor.DepthStream.Enable(DepthImageFormat.Resolution640x480Fps30);
                sensor.SkeletonStream.Enable(new TransformSmoothParameters()
                {
                    Smoothing = 0.5f,
                    Correction = 0.5f,
                    Prediction = 0.5f,
                    JitterRadius = 0.05f,
                    MaxDeviationRadius = 0.04f
                });
                sensor.SkeletonStream.TrackingMode = SkeletonTrackingMode.Default;
                sensor.SkeletonStream.AppChoosesSkeletons = true;

                sensor.Start();



                colorImageBitmap = new WriteableBitmap(
                    sensor.ColorStream.FrameWidth,
                    sensor.ColorStream.FrameWidth,
                    96, 96,
                    PixelFormats.Bgr32, null);

                this.ImageColor.Source = colorImageBitmap;

                colorImageBitmapRect = new Int32Rect(0, 0,
                    sensor.ColorStream.FrameWidth,
                    sensor.ColorStream.FrameHeight);

                colorImageStride = sensor.ColorStream.FrameWidth * sensor.ColorStream.FrameBytesPerPixel;




                skeletons = new Skeleton[sensor.SkeletonStream.FrameSkeletonArrayLength];
                depthByte = new short[sensor.DepthStream.FramePixelDataLength];
                colorByte = new byte[sensor.ColorStream.FramePixelDataLength];
                pixelData = new byte[sensor.ColorStream.FramePixelDataLength];


                KinectSensor.KinectSensors.StatusChanged += KinectSensors_StatusChanged;
                sensor.AllFramesReady += sensor_AllFramesReady;


                kinectMapper = new CoordinateMapper(sensor);
            }

        }

        private void unizializeKinect()
        {
            sensor.Stop();
            sensor.Dispose();
            sensor = null;

        }

        #endregion


        void sessionManager_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case "UserTimer":
                    {
                        if (time != 0)
                        {
                            time = 0;
                            timer.Stop();
                        }
                        else timer.Start();
                        break;
                    }
                case "SessionTimer":
                    {
                        if (time1 != 0)
                        {
                            time = 0;
                            time1 = 0;
                            timer.Stop();
                        }
                        else timer.Start();
                                            
                        break;
                    }
                default:
                    break;
            }

        }



        void timer_Tick(object sender, EventArgs e)
        {
            minute = minute + time / 60;
            time = time % 60;
            labelTimerUser.Content = "" + minute + ":" + time;

            minute1 = minute1 + time1 / 60;
            time1 = time1 % 60;
            labelTimerSession.Content = "" + minute1 + ":" + time1;

            time++;
            time1++;


             this.UpdatePlot();

            plot.RefreshPlot(true);

        }

        public PlotModel PlotModel { get; set; }

        private int maxData=100;

        void UpdatePlot()
        {
            
            var series1 = new LineSeries("Attention") ;
            var series2 = new LineSeries("MovementAvg") ;
            var series3 = new LineSeries("MovementSdv") ;            

            try
            {
                int count1 =analizer.User.AttentionDatas.Count;
                int count2=analizer.User.MovementDatas.Count;
                int min = Math.Min(count1,count2);

                if (min > maxData)
                {
                    for (int i = 0; i < maxData; i++)
                    {
                        series1.Points.Add(new DataPoint(i, analizer.User.AttentionDatas[count1 - maxData + i]));
                        series2.Points.Add(new DataPoint(i, analizer.User.MovementDatas[count2 - maxData + i].avarage));
                        series3.Points.Add(new DataPoint(i, analizer.User.MovementDatas[count2 - maxData + i].stdDev));

                    }
                }
                else
                    for (int i = 0; i < min; i++)
                    {
                        series1.Points.Add(new DataPoint(i, analizer.User.AttentionDatas[i]));
                        series2.Points.Add(new DataPoint(i, analizer.User.MovementDatas[ i].avarage));
                        series3.Points.Add(new DataPoint(i, analizer.User.MovementDatas[ i].stdDev));

                    }


                this.PlotModel.Series[0] = series1;
                this.PlotModel.Series[1] = series2;
                this.PlotModel.Series[2] = series3;

            }
            catch(Exception e) { }
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            sessionManager.Analizer.SetInitialHeadRotation();
        }

        private void checkBoxRefresh_Checked(object sender, RoutedEventArgs e)
        {
                 sessionManager.PropertyChanged += sessionManager_PropertyChanged;
                 analizer.PropertyChanged += analizer_PropertyChanged;

                 timer = new DispatcherTimer();
                 timer.Interval = new TimeSpan(0, 0, 1);
                 timer.Tick += timer_Tick;
                 timer.Start();
        }

        private void checkBoxRefresh_Unchecked(object sender, RoutedEventArgs e)
        {
            sessionManager.PropertyChanged -= sessionManager_PropertyChanged;
            analizer.PropertyChanged -= analizer_PropertyChanged;

            timer.Stop();
        }

    }

}
