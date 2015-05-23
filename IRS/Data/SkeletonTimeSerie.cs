using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Kinect;

namespace IRS.Data
{
    public enum BoneType
    {

        HumerusLeft,
        HumerusRight,
        ForearmLeft,
        ForearmRight,
        ClavicleLeft,
        ClavicleRight,
        FemurLeft,
        FemurRight,
        TibiaLeft,
        TibiaRight,
        Neck,
        Spine

    }

    public class SkeletonTimeSerie
    {
        public Dictionary<JointType, List<Joint>> skeletonSerie { get; private set; }

        public List<DateTime> timeList { get; private set; }

        public int TrakingID { get; private set; }

        public int maxSize { get; set; }

        private Dictionary<BoneType, float> _avarageSkeleton;
        private Dictionary<BoneType, float> AvarageSkeleton
        {
            get
            {
                if (_avarageSkeleton == null)
                {
                    _avarageSkeleton = new Dictionary<BoneType, float>();
                }
                return _avarageSkeleton;
            }
            set { _avarageSkeleton = value; }
        }



        public SkeletonTimeSerie(Skeleton s, int maxSize = 0)
        {
            inizialize(s.TrackingId, maxSize);
            Add(s);
        }

        public SkeletonTimeSerie(int trackingId, int maxSize = 0)
        {
            inizialize(trackingId, maxSize);
        }

        private void inizialize(int trackingId, int maxSize = 0)
        {
            skeletonSerie = new Dictionary<JointType, List<Joint>>() { 
          { JointType.AnkleLeft, new List<Joint>()},
          { JointType.AnkleRight, new List<Joint>()},
          { JointType.ElbowLeft, new List<Joint>()},
          { JointType.ElbowRight, new List<Joint>()},
          { JointType.FootLeft, new List<Joint>()},
          { JointType.FootRight, new List<Joint>()},
          { JointType.HandLeft, new List<Joint>()},
          { JointType.HandRight, new List<Joint>()},
          { JointType.Head, new List<Joint>()},
          { JointType.HipCenter, new List<Joint>()},
          { JointType.HipLeft, new List<Joint>()},
          { JointType.HipRight, new List<Joint>()},
          { JointType.KneeLeft, new List<Joint>()},
          { JointType.KneeRight, new List<Joint>()},
          { JointType.ShoulderCenter, new List<Joint>()},
          { JointType.ShoulderLeft, new List<Joint>()},
          { JointType.ShoulderRight, new List<Joint>()},
          { JointType.Spine, new List<Joint>()},
          { JointType.WristLeft, new List<Joint>()},
          { JointType.WristRight, new List<Joint>()}                    
        };

            timeList = new List<DateTime>();

            TrakingID = trackingId;
            this.maxSize = maxSize;
        }



        public void Add(Skeleton s)
        {
            this.Add(s, DateTime.Now);

        }

        public void Add(Skeleton s, DateTime date)
        {
            if (s.TrackingId == this.TrakingID)
            {
                if (maxSize != 0 && timeList.Count >= maxSize)
                {
                    timeList.RemoveAt(0);
                    timeList.Add(date);
                    foreach (var k in skeletonSerie.Keys)
                    {
                        skeletonSerie[k].RemoveAt(0);
                        skeletonSerie[k].Add(s.Joints[k]);
                    }
                }
                else
                {
                    timeList.Add(date);
                    foreach (var k in skeletonSerie.Keys)
                    {
                        skeletonSerie[k].Add(s.Joints[k]);
                    }
                }
            }


            else
                throw new Exception("Skeleton TrakingID not correspond to this SkeletonSeries");
        }



        public Dictionary<JointType, SkeletonPoint> CalculateDelta(int end, int start = 0)
        {
            Dictionary<JointType, SkeletonPoint> deltaSkeleton = new Dictionary<JointType, SkeletonPoint>();

            if (end < timeList.Count && start < timeList.Count)
            {
                foreach (var k in skeletonSerie.Keys)
                {
                    deltaSkeleton.Add(k, new SkeletonPoint
                    {
                        X = skeletonSerie[k][end].Position.X - skeletonSerie[k][start].Position.X,
                        Y = skeletonSerie[k][end].Position.Y - skeletonSerie[k][start].Position.Y,
                        Z = skeletonSerie[k][end].Position.Z - skeletonSerie[k][start].Position.Z
                    });
                }
                return deltaSkeleton;
            }
            else
                throw new Exception("Indexs not in the Range");
        }

