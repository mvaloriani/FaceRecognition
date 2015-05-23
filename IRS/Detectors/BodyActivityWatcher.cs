using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IRS.Data;
using Microsoft.Kinect;

namespace IRS.Detectors
{
    public class BodyActivityWatcher : IComparable<BodyActivityWatcher>
    {
        private const float ActivityFalloff = 0.98f;
        private float activityLevel;
        public SkeletonTimeSerie skeletonTimeSerie {get; private set;}

        public int TrackingId { get; private set; }
        public bool Updated { get; private set; }

        public BodyActivityWatcher(int trackingId)
        {
            this.activityLevel = 0.0f;
            this.TrackingId = trackingId;
            this.skeletonTimeSerie = new SkeletonTimeSerie(trackingId,40);
        }

        public BodyActivityWatcher(Skeleton s)
            : this(s.TrackingId)
        {
            this.skeletonTimeSerie = new SkeletonTimeSerie(s,40);
        }

        public int CompareTo(BodyActivityWatcher other)
        {
            if (null == other)
            {
                return -1;
            }

            // Use the existing CompareTo on float, but reverse the arguments,
            // since we wish to have larger activityLevels sort ahead of smaller values.
            return other.activityLevel.CompareTo(this.activityLevel);
        }


        public void Update(Skeleton s)
        {
            skeletonTimeSerie.Add(s);
            //try
            //{
            //    Dictionary<JointType, float> deltaLenghts = skeletonTimeSerie.CalculateDeltaLenght(skeletonTimeSerie.timeList.Count, skeletonTimeSerie.timeList.Count - 1);

            //    float deltaSumLength = deltaLenghts.Values.Sum();
            //    float deltaAvarageLength = deltaLenghts.Values.Average();

            //    this.activityLevel = this.activityLevel * ActivityFalloff;
            //    this.activityLevel += deltaAvarageLength;
            //}

            //catch (Exception e) {
            //    Debug.WriteLine(e.Message);
            //}

        }



    }

}
