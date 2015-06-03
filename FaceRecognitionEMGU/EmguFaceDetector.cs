using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Emgu.CV;
using Emgu.Util;
using Emgu.CV.Structure;
using Emgu.CV.Face;


using System.IO;
using System.Drawing;
using System.Runtime.InteropServices;




namespace FaceRecognitionEMGU
{


    public class EmguFaceDetector
    {


        private List<Image<Gray, byte>> images;
        private List<int> labels;


        //const attrs
        private const string CROPPED_IMAGE_DIR = @"cropped_image";
        private const string CROPPED_IMAGE_FILE = @"cropped_image.conf";
        private const char SEPARATOR = ';';



        //public FaceRecognizer model = new FisherFaceRecognizer(0, double.MaxValue);
        public FaceRecognizer model;
        public CascadeClassifier cascadeEyes;
        public CascadeClassifier cascadeFace;

        public async Task<List<EMGUFace>> detect(System.Drawing.Bitmap img)
        {
            // Normalizing it to grayscale
            Image<Gray, byte> normalizedMasterImage = new Image<Gray, byte>(img);

            return detect(normalizedMasterImage).Result;
        }

        public async Task<List<EMGUFace>> detect(Image<Gray, byte> img)
        {
            List<EMGUFace> faces = new List<EMGUFace>();

            if (img == null) return faces;

          //  var rect = cascade.DetectMultiScale(img, 1.4, 0, new Size(100, 100), new Size(800, 800));
            var rect = cascadeFace.DetectMultiScale(img, 1.2, 10);


            foreach (var r in rect)
            {
                Image<Gray, byte> imgBox = img.GetSubRect(r);

                var eyes = cascadeEyes.DetectMultiScale(imgBox, 1.2, 10);

                if (imgBox.Height != 200 || imgBox.Width != 200)
                    imgBox = imgBox.Resize(200, 200, Emgu.CV.CvEnum.Inter.Cubic);


                var res = model.Predict(imgBox);

                EMGUFace face = new EMGUFace();
                face.x= r.X;
                face.y = r.Y;
                face.width = r.Width;
                face.height=r.Height;
                face.gender = (res.Label == 0) ? Gender.Male : Gender.Female;
                face.confidence = res.Distance;

                foreach (var eye in eyes)
                    face.eyes.Add(new System.Windows.Point(eye.X + eye.Width / 2, eye.Y + eye.Height / 2));

                faces.Add(face);
            }
            return faces;
        }


        public EmguFaceDetector(String preloadedTraining = "")
        {
           // model = new LBPHFaceRecognizer();
           // model = new EigenFaceRecognizer();
           
            model = new FisherFaceRecognizer(2, 3000);

            if (preloadedTraining == "")
            {
                images = new List<Image<Gray, byte>>();
                labels = new List<int>();
                prepareTrainingData();
                model.Train(images.ToArray(), labels.ToArray());
                model.Save("Default");
            }
            else
            {
                if (preloadedTraining == "Default")
                    model.Load(preloadedTraining);
                else
                    model.Load(preloadedTraining);
            }


            cascadeFace = new CascadeClassifier(AppDomain.CurrentDomain.BaseDirectory + "haarcascades\\haarcascade_frontalface_default.xml");
            cascadeEyes = new CascadeClassifier(AppDomain.CurrentDomain.BaseDirectory + "haarcascades\\haarcascade_eye.xml");


        }



        private void prepareTrainingData()
        {

            string file = AppDomain.CurrentDomain.BaseDirectory + CROPPED_IMAGE_DIR + "\\" + CROPPED_IMAGE_FILE;

            if (!File.Exists(file))
            {
                string errMsg = "No valid input file was given, please check the given filename.";
                System.Console.WriteLine(errMsg);
                throw new Exception(errMsg);
            }

            try
            {
                StreamReader rd = new StreamReader(new FileStream(file, FileMode.Open, FileAccess.Read));
                string line, path, label;
                while ((line = rd.ReadLine()) != null)
                {
                    String[] pathInfos = line.Split(SEPARATOR);
                    if (pathInfos.Length > 0)
                    {
                        path = pathInfos[0];
                        label = pathInfos[1];
                        Image<Gray, byte> img = null;

                        if (path != null && label != null)
                        {

                            path = AppDomain.CurrentDomain.BaseDirectory + CROPPED_IMAGE_DIR + "\\" + path + ".jpg";

                            if (File.Exists(path))
                            {
                                img = new Image<Gray, byte>(path);
                                if (img.Height != 200 || img.Width != 200)
                                    img = img.Resize(200, 200, Emgu.CV.CvEnum.Inter.Cubic);

                                images.Add(img);
                                labels.Add(int.Parse(label));
                            }
                        }
                    }
                }//end of while

                rd.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

    }
}