        public Dictionary<JointType, float> CalculateDeltaLenght(int end, int start = 0)
        {
            Dictionary<JointType, SkeletonPoint> deltaSkeleton = CalculateDelta(end, start);
            Dictionary<JointType, float> deltaSkeletonLenght = new Dictionary<JointType, float>();

            foreach (var k in skeletonSerie.Keys)
            {
                deltaSkeletonLenght.Add(k, (float)Math.Sqrt(
               (deltaSkeleton[k].X * deltaSkeleton[k].X) +
               (deltaSkeleton[k].Y * deltaSkeleton[k].Y) +
               (deltaSkeleton[k].Z * deltaSkeleton[k].Z)));
            }

            return deltaSkeletonLenght;
        }

        public GlobalDataSTS CalculateGlobalData(int end, int start = 0)
        {

            var delta = CalculateDeltaLenght(end, start).Values;

            GlobalDataSTS g = new GlobalDataSTS();
            g.count = timeList.Count();
            g.sum = delta.Sum();
            g.avarage = delta.Average();

            double sumOfSquaresOfDifferences = delta.Select(val => (val - g.avarage) * (val - g.avarage)).Sum();
            g.stdDev = (float)Math.Sqrt(sumOfSquaresOfDifferences / delta.Count);
            
            return g;
        }

        public Dictionary<BoneType, float> CalculateAvarageSkeleton()
        {

            AvarageSkeleton[BoneType.FemurLeft] = CalculateAvarageBone(JointType.HipLeft, JointType.KneeLeft);
            AvarageSkeleton[BoneType.FemurRight] = CalculateAvarageBone(JointType.HipRight, JointType.KneeRight);
            AvarageSkeleton[BoneType.TibiaLeft] = CalculateAvarageBone(JointType.AnkleLeft, JointType.KneeLeft);
            AvarageSkeleton[BoneType.TibiaRight] = CalculateAvarageBone(JointType.AnkleRight, JointType.KneeRight);

            AvarageSkeleton[BoneType.HumerusLeft] = CalculateAvarageBone(JointType.ShoulderLeft, JointType.ElbowLeft);
            AvarageSkeleton[BoneType.HumerusRight] = CalculateAvarageBone(JointType.ShoulderRight, JointType.ElbowRight);
            AvarageSkeleton[BoneType.ForearmLeft] = CalculateAvarageBone(JointType.ElbowLeft, JointType.WristLeft);
            AvarageSkeleton[BoneType.ForearmRight] = CalculateAvarageBone(JointType.ElbowRight, JointType.WristRight);

            AvarageSkeleton[BoneType.ClavicleLeft] = CalculateAvarageBone(JointType.ShoulderLeft, JointType.ShoulderCenter);
            AvarageSkeleton[BoneType.ClavicleRight] = CalculateAvarageBone(JointType.ShoulderRight, JointType.ShoulderCenter);


            AvarageSkeleton[BoneType.Neck] = CalculateAvarageBone(JointType.Head, JointType.ShoulderCenter);
            AvarageSkeleton[BoneType.Spine] = CalculateAvarageBone(JointType.HipCenter, JointType.ShoulderCenter);

            return AvarageSkeleton;
        }

        private float CalculateBoneLenght(Joint join1, Joint join2)
        {
            if (join1.TrackingState != JointTrackingState.NotTracked &&
                join2.TrackingState != JointTrackingState.NotTracked)
            {
                return (float)Math.Sqrt((double)(
                              Math.Pow(join1.Position.X - join2.Position.X, 2) +
                              Math.Pow(join1.Position.Y - join2.Position.Y, 2) +
                              Math.Pow(join1.Position.Z - join2.Position.Z, 2)
                          ));
            }
            return 0;
        }

        private float CalculateAvarageBone(JointType type1, JointType type2)
        {
            float bone = 0, sum = 0;
            int count = 0;
            for (int i = 0; i < timeList.Count - 1; i++)
            {
                bone = CalculateBoneLenght(skeletonSerie[type1][i], skeletonSerie[type2][i]);
                if (bone != 0)
                {
                    sum += bone;
                    count++;
                }
            }
            return sum / count;
        }


    }

    public class GlobalDataSTS
    {
        public int count {get; set;}
        public float sum { get; set; }
        public float avarage { get; set; }
        public float stdDev { get; set; }

        public GlobalDataSTS() { }
    }
}
