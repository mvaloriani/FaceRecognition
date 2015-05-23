using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Microsoft.Kinect;
using Microsoft.Kinect.Toolkit.FaceTracking;
using Point = System.Windows.Point;
using System.Linq;

namespace IRS.Detectors
{

    public class SkeletonFaceTracker : IDisposable
    {

        private FaceTracker faceTracker;

        private bool lastFaceTrackSucceeded;

        private SkeletonTrackingState skeletonTrackingState;

        private DepthImageFormat depthImageFormat;
        private ColorImageFormat colorImageFormat;

        private List<Vector3DF> rotationInTime;
        private List<Vector4> rotationFromeBones;

        private int maxLength = 30;

        public int CountElemets
        {
            get
            {
                return rotationInTime.Count;
            }
        }

        public SkeletonFaceTracker(KinectSensor sensor)
        {

            try
            {
                this.faceTracker = new FaceTracker(sensor);

                colorImageFormat = sensor.ColorStream.Format;
                depthImageFormat = sensor.DepthStream.Format;

                rotationInTime = new List<Vector3DF>();
                rotationFromeBones = new List<Vector4>();
            }
            catch (InvalidOperationException)
            {
                // During some shutdown scenarios the FaceTracker
                // is unable to be instantiated.  Catch that exception
                // and don't track a face.
                Debug.WriteLine("AllFramesReady - creating a new FaceTracker threw an InvalidOperationException");
                this.faceTracker = null;
            }

        }

        public void Clear()
        {
            if (rotationInTime != null)
                rotationInTime.Clear();
        }

        public void Dispose()
        {
            if (this.faceTracker != null)
            {
                this.faceTracker.Dispose();
                this.faceTracker = null;
            }
        }

        public void Update(byte[] colorImage, short[] depthImage, Skeleton skeleton)
        {
            this.skeletonTrackingState = skeleton.TrackingState;
            if (this.skeletonTrackingState != SkeletonTrackingState.Tracked)
            {
                // nothing to do with an untracked skeleton.
                return;
            }

            if (this.faceTracker != null)
            {
                try
                {
                    FaceTrackFrame frame = this.faceTracker.Track(
                         colorImageFormat, colorImage, depthImageFormat, depthImage, skeleton);
                        this.lastFaceTrackSucceeded = frame.TrackSuccessful;
                        if (this.lastFaceTrackSucceeded)
                        {
                            rotationInTime.Add(frame.Rotation);
                        }
                        else {
                            rotationInTime.Add(new Vector3DF(100, 100, 100));
                        }

                        if (rotationInTime.Count > maxLength)
                            rotationInTime.RemoveAt(0);
                    
                }catch(Exception e){

                Debug.WriteLine("problem with facetracker: " + e.Message);
                            }
            // CalculateRotationFromeBone(skeleton);

            //if (rotationFromeBones.Count > maxLength)
            //    rotationFromeBones.RemoveAt(0);

            }

        }

        //private void CalculateRotationFromeBone(Skeleton s)
        //{

        //    float delatz = (s.Joints[JointType.ShoulderLeft].Position.Z - s.Joints[JointType.ShoulderRight].Position.Z);
        //    float delatx = (s.Joints[JointType.ShoulderLeft].Position.X - s.Joints[JointType.ShoulderRight].Position.X);

        //    double angle = Math.Atan(delatz / delatx);

        //    rotationFromeBones.Add(s.BoneOrientations[JointType.Head].HierarchicalRotation.Quaternion);

        //}

        public Vector3DF getLastRotation()
        {

            if (rotationInTime.Count > 0)
            {
                return rotationInTime.Last();
            }

            else
            {
                return new Vector3DF();
            }
        }


        public double CalculateDistraction(float intialPointX = 0, float intialPointY = 0, float intialPointZ = 0)
        {

            //var res = from p in rotationInTime
            //          where (p.X - intialPointX > 20 || p.X - intialPointX < -20 ||
            //                 p.Y - intialPointY > 20 || p.Y - intialPointY < -20 ||
            //                 p.Z - intialPointZ > 20 || p.Z - intialPointZ < -20)
            //          select p;

            int res = rotationInTime.Count(p => p.X - intialPointX > 15 || p.X - intialPointX < -15 ||
                                      p.Y - intialPointY > 15 || p.Y - intialPointY < -15 ) ;

            return res * 1.0 / rotationInTime.Count;

        }


    }

}