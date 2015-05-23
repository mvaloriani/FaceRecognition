using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.Serialization;

namespace IRS.Data
{
    public enum Gender
    {
        male,
        famele,
        unknown
    }

    public enum Age
    {
        adult,
        child,
        unknown
    }

    public class UserProfile : INotifyPropertyChanged
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

        private Gender _userGender;
        public Gender UserGender
        {
            get { return _userGender; }
            set
            {
                _userGender = value;
                NotifyPropertyChanged("UserGender");

            }
        }

        private Age _userAge;
        public Age UserAge
        {
            get { return _userAge; }
            set
            {
                _userAge = value;
                NotifyPropertyChanged("UserAge");
            }
        }

        private double _userInterest;
        public double UserInterest
        {
            get { return _userInterest; }
            set
            {
                _userInterest = value;
                NotifyPropertyChanged("UserInterest");
            }
        }

        private System.Windows.Media.Imaging.BitmapSource _userImage;
        [IgnoreDataMember]
        public System.Windows.Media.Imaging.BitmapSource UserImage
        {
            get { return _userImage; }
            set
            {
                _userImage = value;
                NotifyPropertyChanged("UserImage");
            }
        }

        public int UserTrakingID { get; set; }
        public Guid UserID;

        [IgnoreDataMember]
        public bool staticAnalisysPerformed { get; set; }

        public ObservableCollection<double> AttentionDatas { get; set; }
        public ObservableCollection<GlobalDataSTS> MovementDatas { get; set; }


        public DateTime StartTime { get; private set; }
        public DateTime EndTime { get; set; }

        public UserProfile(int id)
        {
            StartTime = new DateTime();
            StartTime = DateTime.Now;
            UserGender = Gender.unknown;
            UserAge = Age.unknown;
            UserTrakingID = id;
            UserID = Guid.NewGuid();
            AttentionDatas = new ObservableCollection<double>();
            MovementDatas = new ObservableCollection<GlobalDataSTS>();
           
        }

       
    }
}
