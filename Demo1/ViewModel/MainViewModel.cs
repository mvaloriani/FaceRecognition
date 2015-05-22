using GalaSoft.MvvmLight;
using System;
using System.Windows.Media.Imaging;

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

        public const string BackgroundBitmapPropertyName = "BackgroundBitmap";
        private BitmapImage _backgroundBitmap = null;
        public BitmapImage BackgroundBitmap
        {
            get { return _backgroundBitmap; }
            set
            {
                if (_backgroundBitmap == value)
                { return; }
                _backgroundBitmap = value;
                RaisePropertyChanged(BackgroundBitmapPropertyName);
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

        /// <summary>
        /// Initializes a new instance of the MainViewModel class.
        /// </summary>
        public MainViewModel()
        {
            
        }

        internal void Initialize()
        {
            
        }

        internal void SelectImage()
        {
            // Create OpenFileDialog 
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();



            // Set filter for file extension and default file extension 
            dlg.DefaultExt = ".png";
            dlg.Filter = "JPEG Files (*.jpeg)|*.jpeg|PNG Files (*.png)|*.png|JPG Files (*.jpg)|*.jpg|GIF Files (*.gif)|*.gif";


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
    }
}