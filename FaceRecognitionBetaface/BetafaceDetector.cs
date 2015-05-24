using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

namespace FaceRecognitionBetaface
{
    public class BetafaceDetector
    {
        public BetafaceDetector() { }

        //public BetafaceImageInfoResponse GetUserInfoObject(Image userImage)
        //{
        //    //Image userImage = Image.FromFile(photoID);
        //    string base64ImageAndTag = ToBase64String(userImage);

        //    WebRequest request = WebRequest.Create("http://www.betafaceapi.com/service.svc/UploadNewImage_File");

        //    // Set the Method property of the request to POST.
        //    request.Method = "POST";

        //    // Set the ContentType property of the WebRequest.
        //    request.ContentType = "application/xml";

        //    // Create POST data and convert it to a byte array.
        //    string postData = "<?xml version=\"1.0\"?><ImageRequestBinary><api_key>d45fd466-51e2-4701-8da8-04351c872236</api_key><api_secret>171e8465-f548-401d-b63b-caf0dc28df5f</api_secret><detection_flags>cropface,classifiers</detection_flags><imagefile_data>";
        //    postData += base64ImageAndTag + "</imagefile_data><original_filename>sample1.jpg</original_filename></ImageRequestBinary>";
        //    byte[] byteArray = Encoding.UTF8.GetBytes(postData);

        //    // Set the ContentLength property of the WebRequest.
        //    request.ContentLength = byteArray.Length;

        //    // Get the request stream.
        //    Stream dataStream = request.GetRequestStream();

        //    // Write the data to the request stream.
        //    dataStream.Write(byteArray, 0, byteArray.Length);

        //    // Close the Stream object.
        //    dataStream.Close();

        //    // Get the response.

        //    WebResponse response = request.GetResponse();

        //    // Get the stream containing content returned by the server.
        //    dataStream = response.GetResponseStream();

        //    // Open the stream using a StreamReader for easy access.
        //    StreamReader reader = new StreamReader(dataStream);

        //    // Read the content.
        //    String responseFromServer = reader.ReadToEnd();
        //    XmlReader xmlReader = XmlReader.Create(new StringReader(responseFromServer));



        //    bool trovato = false;
        //    string img_id = null;

        //    while (xmlReader.Read() && !trovato)
        //    {
        //        if (xmlReader.Name == "img_uid")
        //        {
        //            img_id = xmlReader.ReadString();
        //            trovato = true;
        //        }
        //    }
        //    reader.Close();
        //    dataStream.Close();
        //    response.Close();

        //    response = null;
        //    String int_response;

        //    do
        //    {
        //        int_response = "-1";
        //        request = WebRequest.Create("http://www.betafaceapi.com/service.svc/GetImageInfo");
        //        request.Method = "POST";
        //        request.ContentType = "application/xml";
        //        postData = "<?xml version=\"1.0\"?><ImageInfoRequestUid><api_key>d45fd466-51e2-4701-8da8-04351c872236</api_key><api_secret>171e8465-f548-401d-b63b-caf0dc28df5f</api_secret>";
        //        postData += "<img_uid>" + img_id + "</img_uid></ImageInfoRequestUid>";

        //        byteArray = Encoding.UTF8.GetBytes(postData);
        //        request.ContentLength = byteArray.Length;
        //        dataStream = request.GetRequestStream();


        //        dataStream.Write(byteArray, 0, byteArray.Length);
        //        dataStream.Close();


        //        response = request.GetResponse();
        //        dataStream = response.GetResponseStream();
        //        reader = new StreamReader(dataStream);
        //        responseFromServer = reader.ReadToEnd();
        //        xmlReader = XmlReader.Create(new StringReader(responseFromServer));

        //        while (xmlReader.Read() && int_response == "-1")
        //        {
        //            if (xmlReader.Name == "int_response")
        //            {
        //                int_response = xmlReader.ReadString();
        //            }
        //        }
        //    } while (int_response != "0");

        //    trovato = false;

        //    XmlSerializer serializer = new XmlSerializer(typeof(BetafaceImageInfoResponse));
            
        //    xmlReader = XmlReader.Create(new StringReader(responseFromServer));
        //    BetafaceImageInfoResponse result = (BetafaceImageInfoResponse)serializer.Deserialize(xmlReader);


        //    reader.Close();
        //    dataStream.Close();
        //    response.Close();


        //    return result;
        //}

