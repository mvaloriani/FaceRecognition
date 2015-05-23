using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;
using IRS.Data;
using IRS.Detectors;
using Microsoft.Kinect;
using Microsoft.Kinect.Toolkit.FaceTracking;

namespace IRS
{
    public class Analizer : INotifyPropertyChanged
    {
        #region INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged(String propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
        #endregion


        private UserProfile _user;
        public UserProfile User
        {
            get { return _user; }
            set
            {
                _user = value;
                if (value != null)
                {
                    movementDetector = new BodyActivityWatcher(value.UserTrakingID);
                    faceDetector.Clear();
                    countDynamic = 0;
                    countStatic = 0;
     
                }

                NotifyPropertyChanged("User");
            }
        }

        public Vector3DF LastRotation
        {
            get { return faceDetector!=null? faceDetector.getLastRotation():new Vector3DF(); }
        }


        public double LastAttentionLevel
        {
            get { return (User != null && User.AttentionDatas.Count > 0) ? User.AttentionDatas.Last() : 0; }
        }

              private BodyActivityWatcher movementDetector;
        private AgeDetector ageDetector;
        private GenderDetector genDerdetector;
        private SkeletonFaceTracker faceDetector;

        private Vector3 initiaPosition = new Vector3(0, 0, 0);


        private short[] depthByte;
        private byte[] colorByte;
        private byte[] pixelData;

        private int framesBetweenDynimicAnalisys = 10;
        private int countDynamic = 0;

        private int framesBeforeStaticAnalisys = 150;
        private int countStatic = 0;


        GlobalDataSTS g = new GlobalDataSTS();


        public Analizer(KinectSensor sensor)
        {

            if (sensor != null)
            {

                ageDetector = new AgeDetector();
                //genDerdetector = new GenderDetector();
                faceDetector = new SkeletonFaceTracker(sensor);



                depthByte = new short[sensor.DepthStream.FramePixelDataLength];
                colorByte = new byte[sensor.ColorStream.FramePixelDataLength];
                pixelData = new byte[sensor.ColorStream.FramePixelDataLength];

            
            }
        }

        public void Update(Skeleton[] skeletons, ColorImageFrame colorFrame, DepthImageFrame depthFrame)
        {
            if (User == null || colorFrame == null || depthFrame == null)
                return;

            Skeleton s = skeletons.Where(x => x.TrackingId == User.UserTrakingID).FirstOrDefault();
            if (s == null)
                return;

            movementDetector.Update(s);

            colorFrame.CopyPixelDataTo(colorByte);
            depthFrame.CopyPixelDataTo(depthByte);
            faceDetector.Update(colorByte, depthByte, s);

            if (framesBetweenDynimicAnalisys == countDynamic)
            {
                DinamicAnalisys();
                countDynamic = 0;
            }
            else
                countDynamic++;

            if (User.staticAnalisysPerformed == false && countStatic == framesBeforeStaticAnalisys)
            {
                StaticAnalisys(colorFrame, s);
            }
            else
                countStatic++;

        }

        private void StaticAnalisys(ColorImageFrame colorFrame, Skeleton s)
        {
            User.UserAge = ageDetector.Analize(movementDetector.skeletonTimeSerie.CalculateAvarageSkeleton());
            User.UserImage = ImageHelper.ToBitmapSource(ImageHelper.cropFace(colorFrame, s));
            //  User.UserGender = genderdetector.detectThroughKinect(colorframe, s);

            User.staticAnalisysPerformed = true;

        }

        private void DinamicAnalisys()
        {
            User.AttentionDatas.Add(1 - faceDetector.CalculateDistraction((float)initiaPosition.X, (float)initiaPosition.Y, (float)initiaPosition.Z));
            try
            {
                User.MovementDatas.Add(movementDetector.skeletonTimeSerie.CalculateGlobalData(movementDetector.skeletonTimeSerie.timeList.Count - 1,
                    movementDetector.skeletonTimeSerie.timeList.Count - 30 - 1));
            }
            catch (Exception e) { Debug.WriteLine(e.Message); }
            NotifyPropertyChanged("LastAttentionLevel");
            NotifyPropertyChanged("LastRotation");
        }

        public void Restart()
        {
            faceDetector.Clear();
            movementDetector = null;
        
            User = null;

        }




        internal void SetInitialHeadRotation()
        {
            initiaPosition.X = LastRotation.X;
            initiaPosition.Y = LastRotation.Y;
            initiaPosition.Z = LastRotation.Z;

        }
    }


}
