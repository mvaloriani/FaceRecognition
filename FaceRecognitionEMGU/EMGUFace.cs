using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace FaceRecognitionEMGU
{
    public enum Gender
    {
        Male,
        Female,
        Unknown
    }

    public class EMGUFace
    {
        public int x, y;

        public double height, width;

        public Gender gender;

        public double confidence;


        public List<Point> eyes = new List<Point>();


        public override string ToString()
        {
            return "X: " + x + ", Y: " + y + "; Height: " + height + " ; Width: " + width + " ; Gender: " + gender + " ; Conf: " + confidence;
        }

    }
}