        public BetafaceDetectorResult StartUserDetection(Image userImage)
        {
            //Image userImage = Image.FromFile(photoID);
            string base64ImageAndTag = ToBase64String(userImage);

            WebRequest request = WebRequest.Create("http://www.betafaceapi.com/service.svc/UploadNewImage_File");

            // Set the Method property of the request to POST.
            request.Method = "POST";

            // Set the ContentType property of the WebRequest.
            request.ContentType = "application/xml";

            // Create POST data and convert it to a byte array.
            string postData = "<?xml version=\"1.0\"?><ImageRequestBinary><api_key>d45fd466-51e2-4701-8da8-04351c872236</api_key><api_secret>171e8465-f548-401d-b63b-caf0dc28df5f</api_secret><detection_flags>cropface,classifiers</detection_flags><imagefile_data>";
            postData += base64ImageAndTag + "</imagefile_data><original_filename>sample1.jpg</original_filename></ImageRequestBinary>";
            byte[] byteArray = Encoding.UTF8.GetBytes(postData);

            // Set the ContentLength property of the WebRequest.
            request.ContentLength = byteArray.Length;

            // Get the request stream.
            Stream dataStream = request.GetRequestStream();

            // Write the data to the request stream.
            dataStream.Write(byteArray, 0, byteArray.Length);

            // Close the Stream object.
            dataStream.Close();

            // Get the response.

            WebResponse response = request.GetResponse();

            // Get the stream containing content returned by the server.
            dataStream = response.GetResponseStream();

            // Open the stream using a StreamReader for easy access.
            StreamReader reader = new StreamReader(dataStream);

            // Read the content.
            String responseFromServer = reader.ReadToEnd();
            XmlReader xmlReader = XmlReader.Create(new StringReader(responseFromServer));



            bool trovato = false;
            string img_id = null;

            while (xmlReader.Read() && !trovato)
            {
                if (xmlReader.Name == "img_uid")
                {
                    img_id = xmlReader.ReadString();
                    trovato = true;
                }
            }
            reader.Close();
            dataStream.Close();
            response.Close();

            response = null;
            String int_response;

            do
            {
                int_response = "-1";
                request = WebRequest.Create("http://www.betafaceapi.com/service.svc/GetImageInfo");
                request.Method = "POST";
                request.ContentType = "application/xml";
                postData = "<?xml version=\"1.0\"?><ImageInfoRequestUid><api_key>d45fd466-51e2-4701-8da8-04351c872236</api_key><api_secret>171e8465-f548-401d-b63b-caf0dc28df5f</api_secret>";
                postData += "<img_uid>" + img_id + "</img_uid></ImageInfoRequestUid>";

                byteArray = Encoding.UTF8.GetBytes(postData);
                request.ContentLength = byteArray.Length;
                dataStream = request.GetRequestStream();


                dataStream.Write(byteArray, 0, byteArray.Length);
                dataStream.Close();


                response = request.GetResponse();
                dataStream = response.GetResponseStream();
                reader = new StreamReader(dataStream);
                responseFromServer = reader.ReadToEnd();
                xmlReader = XmlReader.Create(new StringReader(responseFromServer));

                while (xmlReader.Read() && int_response == "-1")
                {
                    if (xmlReader.Name == "int_response")
                    {
                        int_response = xmlReader.ReadString();
                    }
                }
            } while (int_response != "0");

            trovato = false;

            XmlSerializer serializer = new XmlSerializer(typeof(BetafaceImageInfoResponse));

            xmlReader = XmlReader.Create(new StringReader(responseFromServer));
            BetafaceImageInfoResponse betafaceObjectResult = (BetafaceImageInfoResponse)serializer.Deserialize(xmlReader);

            //close streams
            reader.Close();
            dataStream.Close();
            response.Close();

            BetafaceDetectorResult result = new BetafaceDetectorResult();
            result.BetafaceXMLResponse = responseFromServer;
            result.BetafaceObjectResponse = betafaceObjectResult;


            return result;
        }

        /// <summary>
        /// Convert the image to a Base 64 string
        /// </summary>
        /// <param name="image">the image to convert</param>
        /// <returns>the raw data representing the image</returns>
        private string ToBase64String(Image image)
        {
            using (MemoryStream stream = new MemoryStream())
            {
                image.Save(stream, image.RawFormat);
                return Convert.ToBase64String(stream.ToArray());
            }
        }

        //private string ToBase64String(BitmapSource bitmapSource)
        //{
        //    JpegBitmapEncoder encoder = new JpegBitmapEncoder();
        //    encoder.Frames.Add(BitmapFrame.Create(bitmapSource));
        //    encoder.QualityLevel = 100;
        //    byte[] bytes = new byte[0];
        //    using (MemoryStream stream = new MemoryStream())
        //    {
        //        encoder.Frames.Add(BitmapFrame.Create(bitmapSource));
        //        encoder.Save(stream);
        //        bytes = stream.ToArray();

        //        string base64String = Convert.ToBase64String(bytes);
        //        stream.Close();

        //        return base64String;
        //    }
        //}
    }

    public class BetafaceDetectorResult
    {
        private string betafaceXMLResponse;

        private BetafaceImageInfoResponse betafaceObjectResponse;

        public BetafaceDetectorResult() { }

        public string BetafaceXMLResponse
        {
            get { return betafaceXMLResponse; }
            set { betafaceXMLResponse = value; }
        }

        public BetafaceImageInfoResponse BetafaceObjectResponse
        {
            get { return betafaceObjectResponse; }
            set { betafaceObjectResponse = value; }
        }
    }
}
