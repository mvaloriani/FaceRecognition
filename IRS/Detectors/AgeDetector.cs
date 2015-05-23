using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IRS.Data;

namespace IRS.Detectors
{
    public class AgeDetector
    {
        private const float humerusAdultLenght = 0.20f;
        private const float humerusChildLenght = 0.15f;

        private const float forearmAdultLenght = 0.15f;
        private const float forearmChildLenght = 0.10f;

        private const float clavicleAdultLenght = 0.10f;
        private const float clavicleChildLenght = 0.7f;

        private const float femurAdultLenght = 0.30f;
        private const float femurChildLenght = 0.20f;

        private const float tibiaAdultLenght = 0.25f;
        private const float tibiaChildLenght = 0.15f;

        private const float adultLenght = 1.50f;
        private const float childLenght = 1.20f;

        private Dictionary<BoneType, List<float>> bonesLenght = new Dictionary<BoneType, List<float>>(){        
            {BoneType.HumerusLeft, new List<float>(){humerusAdultLenght,humerusChildLenght} },
            {BoneType.HumerusRight, new List<float>(){humerusAdultLenght,humerusChildLenght} },
            {BoneType.ForearmLeft, new List<float>(){forearmAdultLenght,forearmChildLenght} },
            {BoneType.ForearmRight, new List<float>(){forearmAdultLenght,forearmChildLenght} },
            {BoneType.ClavicleLeft, new List<float>(){clavicleAdultLenght,clavicleChildLenght} },
            {BoneType.ClavicleRight, new List<float>(){clavicleAdultLenght,clavicleChildLenght} },
            {BoneType.FemurLeft, new List<float>(){femurAdultLenght,femurChildLenght} },
            {BoneType.FemurRight, new List<float>(){femurAdultLenght,femurChildLenght} },
            {BoneType.TibiaLeft, new List<float>(){tibiaAdultLenght,tibiaChildLenght} },
            {BoneType.TibiaRight, new List<float>(){tibiaAdultLenght,tibiaChildLenght} }                      
        };

        public Age Analize(Dictionary<BoneType, float> SkeletonBones)
        {
            float userHeight1 = SkeletonBones[BoneType.TibiaRight] + SkeletonBones[BoneType.FemurRight] +
                SkeletonBones[BoneType.Spine] * 1.10f + SkeletonBones[BoneType.Neck] * 2.0f;
             
            float userHeight2 = SkeletonBones[BoneType.HumerusLeft]*3.26f+0.6210f;
            float userHeight3 = SkeletonBones[BoneType.ForearmLeft] * 3.42f + 0.8156f;

            float userHeight = (userHeight1 + userHeight2 + userHeight3) / 3f * 1.2f;
                        

            List<Age> agesEstimate = new List<Age>();
            agesEstimate.Add(EstimateAgeFromBones(userHeight, adultLenght, childLenght));

            foreach (var bone in SkeletonBones.Keys)
            {
                agesEstimate.Add(EstimateAgeFromBones(SkeletonBones[bone], bone));
            }

            int adult = agesEstimate.Count(x => x == Age.adult);
            int child = agesEstimate.Count(x => x == Age.child);
            int unknown = agesEstimate.Count(x => x == Age.unknown);

            if(adult>child && adult>unknown)
                return Age.adult;
            if(child>adult && child>unknown)
                return Age.child;
          
            return Age.unknown;

        }

        private Age EstimateAgeFromBones(float lenght, float adultLenght, float childLenght)
        {

            if (lenght > adultLenght)
            {
                return Age.adult;
            }
            else if (lenght < childLenght && lenght!=0)
            {
                return Age.child;
            }

            return Age.unknown;


        }

        private Age EstimateAgeFromBones(float lenght, BoneType type)
        {
            if (bonesLenght.Keys.Contains(type))
            {
                return EstimateAgeFromBones(lenght*1.2f, bonesLenght[type][0], bonesLenght[type][1]);
            }
            else
                return Age.unknown;


        }

        
    }
}
